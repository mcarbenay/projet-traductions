using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace UseTheOps.PolyglotInitiative.Tests
{
    public class TestContainerFixture : IAsyncLifetime
    {
        public TestcontainerDatabase PgContainer { get; private set; }
        public WebApplicationFactory<Program> Factory { get; private set; }
        public HttpClient Client { get; private set; }

        public TestContainerFixture()
        {
            PgContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
                .WithDatabase(new PostgreSqlTestcontainerConfiguration
                {
                    Database = "polyglot_test",
                    Username = "test",
                    Password = "test"
                })
                .WithImage("postgres:15-alpine")
                .WithCleanUp(true)
                .Build();
        }

        public async Task InitializeAsync()
        {
            await PgContainer.StartAsync();
            Factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<UseTheOps.PolyglotInitiative.Data.PolyglotInitiativeDbContext>));
                        if (descriptor != null) services.Remove(descriptor);
                        services.AddDbContext<UseTheOps.PolyglotInitiative.Data.PolyglotInitiativeDbContext>(options =>
                            options.UseNpgsql(PgContainer.ConnectionString));
                    });
                });
            Client = Factory.CreateClient();

            // Truncate all tables for a clean state
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<UseTheOps.PolyglotInitiative.Data.PolyglotInitiativeDbContext>();
            await db.Database.OpenConnectionAsync();
            var tables = new[] {
                "ResourceTranslations", "TranslatableResources", "ResourceFiles", "Components", "Projects", "Solutions", "TranslationNeeds", "Users", "UserSolutionAccesses", "ApiKeys", "ExternalIdentifiers"
            };
            foreach (var table in tables)
            {
                // Table names are hardcoded and not user input, so this is safe
                await db.Database.ExecuteSqlRawAsync($"TRUNCATE \"{table}\" RESTART IDENTITY CASCADE;");
            }
            await db.Database.CloseConnectionAsync();
        }

        public async Task DisposeAsync()
        {
            Client?.Dispose();
            Factory?.Dispose();
            await PgContainer.StopAsync();
        }
    }
}
