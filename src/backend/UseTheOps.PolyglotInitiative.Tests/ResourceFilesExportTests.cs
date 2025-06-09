using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UseTheOps.PolyglotInitiative.Models;
using UseTheOps.PolyglotInitiative.Tests;

namespace UseTheOps.PolyglotInitiative.Tests
{
    [TestClass]
    public class ResourceFilesExportTests
    {
        private static TestContainerFixture _fixture;
        private static bool _initialized = false;

        [ClassInitialize]
        public static async Task ClassInit(TestContext context)
        {
            _fixture = new TestContainerFixture();
            await _fixture.InitializeAsync();
            _initialized = true;
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            if (_initialized)
                await _fixture.DisposeAsync();
        }

        [TestInitialize]
        public async Task TestInit()
        {
            _fixture = TestAssemblyInit.Fixture;
            // Truncate all tables for a clean state, sauf Users
            using var scope = _fixture.Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<UseTheOps.PolyglotInitiative.Data.PolyglotInitiativeDbContext>();
            await db.Database.OpenConnectionAsync();
            var tables = new[] {
                "ResourceTranslations", "TranslatableResources", "ResourceFiles", "Components", "Projects", "Solutions", "TranslationNeeds", /*"Users",*/ "UserSolutionAccesses", "ApiKeys", "ExternalIdentifiers"
            };
            foreach (var table in tables)
            {
#pragma warning disable EF1002 // Risk of vulnerability to SQL injection.
                await db.Database.ExecuteSqlRawAsync($"TRUNCATE \"{table}\" RESTART IDENTITY CASCADE;");
#pragma warning restore EF1002 // Risk of vulnerability to SQL injection.
            }
            // Supprimer tous les utilisateurs sauf l'admin de test
            db.Users.RemoveRange(db.Users.Where(u => u.Email != "admin@example.com"));
            await db.SaveChangesAsync();
            // Recréer l'admin si besoin
            if (!db.Users.Any(u => u.Email == "admin@example.com"))
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
            await _fixture.AuthenticateAsAdminAsync();
        }

