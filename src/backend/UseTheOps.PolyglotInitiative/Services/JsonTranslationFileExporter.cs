using System.Collections.Generic;
using System.IO;
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
                var t = translations.FirstOrDefault(tr => tr.ResourceKey == r.ResourceKey && tr.Language == language);
                dict[r.ResourceKey] = t?.Value ?? string.Empty;
            }
            using var ms = new MemoryStream();
            await JsonSerializer.SerializeAsync(ms, dict, new JsonSerializerOptions { WriteIndented = true });
            return ms.ToArray();
        }
    }
}
