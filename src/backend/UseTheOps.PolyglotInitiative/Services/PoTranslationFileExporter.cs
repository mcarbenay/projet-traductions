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

        public Task<byte[]> ExportAsync(
            IEnumerable<TranslatableResource> resources,
            IEnumerable<ResourceTranslation> translations,
            string language)
        {
            var sb = new StringBuilder();
            foreach (var r in resources)
            {
                var t = translations.FirstOrDefault(tr => tr.TranslatableResourceId == r.Id && tr.TranslationNeed != null && tr.TranslationNeed.Code == language);
                var value = t?.ValidatedValue ?? $"##{r.Key} in {language}##";
                sb.AppendLine($"msgid \"{r.Key}\"");
                sb.AppendLine($"msgstr \"{value.Replace("\"", "\\\"")}\"");
                sb.AppendLine();
            }
            return Task.FromResult(Encoding.UTF8.GetBytes(sb.ToString()));
        }
    }
}
