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
    public class TestContainerFixture
    {
        public PostgreSqlContainer PgContainer { get; private set; }
        public WebApplicationFactory<Program> Factory { get; private set; }
        public HttpClient Client { get; private set; }
        public InMemoryLoggerProvider InMemoryLoggerProvider { get; private set; }

        public TestContainerFixture()
        {
            PgContainer = new PostgreSqlBuilder()
                .WithDatabase("polyglot_test")
                .WithPassword("test")
                .WithUsername("test")
                .Build();
        }

        public async Task InitializeAsync()
        {
            await PgContainer.StartAsync();
            InMemoryLoggerProvider = new InMemoryLoggerProvider();
            // Force la variable d'environnement JWT_SECRET à une valeur valide (32+ caractères)
            Environment.SetEnvironmentVariable("JWT_SECRET", "SuperSecretKeyForJwtToken123456!@#");
            Factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<UseTheOps.PolyglotInitiative.Data.PolyglotInitiativeDbContext>));
                        if (descriptor != null) services.Remove(descriptor);
                        services.AddDbContext<UseTheOps.PolyglotInitiative.Data.PolyglotInitiativeDbContext>(options =>
                            options.UseNpgsql(PgContainer.GetConnectionString()));
                    });
                    builder.ConfigureServices(services =>
                    {
                        services.AddSingleton<ILoggerProvider>(InMemoryLoggerProvider);
                    });
                    // builder.UseEnvironment("Development"); // Supprimé car non supporté ici
                });
            Client = Factory.CreateClient();

            // Truncate all tables for a clean state
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<UseTheOps.PolyglotInitiative.Data.PolyglotInitiativeDbContext>();
            // Appliquer les migrations pour créer le schéma AVANT toute connexion
            db.Database.Migrate();
            await db.Database.OpenConnectionAsync();onten
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

            // Création d'un administrateur de test si aucun utilisateur n'existe
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

        public async Task DisposeAsync()
        {
            Client?.Dispose();
            if(Factory!= null)
                await Factory.DisposeAsync();
            await PgContainer.StopAsync();
        }

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

        private class LoginResult { public string token { get; set; } }
    }
}
