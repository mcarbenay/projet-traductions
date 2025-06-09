using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Linq;

namespace UseTheOps.PolyglotInitiative.Services
{
    /// <summary>
    /// Service for parsing translations from RESX files.
    /// </summary>
    public class ResxTranslationFileParser : ITranslationFileParser
    {
        /// <summary>
        /// Determines if the parser can handle the given file extension.
        /// </summary>
        /// <param name="fileExtension">The file extension to check.</param>
        /// <returns>True if the parser can handle the file extension; otherwise, false.</returns>
        public bool CanParse(string fileExtension) => fileExtension.ToLowerInvariant() == ".resx";

        /// <summary>
        /// Asynchronously parses the translation entries from the given stream.
        /// </summary>
        /// <param name="stream">The stream containing the RESX file data.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the parsed translation entries.</returns>
        public Task<TranslationFileParseResult> ParseAsync(Stream stream)
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
            return Task.FromResult(result);
        }
    }
}
