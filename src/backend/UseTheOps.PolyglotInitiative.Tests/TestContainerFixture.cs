using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Testcontainers;
using UseTheOps.PolyglotInitiative.Tests;
using Microsoft.Extensions.Logging;

namespace UseTheOps.PolyglotInitiative.Tests
{
    /// <summary>
    /// Provides a test fixture that manages a PostgreSQL test container, a test web application factory, and an authenticated HTTP client for integration tests.
    /// Ensures a clean database state and injects an in-memory logger for observability.
    /// </summary>
    public class TestContainerFixture
    {
        /// <summary>
        /// Gets the PostgreSQL test container instance.
        /// </summary>
        public PostgreSqlContainer PgContainer { get; private set; }

        /// <summary>
        /// Gets the WebApplicationFactory used to create the test server and services.
        /// </summary>
        public WebApplicationFactory<Program> Factory { get; private set; }

        /// <summary>
        /// Gets the HTTP client configured to communicate with the test server.
        /// </summary>
        public HttpClient Client { get; private set; }

        /// <summary>
        /// Gets the in-memory logger provider for capturing logs during tests.
        /// </summary>
        public InMemoryLoggerProvider InMemoryLoggerProvider { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestContainerFixture"/> class.
        /// </summary>
        public TestContainerFixture()
        {
            PgContainer = new PostgreSqlBuilder()
                .WithDatabase("polyglot_test")
                .WithPassword("test")
                .WithUsername("test")
                .Build();
        }

        /// <summary>
        /// Starts the PostgreSQL container, configures the test server, resets the database, and creates a default admin user.
        /// </summary>
        public async Task InitializeAsync()
        {
            await PgContainer.StartAsync();
            // Set the environment variable for the backend connection string
            Environment.SetEnvironmentVariable("PG_CONNECTION_STRING", PgContainer.GetConnectionString());
            InMemoryLoggerProvider = new InMemoryLoggerProvider();
            // Set a valid JWT secret for authentication
            Environment.SetEnvironmentVariable("JWT_SECRET", "SuperSecretKeyForJwtToken123456!@#");
            Factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        // Replace the DbContext with one using the test container's connection string
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<UseTheOps.PolyglotInitiative.Data.PolyglotInitiativeDbContext>));
                        if (descriptor != null) services.Remove(descriptor);
                        services.AddDbContext<UseTheOps.PolyglotInitiative.Data.PolyglotInitiativeDbContext>(options =>
                            options.UseNpgsql(PgContainer.GetConnectionString()));
                    });
                    builder.ConfigureServices(services =>
                    {
                        // Inject the in-memory logger for capturing logs
                        services.AddSingleton<ILoggerProvider>(InMemoryLoggerProvider);
                    });
                    // builder.UseEnvironment("Development"); // Not supported here
                });
            Client = Factory.CreateClient();

            // Truncate all tables for a clean state before each test run
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<UseTheOps.PolyglotInitiative.Data.PolyglotInitiativeDbContext>();
            // Apply migrations to ensure schema is up to date
            db.Database.Migrate();
            await db.Database.OpenConnectionAsync();
            var tables = new[] {
                "ResourceTranslations", "TranslatableResources", "ResourceFiles", "Components", "Projects", "Solutions", "TranslationNeeds", "Users", "UserSolutionAccesses", "ApiKeys", "ExternalIdentifiers"
            };
            foreach (var table in tables)
            {
                // Table names are hardcoded and not user input, so this is safe
#pragma warning disable EF1002 // Risk of vulnerability to SQL injection.
                await db.Database.ExecuteSqlRawAsync($"TRUNCATE \"{table}\" RESTART IDENTITY CASCADE;");
#pragma warning restore EF1002 // Risk of vulnerability to SQL injection.
            }

            // Create a default admin user if none exists
            if (!db.Users.Any())
            {
                var adminEmail = "admin@example.com";
                var adminPassword = "TestPassword123!";
                var adminUser = new UseTheOps.PolyglotInitiative.Models.User
                {
                    Name = "Admin",
                    Email = adminEmail,
                    PasswordHash = UseTheOps.PolyglotInitiative.Services.UserService.HashPassword(adminPassword),
                    IsAdministrator = true,
                    Status = "confirmed"
                };
                db.Users.Add(adminUser);
                db.SaveChanges();
            }

            await db.Database.CloseConnectionAsync();
        }

        /// <summary>
        /// Disposes the HTTP client, test server, and stops the PostgreSQL container.
        /// </summary>
        public async Task DisposeAsync()
        {
            Client?.Dispose();
            if(Factory!= null)
                await Factory.DisposeAsync();
            await PgContainer.StopAsync();
        }

        /// <summary>
        /// Authenticates the test HTTP client as the default admin user and sets the Authorization header.
        /// </summary>
        /// <returns>A task that completes when authentication is done.</returns>
        /// <exception cref="Exception">Thrown if authentication fails or no token is returned.</exception>
        public async Task AuthenticateAsAdminAsync()
        {
            var loginRequest = new
            {
                Email = "admin@example.com",
                Password = "TestPassword123!"
            };
            var response = await Client.PostAsJsonAsync("/api/Auth/login", loginRequest);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var headers = string.Join("\n", response.Headers.Select(h => $"{h.Key}: {string.Join(",", h.Value)}"));
                throw new Exception($"/api/Auth/login failed: {(int)response.StatusCode} {response.ReasonPhrase}\nHeaders:\n{headers}\nBody:\n{errorContent}");
            }
            var result = await response.Content.ReadFromJsonAsync<LoginResult>();
            if (result == null || string.IsNullOrWhiteSpace(result.token))
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"/api/Auth/login succeeded but no token returned! Body:\n{errorContent}");
            }
            Client.DefaultRequestHeaders.Remove("Authorization");
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {result.token}");
        }

        /// <summary>
        /// Represents the result of a login request for authentication.
        /// </summary>
        private class LoginResult { public string token { get; set; } }
    }
}
