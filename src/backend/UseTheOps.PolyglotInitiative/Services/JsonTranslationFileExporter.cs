using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using UseTheOps.PolyglotInitiative.Models;

namespace UseTheOps.PolyglotInitiative.Services
{
    public class JsonTranslationFileExporter : ITranslationFileExporter
    {
        public string FileExtension => ".json";

        public async Task<byte[]> ExportAsync(
            IEnumerable<TranslatableResource> resources,
            IEnumerable<ResourceTranslation> translations,
            string language)
        {
            var dict = new Dictionary<string, string>();
            foreach (var r in resources)
            {
                var t = translations.FirstOrDefault(tr => tr.TranslatableResourceId == r.Id && tr.TranslationNeed != null && tr.TranslationNeed.Code == language);
                dict[r.Key] = t?.ValidatedValue ?? $"##{r.Key} in {language}##";
            }
            using var ms = new MemoryStream();
            await JsonSerializer.SerializeAsync(ms, dict, new JsonSerializerOptions { WriteIndented = true });
            return ms.ToArray();
        }
    }
}
