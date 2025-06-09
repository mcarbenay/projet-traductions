using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UseTheOps.PolyglotInitiative.Models;

namespace UseTheOps.PolyglotInitiative.Services
{
    public class XliffTranslationFileExporter : ITranslationFileExporter
    {
        public string FileExtension => ".xliff";

        public Task<byte[]> ExportAsync(
            IEnumerable<TranslatableResource> resources,
            IEnumerable<ResourceTranslation> translations,
            string language)
        {
            var xliff = new XDocument(
                new XElement("xliff",
                    new XAttribute("version", "1.2"),
                    new XElement("file",
                        new XAttribute("source-language", language),
                        new XAttribute("datatype", "plaintext"),
                        new XAttribute("original", "file.ext"),
                        new XElement("body",
                            resources.Select(r =>
                                new XElement("trans-unit",
                                    new XAttribute("id", r.Key),
                                    new XElement("source", r.Key),
                                    new XElement("target", translations.FirstOrDefault(tr => tr.TranslatableResourceId == r.Id && tr.TranslationNeed != null && tr.TranslationNeed.Code == language)?.ValidatedValue ?? $"##{r.Key} in {language}##")
                                )
                            )
                        )
                    )
                )
            );
            using var ms = new MemoryStream();
            xliff.Save(ms);
            return Task.FromResult(ms.ToArray());
        }
    }
}
