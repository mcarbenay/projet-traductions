using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UseTheOps.PolyglotInitiative.Models;

namespace UseTheOps.PolyglotInitiative.Services
{
    public class ResxTranslationFileExporter : ITranslationFileExporter
    {
        public string FileExtension => ".resx";

        public async Task<byte[]> ExportAsync(
            IEnumerable<TranslatableResource> resources,
            IEnumerable<ResourceTranslation> translations,
            string language)
        {
            var translationDict = new Dictionary<string, string>();
            foreach (var t in translations)
            {
                if (t.Language == language)
                    translationDict[t.ResourceKey] = t.Value;
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
                            new XAttribute("name", r.ResourceKey),
                            new XAttribute(XNamespace.Xml + "space", "preserve"),
                            new XElement("value", translationDict.TryGetValue(r.ResourceKey, out var v) ? v : "")
                        )
                    )
                )
            );

            using var ms = new MemoryStream();
            using var writer = new StreamWriter(ms, Encoding.UTF8);
            doc.Save(writer);
            await writer.FlushAsync();
            return ms.ToArray();
        }
    }
}