        [TestMethod]
        public async Task Download_Resx_Export_Works()
        {
            // Arrange: create a solution, project, component, and upload a .resx file
            var solutionId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var componentId = Guid.NewGuid();
            var resourceFileName = "test.resx";
            var resxContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><root><data name=\"Hello\"><value>Hello</value></data><data name=\"World\"><value>World</value></data></root>";

            // Respect entity dependencies: solution > project > component
            using (var scope = _fixture.Factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<UseTheOps.PolyglotInitiative.Data.PolyglotInitiativeDbContext>();
                var userId = Guid.NewGuid();
                db.Users.Add(new Models.User { Id = userId, Name = "Test Owner", Email = "owner@example.com", PasswordHash = "", IsAdministrator = false, Status = "active" });
                db.Solutions.Add(new Models.Solution { Id = solutionId, Code = "sol", Name = "Solution", OwnerId = userId });
                db.Projects.Add(new Models.Project { Id = projectId, Code = "proj", Name = "Project", SolutionId = solutionId });
                db.Components.Add(new Models.Component { Id = componentId, Name = "Component", Code = "comp", ProjectId = projectId });
                await db.SaveChangesAsync();
                await db.Database.CloseConnectionAsync(); // Ensure commit and visibility
            }

            var uploadUrl = $"/api/ResourceFiles/upload?componentId={componentId}&language=en";
            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(resxContent))), "file", resourceFileName);
            var uploadResp = await _fixture.Client.PostAsync(uploadUrl, content);
            if (!uploadResp.IsSuccessStatusCode)
            {
                var errorContent = await uploadResp.Content.ReadAsStringAsync();
                throw new Exception($"Upload failed: {(int)uploadResp.StatusCode} {uploadResp.ReasonPhrase}\nBody:\n{errorContent}");
            }
            var uploaded = await uploadResp.Content.ReadFromJsonAsync<Models.ResourceFile>();
            Assert.IsNotNull(uploaded);

            // Act: download as .resx
            var downloadResp = await _fixture.Client.GetAsync($"/api/ResourceFiles/download/{uploaded.Id}?format=.resx&language=en");
            if (!downloadResp.IsSuccessStatusCode)
            {
                var errorContent = await downloadResp.Content.ReadAsStringAsync();
                throw new Exception($"Download failed: {(int)downloadResp.StatusCode} {downloadResp.ReasonPhrase}\nBody:\n{errorContent}");
            }
            var downloaded = await downloadResp.Content.ReadAsStringAsync();

            // Assert: check XML contains expected keys/values (fallback only, since nothing is translated)
            Console.WriteLine("[DEBUG] RESX content:\n" + downloaded);
            Assert.IsTrue(downloaded.Contains("<data name=\"Hello\" xml:space=\"preserve\">"), "RESX should contain <data name=\"Hello\" xml:space=\"preserve\"> block");
            Assert.IsTrue(downloaded.Contains("<value>##Hello in en##</value>"), "RESX should fallback to ##Hello in en## for Hello");
            Assert.IsTrue(downloaded.Contains("<data name=\"World\" xml:space=\"preserve\">"), "RESX should contain <data name=\"World\" xml:space=\"preserve\"> block");
            Assert.IsTrue(downloaded.Contains("<value>##World in en##</value>"), "RESX should fallback to ##World in en## for World");
            // Optionally, ensure original values are NOT present
            Assert.IsFalse(downloaded.Contains("<value>Hello</value>"), "RESX should not contain untranslated value 'Hello'");
            Assert.IsFalse(downloaded.Contains("<value>World</value>"), "RESX should not contain untranslated value 'World'");
        }

        [TestMethod]
        public async Task Download_Json_Export_Works()
        {
            // Arrange: create a solution, project, component, and upload a .json file
            var solutionId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var componentId = Guid.NewGuid();
            var resourceFileName = "test.json";
            var jsonContent = "{\"Hello\":\"Bonjour\",\"World\":\"Monde\"}";

            // Respect entity dependencies: solution > project > component
            using (var scope = _fixture.Factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<UseTheOps.PolyglotInitiative.Data.PolyglotInitiativeDbContext>();
                var userId = Guid.NewGuid();
                db.Users.Add(new Models.User { Id = userId, Name = "Test Owner", Email = "owner@example.com", PasswordHash = "", IsAdministrator = false, Status = "active" });
                db.Solutions.Add(new Models.Solution { Id = solutionId, Code = "sol", Name = "Solution", OwnerId = userId });
                db.Projects.Add(new Models.Project { Id = projectId, Code = "proj", Name = "Project", SolutionId = solutionId });
                db.Components.Add(new Models.Component { Id = componentId, Name = "Component", Code = "comp", ProjectId = projectId });
                await db.SaveChangesAsync();
                await db.Database.CloseConnectionAsync(); // Ensure commit and visibility
            }

            var uploadUrl = $"/api/ResourceFiles/upload?componentId={componentId}&language=fr";
            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonContent))), "file", resourceFileName);
            var uploadResp = await _fixture.Client.PostAsync(uploadUrl, content);
            if (!uploadResp.IsSuccessStatusCode)
            {
                var errorContent = await uploadResp.Content.ReadAsStringAsync();
                throw new Exception($"Upload failed: {(int)uploadResp.StatusCode} {uploadResp.ReasonPhrase}\nBody:\n{errorContent}");
            }
            var uploaded = await uploadResp.Content.ReadFromJsonAsync<Models.ResourceFile>();
            Assert.IsNotNull(uploaded);

            // Act: download as .json
            var downloadResp = await _fixture.Client.GetAsync($"/api/ResourceFiles/download/{uploaded.Id}?format=.json&language=fr");
            if (!downloadResp.IsSuccessStatusCode)
            {
                var errorContent = await downloadResp.Content.ReadAsStringAsync();
                throw new Exception($"Download failed: {(int)downloadResp.StatusCode} {downloadResp.ReasonPhrase}\nBody:\n{errorContent}");
            }
            var downloaded = await downloadResp.Content.ReadAsStringAsync();

            // Assert: check JSON contains expected keys/values
            Assert.IsTrue(downloaded.Contains("\"Hello\": \"Bonjour\"") || downloaded.Contains("\"Hello\": \"##Hello in fr##\""));
            Assert.IsTrue(downloaded.Contains("\"World\": \"Monde\"") || downloaded.Contains("\"World\": \"##World in fr##\""));
        }

        [TestMethod]
        public async Task Download_Po_Export_Works()
        {
            // Arrange: create a solution, project, component, and upload a .po file
            var solutionId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var componentId = Guid.NewGuid();
            var resourceFileName = "test.po";
            var poContent = "msgid \"Hello\"\nmsgstr \"Hallo\"\n\nmsgid \"World\"\nmsgstr \"Welt\"\n";

            // Respect entity dependencies: solution > project > component
            using (var scope = _fixture.Factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<UseTheOps.PolyglotInitiative.Data.PolyglotInitiativeDbContext>();
                // Insert a User first to satisfy FK constraint for Solution.OwnerId
                var userId = Guid.NewGuid();
                db.Users.Add(new Models.User { Id = userId, Name = "Test Owner", Email = "owner@example.com", PasswordHash = "", IsAdministrator = false, Status = "active" });
                db.Solutions.Add(new Models.Solution { Id = solutionId, Code = "sol", Name = "Solution", OwnerId = userId });
                db.Projects.Add(new Models.Project { Id = projectId, Code = "proj", Name = "Project", SolutionId = solutionId });
                db.Components.Add(new Models.Component { Id = componentId, Name = "Component", Code = "comp", ProjectId = projectId });
                await db.SaveChangesAsync();
                await db.Database.CloseConnectionAsync(); // Ensure commit and visibility
            }

            var uploadUrl = $"/api/ResourceFiles/upload?componentId={componentId}&language=de";
            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(poContent))), "file", resourceFileName);
            var uploadResp = await _fixture.Client.PostAsync(uploadUrl, content);
            if (!uploadResp.IsSuccessStatusCode)
            {
                var errorContent = await uploadResp.Content.ReadAsStringAsync();
                throw new Exception($"Upload failed: {(int)uploadResp.StatusCode} {uploadResp.ReasonPhrase}\nBody:\n{errorContent}");
            }
            var uploaded = await uploadResp.Content.ReadFromJsonAsync<Models.ResourceFile>();
            Assert.IsNotNull(uploaded);

            // Act: download as .po
            var downloadResp = await _fixture.Client.GetAsync($"/api/ResourceFiles/download/{uploaded.Id}?format=.po&language=de");
            if (!downloadResp.IsSuccessStatusCode)
            {
                var errorContent = await downloadResp.Content.ReadAsStringAsync();
                throw new Exception($"Download failed: {(int)downloadResp.StatusCode} {downloadResp.ReasonPhrase}\nBody:\n{errorContent}");
            }
            var downloaded = await downloadResp.Content.ReadAsStringAsync();

            // Assert: check PO contains expected keys/values
            Assert.IsTrue(downloaded.Contains("msgid \"Hello\""));
            Assert.IsTrue(downloaded.Contains("msgstr \"Hallo\"") || downloaded.Contains("msgstr \"##Hello in de##\""));
            Assert.IsTrue(downloaded.Contains("msgid \"World\""));
            Assert.IsTrue(downloaded.Contains("msgstr \"Welt\"") || downloaded.Contains("msgstr \"##World in de##\""));
        }

        [TestMethod]
        public async Task Download_Xliff_Export_Works()
        {
            // Arrange: create a solution, project, component, and upload a .xliff file
            var solutionId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var componentId = Guid.NewGuid();
            var resourceFileName = "test.xliff";
            var xliffContent = "<xliff version=\"1.2\"><file source-language=\"es\" datatype=\"plaintext\" original=\"file.ext\"><body><trans-unit id=\"Hello\"><source>Hello</source><target>Hola</target></trans-unit><trans-unit id=\"World\"><source>World</source><target>Mundo</target></trans-unit></body></file></xliff>";

            // Respect entity dependencies: solution > project > component
            using (var scope = _fixture.Factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<UseTheOps.PolyglotInitiative.Data.PolyglotInitiativeDbContext>();
                var userId = Guid.NewGuid();
                db.Users.Add(new Models.User { Id = userId, Name = "Test Owner", Email = "owner@example.com", PasswordHash = "", IsAdministrator = false, Status = "active" });
                db.Solutions.Add(new Models.Solution { Id = solutionId, Code = "sol", Name = "Solution", OwnerId = userId });
                db.Projects.Add(new Models.Project { Id = projectId, Code = "proj", Name = "Project", SolutionId = solutionId });
                db.Components.Add(new Models.Component { Id = componentId, Name = "Component", Code = "comp", ProjectId = projectId });
                await db.SaveChangesAsync();
                await db.Database.CloseConnectionAsync(); // Ensure commit and visibility
            }

            var uploadUrl = $"/api/ResourceFiles/upload?componentId={componentId}&language=es";
            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xliffContent))), "file", resourceFileName);
            var uploadResp = await _fixture.Client.PostAsync(uploadUrl, content);
            if (!uploadResp.IsSuccessStatusCode)
            {
                var errorContent = await uploadResp.Content.ReadAsStringAsync();
                throw new Exception($"Upload failed: {(int)uploadResp.StatusCode} {uploadResp.ReasonPhrase}\nBody:\n{errorContent}");
            }
            var uploaded = await uploadResp.Content.ReadFromJsonAsync<Models.ResourceFile>();
            Assert.IsNotNull(uploaded);

            // Act: download as .xliff
            var downloadResp = await _fixture.Client.GetAsync($"/api/ResourceFiles/download/{uploaded.Id}?format=.xliff&language=es");
            if (!downloadResp.IsSuccessStatusCode)
            {
                var errorContent = await downloadResp.Content.ReadAsStringAsync();
                throw new Exception($"Download failed: {(int)downloadResp.StatusCode} {downloadResp.ReasonPhrase}\nBody:\n{errorContent}");
            }
            var downloaded = await downloadResp.Content.ReadAsStringAsync();

            // Assert: check XLIFF contains expected keys/values
            Assert.IsTrue(downloaded.Contains("<trans-unit id=\"Hello\">"));
            Assert.IsTrue(downloaded.Contains("<target>Hola</target>") || downloaded.Contains("<target>##Hello in es##</target>"));
            Assert.IsTrue(downloaded.Contains("<trans-unit id=\"World\">"));
            Assert.IsTrue(downloaded.Contains("<target>Mundo</target>") || downloaded.Contains("<target>##World in es##</target>"));
        }

        [TestMethod]
        public async Task Download_Export_Language_Selection_Works()
        {
            // Arrange: create a solution, project, component, and upload a JSON file with two languages
            var solutionId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var componentId = Guid.NewGuid();
            var resourceFileName = "test.json";
            var jsonContent = "{\"en\":{\"Hello\":\"Hello\",\"World\":\"World\"},\"fr\":{\"Hello\":\"Bonjour\",\"World\":\"Monde\"}}";

            // Respect entity dependencies: solution > project > component
            using (var scope = _fixture.Factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<UseTheOps.PolyglotInitiative.Data.PolyglotInitiativeDbContext>();
                var userId = Guid.NewGuid();
                db.Users.Add(new Models.User { Id = userId, Name = "Test Owner", Email = "owner@example.com", PasswordHash = "", IsAdministrator = false, Status = "active" });
                db.Solutions.Add(new Models.Solution { Id = solutionId, Code = "sol", Name = "Solution", OwnerId = userId });
                db.Projects.Add(new Models.Project { Id = projectId, Code = "proj", Name = "Project", SolutionId = solutionId });
                db.Components.Add(new Models.Component { Id = componentId, Name = "Component", Code = "comp", ProjectId = projectId });
                await db.SaveChangesAsync();
                await db.Database.CloseConnectionAsync(); // Ensure commit and visibility
            }

            var uploadUrl = $"/api/ResourceFiles/upload?componentId={componentId}&language=fr";
            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonContent))), "file", resourceFileName);
            var uploadResp = await _fixture.Client.PostAsync(uploadUrl, content);
            if (!uploadResp.IsSuccessStatusCode)
            {
                var errorContent = await uploadResp.Content.ReadAsStringAsync();
                throw new Exception($"Upload failed: {(int)uploadResp.StatusCode} {uploadResp.ReasonPhrase}\nBody:\n{errorContent}");
            }
            var uploaded = await uploadResp.Content.ReadFromJsonAsync<Models.ResourceFile>();
            Assert.IsNotNull(uploaded);

            // Act: download as .json, language=fr
            var downloadResp = await _fixture.Client.GetAsync($"/api/ResourceFiles/download/{uploaded.Id}?format=.json&language=fr");
            if (!downloadResp.IsSuccessStatusCode)
            {
                var errorContent = await downloadResp.Content.ReadAsStringAsync();
                throw new Exception($"Download failed: {(int)downloadResp.StatusCode} {downloadResp.ReasonPhrase}\nBody:\n{errorContent}");
            }
            var downloaded = await downloadResp.Content.ReadAsStringAsync();

            // Assert: only French values present (or fallback)
            Assert.IsTrue(downloaded.Contains("\"Hello\": \"Bonjour\"") || downloaded.Contains("\"Hello\": \"##Hello in fr##\""));
            Assert.IsTrue(downloaded.Contains("\"World\": \"Monde\"") || downloaded.Contains("\"World\": \"##World in fr##\""));
            Assert.IsFalse(downloaded.Contains("\"Hello\": \"Hello\"") && !downloaded.Contains("\"Hello\": \"##Hello in fr##\""));
            Assert.IsFalse(downloaded.Contains("\"World\": \"World\"") && !downloaded.Contains("\"World\": \"##World in fr##\""));
        }

        [TestMethod]
        public async Task Download_Export_Unsupported_Format_Returns_Error()
        {
            // Arrange: create a solution, project, component, and upload a .json file
            var solutionId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var componentId = Guid.NewGuid();
            var resourceFileName = "test.json";
            var jsonContent = "{\"Hello\":\"Bonjour\"}";

            // Respect entity dependencies: solution > project > component
            using (var scope = _fixture.Factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<UseTheOps.PolyglotInitiative.Data.PolyglotInitiativeDbContext>();
                var userId = Guid.NewGuid();
                db.Users.Add(new Models.User { Id = userId, Name = "Test Owner", Email = "owner@example.com", PasswordHash = "", IsAdministrator = false, Status = "active" });
                db.Solutions.Add(new Models.Solution { Id = solutionId, Code = "sol", Name = "Solution", OwnerId = userId });
                db.Projects.Add(new Models.Project { Id = projectId, Code = "proj", Name = "Project", SolutionId = solutionId });
                db.Components.Add(new Models.Component { Id = componentId, Name = "Component", Code = "comp", ProjectId = projectId });
                await db.SaveChangesAsync();
                await db.Database.CloseConnectionAsync(); // Ensure commit and visibility
            }

            var uploadUrl = $"/api/ResourceFiles/upload?componentId={componentId}&language=fr";
            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonContent))), "file", resourceFileName);
            var uploadResp = await _fixture.Client.PostAsync(uploadUrl, content);
            if (!uploadResp.IsSuccessStatusCode)
            {
                var errorContent = await uploadResp.Content.ReadAsStringAsync();
                throw new Exception($"Upload failed: {(int)uploadResp.StatusCode} {uploadResp.ReasonPhrase}\nBody:\n{errorContent}");
            }
            var uploaded = await uploadResp.Content.ReadFromJsonAsync<Models.ResourceFile>();
            Assert.IsNotNull(uploaded);

            // Act: download with unsupported format
            var downloadResp = await _fixture.Client.GetAsync($"/api/ResourceFiles/download/{uploaded.Id}?format=.unsupported&language=fr");
            if (downloadResp.IsSuccessStatusCode)
            {
                throw new Exception($"Download with unsupported format should have failed but succeeded. Status: {(int)downloadResp.StatusCode} {downloadResp.ReasonPhrase}");
            }
            var errorContent2 = await downloadResp.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(errorContent2))
            {
                throw new Exception($"Download with unsupported format failed but no error body returned.");
            }
        }

        [TestMethod]
        public async Task Download_Export_Content_Correctness()
        {
            // Arrange: create a solution, project, component, and upload a .json file with known values
            var solutionId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var componentId = Guid.NewGuid();
            var resourceFileName = "test.json";
            var jsonContent = "{\"Hello\":\"Bonjour\",\"World\":\"Monde\"}";

            // Respect entity dependencies: solution > project > component
            using (var scope = _fixture.Factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<UseTheOps.PolyglotInitiative.Data.PolyglotInitiativeDbContext>();
                var userId = Guid.NewGuid();
                db.Users.Add(new Models.User { Id = userId, Name = "Test Owner", Email = "owner@example.com", PasswordHash = "", IsAdministrator = false, Status = "active" });
                db.Solutions.Add(new Models.Solution { Id = solutionId, Code = "sol", Name = "Solution", OwnerId = userId });
                db.Projects.Add(new Models.Project { Id = projectId, Code = "proj", Name = "Project", SolutionId = solutionId });
                db.Components.Add(new Models.Component { Id = componentId, Name = "Component", Code = "comp", ProjectId = projectId });
                await db.SaveChangesAsync();
                await db.Database.CloseConnectionAsync(); // Ensure commit and visibility
            }

            var uploadUrl = $"/api/ResourceFiles/upload?componentId={componentId}&language=fr";
            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonContent))), "file", resourceFileName);
            var uploadResp = await _fixture.Client.PostAsync(uploadUrl, content);
            if (!uploadResp.IsSuccessStatusCode)
            {
                var errorContent = await uploadResp.Content.ReadAsStringAsync();
                throw new Exception($"Upload failed: {(int)uploadResp.StatusCode} {uploadResp.ReasonPhrase}\nBody:\n{errorContent}");
            }
            var uploaded = await uploadResp.Content.ReadFromJsonAsync<Models.ResourceFile>();
            Assert.IsNotNull(uploaded);

            // Act: download as .json
            var downloadResp = await _fixture.Client.GetAsync($"/api/ResourceFiles/download/{uploaded.Id}?format=.json&language=fr");
            if (!downloadResp.IsSuccessStatusCode)
            {
                var errorContent = await downloadResp.Content.ReadAsStringAsync();
                throw new Exception($"Download failed: {(int)downloadResp.StatusCode} {downloadResp.ReasonPhrase}\nBody:\n{errorContent}");
            }
            var downloaded = await downloadResp.Content.ReadAsStringAsync();

            // Assert: check that the content matches the known values (or fallback)
            Assert.IsTrue(downloaded.Contains("\"Hello\": \"Bonjour\"") || downloaded.Contains("\"Hello\": \"##Hello in fr##\""));
            Assert.IsTrue(downloaded.Contains("\"World\": \"Monde\"") || downloaded.Contains("\"World\": \"##World in fr##\""));
            Assert.IsFalse(downloaded.Contains("\"Hello\": \"Hello\"") && !downloaded.Contains("\"Hello\": \"##Hello in fr##\""));
            Assert.IsFalse(downloaded.Contains("\"World\": \"World\"") && !downloaded.Contains("\"World\": \"##World in fr##\""));
        }
    }
}
