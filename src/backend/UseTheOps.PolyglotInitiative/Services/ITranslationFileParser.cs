using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace UseTheOps.PolyglotInitiative.Services
{
    /// <summary>
    /// Interface for translation file parsers (resx, json, po, xliff, etc.).
    /// </summary>
    public interface ITranslationFileParser
    {
        /// <summary>
        /// Parses a translation file stream and extracts translation entries and language codes.
        /// </summary>
        /// <param name="stream">The file stream (read-only, not owned).</param>
        /// <returns>A result containing translation needs (languages), keys, and values.</returns>
        Task<TranslationFileParseResult> ParseAsync(Stream stream);
        /// <summary>
        /// Returns true if the parser supports the given file extension (e.g. ".resx").
        /// </summary>
        bool CanParse(string fileExtension);
    }

    public class TranslationFileParseResult
    {
        public List<string> Languages { get; set; } = new();
        public List<TranslationEntry> Entries { get; set; } = new();
    }

    public class TranslationEntry
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string? Language { get; set; }
    }
}
