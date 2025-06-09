using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using System.Text.RegularExpressions;

namespace UseTheOps.PolyglotInitiative.Services
{
    /// <summary>
    /// Parser for .po translation files (gettext format).
    /// </summary>
    public class PoTranslationFileParser : ITranslationFileParser
    {
        public bool CanParse(string fileExtension) => fileExtension.ToLowerInvariant() == ".po";

        public async Task<TranslationFileParseResult> ParseAsync(Stream stream)
        {
            var result = new TranslationFileParseResult();
            using var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, leaveOpen: true);
            string? line;
            string? msgid = null;
            string? msgstr = null;
            string? lang = null;
            var langRegex = new Regex(@"Language:\s*([\w\-_]+)", RegexOptions.IgnoreCase);
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (line.StartsWith("msgid "))
                    msgid = line[6..].Trim(' ', '"');
                else if (line.StartsWith("msgstr "))
                    msgstr = line[7..].Trim(' ', '"');
                else if (line.StartsWith("Language:"))
                {
                    var m = langRegex.Match(line);
                    if (m.Success) lang = m.Groups[1].Value;
                }
                if (msgid != null && msgstr != null)
                {
                    result.Entries.Add(new TranslationEntry { Key = msgid, Value = msgstr, Language = lang });
                    msgid = msgstr = null;
                }
            }
            if (lang != null) result.Languages.Add(lang);
            return result;
        }
    }
}
