using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UseTheOps.PolyglotInitiative.Models;

namespace UseTheOps.PolyglotInitiative.Services
{
    /// <summary>
    /// Service for exporting translations to RESX files.
    /// </summary>
    public class ResxTranslationFileExporter : ITranslationFileExporter
    {
        /// <summary>
        /// Gets the file extension for RESX files.
        /// </summary>
        public string FileExtension => ".resx";

        /// <summary>
        /// Exports the translations to a byte array asynchronously.
        /// </summary>
        /// <param name="resources">The translatable resources.</param>
        /// <param name="translations">The translations for the resources.</param>
        /// <param name="language">The language code for the translations.</param>
        /// <returns>A task that represents the asynchronous export operation. The task result contains the byte array of the exported RESX file.</returns>
        public Task<byte[]> ExportAsync(
            IEnumerable<TranslatableResource> resources,
            IEnumerable<ResourceTranslation> translations,
            string language)
        {
            var translationDict = new Dictionary<string, string>();
            foreach (var t in translations)
            {
                if (t.TranslationNeed != null && t.TranslationNeed.Code == language)
                    translationDict[t.TranslatableResource.Key] = t.ValidatedValue ?? $"##{t.TranslatableResource.Key} in {language}##";
            }

            var doc = new XDocument(
                new XElement("root",
                    new XElement("resheader",
                        new XAttribute("name", "resmimetype"),
                        new XElement("value", "text/microsoft-resx")
                    ),
                    new XElement("resheader",
                        new XAttribute("name", "version"),
                        new XElement("value", "2.0")
                    ),
                    new XElement("resheader",
                        new XAttribute("name", "reader"),
                        new XElement("value", "System.Resources.ResXResourceReader, System.Windows.Forms, ...")
                    ),
                    new XElement("resheader",
                        new XAttribute("name", "writer"),
                        new XElement("value", "System.Resources.ResXResourceWriter, System.Windows.Forms, ...")
                    ),
                    resources.Select(r =>
                        new XElement("data",
                            new XAttribute("name", r.Key),
                            new XAttribute(XNamespace.Xml + "space", "preserve"),
                            new XElement("value", translationDict.TryGetValue(r.Key, out var v) ? v : $"##{r.Key} in {language}##")
                        )
                    )
                )
            );
            using var ms = new MemoryStream();
            doc.Save(ms);
            return Task.FromResult(ms.ToArray());
        }
    }
}
