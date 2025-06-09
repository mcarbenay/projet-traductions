using System;
using System.Collections.Generic;
using System.IO;
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
            // Truncate all tables for a clean state
            using var scope = _fixture.Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<UseTheOps.PolyglotInitiative.Data.PolyglotInitiativeDbContext>();
            await db.Database.OpenConnectionAsync();
            var tables = new[] {
                "ResourceTranslations", "TranslatableResources", "ResourceFiles", "Components", "Projects", "Solutions", "TranslationNeeds", "Users", "UserSolutionAccesses", "ApiKeys", "ExternalIdentifiers"
            };
            foreach (var table in tables)
            {
                await db.Database.ExecuteSqlRawAsync($"TRUNCATE \"{table}\" RESTART IDENTITY CASCADE;");
            }
            await db.Database.CloseConnectionAsync();
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

            // Insert solution, project, component directly in DB
            using (var scope = _fixture.Factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<UseTheOps.PolyglotInitiative.Data.PolyglotInitiativeDbContext>();
                db.Solutions.Add(new Models.Solution { Id = solutionId, Code = "sol", Name = "Solution", OwnerId = Guid.NewGuid() });
                db.Projects.Add(new Models.Project { Id = projectId, Code = "proj", Name = "Project", SolutionId = solutionId });
                db.Components.Add(new Models.Component { Id = componentId, Name = "Component", Code = "comp", ProjectId = projectId });
                await db.SaveChangesAsync();
            }

            // Upload .resx file
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(componentId.ToString()), "componentId");
            content.Add(new StringContent("en"), "language");
            content.Add(new StreamContent(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(resxContent))), "file", resourceFileName);
            var uploadResp = await _fixture.Client.PostAsync("/api/ResourceFiles/upload", content);
            uploadResp.EnsureSuccessStatusCode();
            var uploaded = await uploadResp.Content.ReadFromJsonAsync<Models.ResourceFile>();
            Assert.IsNotNull(uploaded);

            // Act: download as .resx
            var downloadResp = await _fixture.Client.GetAsync($"/api/ResourceFiles/download/{uploaded.Id}?format=.resx&language=en");
            downloadResp.EnsureSuccessStatusCode();
            var downloaded = await downloadResp.Content.ReadAsStringAsync();

            // Assert: check XML contains expected keys/values
            Assert.IsTrue(downloaded.Contains("<data name=\"Hello\">"));
            Assert.IsTrue(downloaded.Contains("<value>Hello</value>"));
            Assert.IsTrue(downloaded.Contains("<data name=\"World\">"));
            Assert.IsTrue(downloaded.Contains("<value>World</value>"));
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

            using (var scope = _fixture.Factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<UseTheOps.PolyglotInitiative.Data.PolyglotInitiativeDbContext>();
                db.Solutions.Add(new Models.Solution { Id = solutionId, Code = "sol", Name = "Solution", OwnerId = Guid.NewGuid() });
                db.Projects.Add(new Models.Project { Id = projectId, Code = "proj", Name = "Project", SolutionId = solutionId });
                db.Components.Add(new Models.Component { Id = componentId, Name = "Component", Code = "comp", ProjectId = projectId });
                await db.SaveChangesAsync();
            }

            var content = new MultipartFormDataContent();
            content.Add(new StringContent(componentId.ToString()), "componentId");
            content.Add(new StringContent("fr"), "language");
            content.Add(new StreamContent(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonContent))), "file", resourceFileName);
            var uploadResp = await _fixture.Client.PostAsync("/api/ResourceFiles/upload", content);
            uploadResp.EnsureSuccessStatusCode();
            var uploaded = await uploadResp.Content.ReadFromJsonAsync<Models.ResourceFile>();
            Assert.IsNotNull(uploaded);

            // Act: download as .json
            var downloadResp = await _fixture.Client.GetAsync($"/api/ResourceFiles/download/{uploaded.Id}?format=.json&language=fr");
            downloadResp.EnsureSuccessStatusCode();
            var downloaded = await downloadResp.Content.ReadAsStringAsync();

            // Assert: check JSON contains expected keys/values
            Assert.IsTrue(downloaded.Contains("\"Hello\": \"Bonjour\""));
            Assert.IsTrue(downloaded.Contains("\"World\": \"Monde\""));
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

            using (var scope = _fixture.Factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<UseTheOps.PolyglotInitiative.Data.PolyglotInitiativeDbContext>();
                db.Solutions.Add(new Models.Solution { Id = solutionId, Code = "sol", Name = "Solution", OwnerId = Guid.NewGuid() });
                db.Projects.Add(new Models.Project { Id = projectId, Code = "proj", Name = "Project", SolutionId = solutionId });
                db.Components.Add(new Models.Component { Id = componentId, Name = "Component", Code = "comp", ProjectId = projectId });
                await db.SaveChangesAsync();
            }

            var content = new MultipartFormDataContent();
            content.Add(new StringContent(componentId.ToString()), "componentId");
            content.Add(new StringContent("de"), "language");
            content.Add(new StreamContent(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(poContent))), "file", resourceFileName);
            var uploadResp = await _fixture.Client.PostAsync("/api/ResourceFiles/upload", content);
            uploadResp.EnsureSuccessStatusCode();
            var uploaded = await uploadResp.Content.ReadFromJsonAsync<Models.ResourceFile>();
            Assert.IsNotNull(uploaded);

            // Act: download as .po
            var downloadResp = await _fixture.Client.GetAsync($"/api/ResourceFiles/download/{uploaded.Id}?format=.po&language=de");
            downloadResp.EnsureSuccessStatusCode();
            var downloaded = await downloadResp.Content.ReadAsStringAsync();

            // Assert: check PO contains expected keys/values
            Assert.IsTrue(downloaded.Contains("msgid \"Hello\""));
            Assert.IsTrue(downloaded.Contains("msgstr \"Hallo\""));
            Assert.IsTrue(downloaded.Contains("msgid \"World\""));
            Assert.IsTrue(downloaded.Contains("msgstr \"Welt\""));
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

            using (var scope = _fixture.Factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<UseTheOps.PolyglotInitiative.Data.PolyglotInitiativeDbContext>();
                db.Solutions.Add(new Models.Solution { Id = solutionId, Code = "sol", Name = "Solution", OwnerId = Guid.NewGuid() });
                db.Projects.Add(new Models.Project { Id = projectId, Code = "proj", Name = "Project", SolutionId = solutionId });
                db.Components.Add(new Models.Component { Id = componentId, Name = "Component", Code = "comp", ProjectId = projectId });
                await db.SaveChangesAsync();
            }

            var content = new MultipartFormDataContent();
            content.Add(new StringContent(componentId.ToString()), "componentId");
            content.Add(new StringContent("es"), "language");
            content.Add(new StreamContent(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xliffContent))), "file", resourceFileName);
            var uploadResp = await _fixture.Client.PostAsync("/api/ResourceFiles/upload", content);
            uploadResp.EnsureSuccessStatusCode();
            var uploaded = await uploadResp.Content.ReadFromJsonAsync<Models.ResourceFile>();
            Assert.IsNotNull(uploaded);

            // Act: download as .xliff
            var downloadResp = await _fixture.Client.GetAsync($"/api/ResourceFiles/download/{uploaded.Id}?format=.xliff&language=es");
            downloadResp.EnsureSuccessStatusCode();
            var downloaded = await downloadResp.Content.ReadAsStringAsync();

            // Assert: check XLIFF contains expected keys/values
            Assert.IsTrue(downloaded.Contains("<trans-unit id=\"Hello\">"));
            Assert.IsTrue(downloaded.Contains("<target>Hola</target>"));
            Assert.IsTrue(downloaded.Contains("<trans-unit id=\"World\">"));
            Assert.IsTrue(downloaded.Contains("<target>Mundo</target>"));
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

            using (var scope = _fixture.Factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<UseTheOps.PolyglotInitiative.Data.PolyglotInitiativeDbContext>();
                db.Solutions.Add(new Models.Solution { Id = solutionId, Code = "sol", Name = "Solution", OwnerId = Guid.NewGuid() });
                db.Projects.Add(new Models.Project { Id = projectId, Code = "proj", Name = "Project", SolutionId = solutionId });
                db.Components.Add(new Models.Component { Id = componentId, Name = "Component", Code = "comp", ProjectId = projectId });
                await db.SaveChangesAsync();
            }

            var content = new MultipartFormDataContent();
            content.Add(new StringContent(componentId.ToString()), "componentId");
            content.Add(new StringContent("fr"), "language");
            content.Add(new StreamContent(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonContent))), "file", resourceFileName);
            var uploadResp = await _fixture.Client.PostAsync("/api/ResourceFiles/upload", content);
            uploadResp.EnsureSuccessStatusCode();
            var uploaded = await uploadResp.Content.ReadFromJsonAsync<Models.ResourceFile>();
            Assert.IsNotNull(uploaded);

            // Act: download as .json, language=fr
            var downloadResp = await _fixture.Client.GetAsync($"/api/ResourceFiles/download/{uploaded.Id}?format=.json&language=fr");
            downloadResp.EnsureSuccessStatusCode();
            var downloaded = await downloadResp.Content.ReadAsStringAsync();

            // Assert: only French values present
            Assert.IsTrue(downloaded.Contains("\"Hello\": \"Bonjour\""));
            Assert.IsTrue(downloaded.Contains("\"World\": \"Monde\""));
            Assert.IsFalse(downloaded.Contains("\"Hello\": \"Hello\""));
            Assert.IsFalse(downloaded.Contains("\"World\": \"World\""));
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

            using (var scope = _fixture.Factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<UseTheOps.PolyglotInitiative.Data.PolyglotInitiativeDbContext>();
                db.Solutions.Add(new Models.Solution { Id = solutionId, Code = "sol", Name = "Solution", OwnerId = Guid.NewGuid() });
                db.Projects.Add(new Models.Project { Id = projectId, Code = "proj", Name = "Project", SolutionId = solutionId });
                db.Components.Add(new Models.Component { Id = componentId, Name = "Component", Code = "comp", ProjectId = projectId });
                await db.SaveChangesAsync();
            }

            var content = new MultipartFormDataContent();
            content.Add(new StringContent(componentId.ToString()), "componentId");
            content.Add(new StringContent("fr"), "language");
            content.Add(new StreamContent(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonContent))), "file", resourceFileName);
            var uploadResp = await _fixture.Client.PostAsync("/api/ResourceFiles/upload", content);
            uploadResp.EnsureSuccessStatusCode();
            var uploaded = await uploadResp.Content.ReadFromJsonAsync<Models.ResourceFile>();
            Assert.IsNotNull(uploaded);

            // Act: download with unsupported format
            var downloadResp = await _fixture.Client.GetAsync($"/api/ResourceFiles/download/{uploaded.Id}?format=.unsupported&language=fr");

            // Assert: should return NotFound or BadRequest
            Assert.IsTrue(!downloadResp.IsSuccessStatusCode);
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

            using (var scope = _fixture.Factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<UseTheOps.PolyglotInitiative.Data.PolyglotInitiativeDbContext>();
                db.Solutions.Add(new Models.Solution { Id = solutionId, Code = "sol", Name = "Solution", OwnerId = Guid.NewGuid() });
                db.Projects.Add(new Models.Project { Id = projectId, Code = "proj", Name = "Project", SolutionId = solutionId });
                db.Components.Add(new Models.Component { Id = componentId, Name = "Component", Code = "comp", ProjectId = projectId });
                await db.SaveChangesAsync();
            }

            var content = new MultipartFormDataContent();
            content.Add(new StringContent(componentId.ToString()), "componentId");
            content.Add(new StringContent("fr"), "language");
            content.Add(new StreamContent(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonContent))), "file", resourceFileName);
            var uploadResp = await _fixture.Client.PostAsync("/api/ResourceFiles/upload", content);
            uploadResp.EnsureSuccessStatusCode();
            var uploaded = await uploadResp.Content.ReadFromJsonAsync<Models.ResourceFile>();
            Assert.IsNotNull(uploaded);

            // Act: download as .json
            var downloadResp = await _fixture.Client.GetAsync($"/api/ResourceFiles/download/{uploaded.Id}?format=.json&language=fr");
            downloadResp.EnsureSuccessStatusCode();
            var downloaded = await downloadResp.Content.ReadAsStringAsync();

            // Assert: check that the content matches the known values
            Assert.IsTrue(downloaded.Contains("\"Hello\": \"Bonjour\""));
            Assert.IsTrue(downloaded.Contains("\"World\": \"Monde\""));
            Assert.IsFalse(downloaded.Contains("\"Hello\": \"Hello\""));
            Assert.IsFalse(downloaded.Contains("\"World\": \"World\""));
        }
    }
}
