using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;

namespace UseTheOps.PolyglotInitiative.Services
{
    /// <summary>
    /// Parser for .json translation files (flat key-value, optionally per language).
    /// </summary>
    public class JsonTranslationFileParser : ITranslationFileParser
    {
        public bool CanParse(string fileExtension) => fileExtension.ToLowerInvariant() == ".json";

        public async Task<TranslationFileParseResult> ParseAsync(Stream stream)
        {
            var result = new TranslationFileParseResult();
            var doc = await JsonDocument.ParseAsync(stream);
            if (doc.RootElement.ValueKind == JsonValueKind.Object)
            {
                // Try to detect language structure: { "en": { ... }, "fr": { ... } }
                bool isPerLanguage = doc.RootElement.EnumerateObject().All(p => p.Value.ValueKind == JsonValueKind.Object);
                if (isPerLanguage)
                {
                    foreach (var langProp in doc.RootElement.EnumerateObject())
                    {
                        string lang = langProp.Name;
                        result.Languages.Add(lang);
                        foreach (var kv in langProp.Value.EnumerateObject())
                        {
                            result.Entries.Add(new TranslationEntry { Key = kv.Name, Value = kv.Value.GetString() ?? string.Empty, Language = lang });
                        }
                    }
                }
                else
                {
                    // Flat key-value
                    foreach (var kv in doc.RootElement.EnumerateObject())
                    {
                        result.Entries.Add(new TranslationEntry { Key = kv.Name, Value = kv.Value.GetString() ?? string.Empty });
                    }
                }
            }
            return result;
        }
    }
}
