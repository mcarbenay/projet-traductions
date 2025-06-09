using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Linq;

namespace UseTheOps.PolyglotInitiative.Services
{
    /// <summary>
    /// Parser for .xliff translation files.
    /// </summary>
    public class XliffTranslationFileParser : ITranslationFileParser
    {
        public bool CanParse(string fileExtension) => fileExtension.ToLowerInvariant() == ".xliff";

        public async Task<TranslationFileParseResult> ParseAsync(Stream stream)
        {
            var result = new TranslationFileParseResult();
            XDocument doc = XDocument.Load(stream);
            var fileElements = doc.Descendants().Where(e => e.Name.LocalName == "file");
            foreach (var fileEl in fileElements)
            {
                var lang = fileEl.Attribute("target-language")?.Value ?? fileEl.Attribute("source-language")?.Value;
                if (!string.IsNullOrEmpty(lang) && !result.Languages.Contains(lang))
                    result.Languages.Add(lang);
                var transUnits = fileEl.Descendants().Where(e => e.Name.LocalName == "trans-unit");
                foreach (var tu in transUnits)
                {
                    var key = tu.Attribute("id")?.Value;
                    var value = tu.Descendants().FirstOrDefault(e => e.Name.LocalName == "target")?.Value
                        ?? tu.Descendants().FirstOrDefault(e => e.Name.LocalName == "source")?.Value;
                    if (!string.IsNullOrEmpty(key))
                        result.Entries.Add(new TranslationEntry { Key = key!, Value = value ?? string.Empty, Language = lang });
                }
            }
            return result;
        }
    }
}
