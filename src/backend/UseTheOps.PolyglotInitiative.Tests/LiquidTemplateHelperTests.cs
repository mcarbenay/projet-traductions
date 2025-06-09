using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UseTheOps.PolyglotInitiative.Helpers;
using System.Reflection;
using System.IO;

namespace UseTheOps.PolyglotInitiative.Tests
{
    [TestClass]
    public class LiquidTemplateHelperTests
    {
        private const string TemplatePath = "Resources/MailTemplates/TestTemplate.liquid";

        [TestMethod]
        public async Task RenderTemplateAsync_RendersCorrectly_WithSimpleModel()
        {
            var model = new { name = "John", link = "https://example.com" };
            var result = await LiquidTemplateHelper.RenderTemplateAsync(TemplatePath, model);
            Assert.IsTrue(result.Contains("Hello John!"));
            Assert.IsTrue(result.Contains("https://example.com"));
        }

        [TestMethod]
        public async Task RenderTemplateAsync_ThrowsIfTemplateNotFound()
        {
            await Assert.ThrowsExceptionAsync<FileNotFoundException>(async () =>
            {
                await LiquidTemplateHelper.RenderTemplateAsync("Resources/NotFound.liquid", new { });
            });
        }
    }
}
