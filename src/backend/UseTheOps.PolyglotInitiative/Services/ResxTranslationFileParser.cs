using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Linq;

namespace UseTheOps.PolyglotInitiative.Services
{
    /// <summary>
    /// Parser for .resx translation files.
    /// </summary>
    public class ResxTranslationFileParser : ITranslationFileParser
    {
        public bool CanParse(string fileExtension) => fileExtension.ToLowerInvariant() == ".resx";

        public async Task<TranslationFileParseResult> ParseAsync(Stream stream)
        {
            var result = new TranslationFileParseResult();
            XDocument doc = XDocument.Load(stream);
            var dataElements = doc.Descendants("data");
            foreach (var el in dataElements)
            {
                var key = el.Attribute("name")?.Value;
                var value = el.Element("value")?.Value;
                if (!string.IsNullOrEmpty(key))
                {
                    result.Entries.Add(new TranslationEntry { Key = key!, Value = value ?? string.Empty });
                }
            }
            // .resx does not encode language, so leave Languages empty
            return result;
        }
    }
}
