using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Fluid;
using Fluid.Values;

namespace UseTheOps.PolyglotInitiative.Helpers
{
    /// <summary>
    /// Helper for rendering Liquid templates from embedded resources.
    /// </summary>
    public static class LiquidTemplateHelper
    {
        /// <summary>
        /// Renders a Liquid template from an embedded resource file with the given model.
        /// </summary>
        /// <typeparam name="T">Type of the model (strongly typed).</typeparam>
        /// <param name="resourcePath">Resource path (ex: "Resources.MailTemplates.UserCreated.fr-FR.liquid").</param>
        /// <param name="model">Model object to pass to the template.</param>
        /// <returns>Rendered HTML string.</returns>
        public static async Task<string> RenderTemplateAsync<T>(string resourcePath, T model)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceFullName = assembly.GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith(resourcePath.Replace('/', '.'), StringComparison.OrdinalIgnoreCase));
            if (resourceFullName == null)
                throw new FileNotFoundException($"Template resource not found: {resourcePath}");
            using var stream = assembly.GetManifestResourceStream(resourceFullName)!;
            using var reader = new StreamReader(stream);
            var templateText = await reader.ReadToEndAsync();
            var parser = new FluidParser();
            if (!parser.TryParse(templateText, out var template, out var errors))
                throw new InvalidOperationException($"Liquid template parse error: {string.Join(", ", errors)}");
            var options = new TemplateOptions();
            options.MemberAccessStrategy.Register<T>();
            var context = new TemplateContext(options);
            context.SetValue("data", model);
            return await template.RenderAsync(context);
        }
    }
}
