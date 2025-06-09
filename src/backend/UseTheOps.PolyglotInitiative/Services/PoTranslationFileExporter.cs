using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseTheOps.PolyglotInitiative.Models;

namespace UseTheOps.PolyglotInitiative.Services
{
    public class PoTranslationFileExporter : ITranslationFileExporter
    {
        public string FileExtension => ".po";

        public async Task<byte[]> ExportAsync(
            IEnumerable<TranslatableResource> resources,
            IEnumerable<ResourceTranslation> translations,
            string language)
        {
            var sb = new StringBuilder();
            foreach (var r in resources)
            {
                var t = translations.FirstOrDefault(tr => tr.ResourceKey == r.ResourceKey && tr.Language == language);
                sb.AppendLine($"msgid \"{r.ResourceKey}\"");
                sb.AppendLine($"msgstr \"{(t?.Value ?? string.Empty).Replace("\"", "\\\"")}\"");
                sb.AppendLine();
            }
            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
