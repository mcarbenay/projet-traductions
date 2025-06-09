using System.Collections.Generic;
using System.Threading.Tasks;
using UseTheOps.PolyglotInitiative.Models;

namespace UseTheOps.PolyglotInitiative.Services
{
    public interface ITranslationFileExporter
    {
        /// <summary>
        /// Exports the given resources and translations to a file format as a byte array.
        /// </summary>
        /// <param name="resources">The list of translatable resources.</param>
        /// <param name="translations">The list of translations for the resources.</param>
        /// <param name="language">The language code to export.</param>
        /// <returns>Byte array representing the exported file.</returns>
        Task<byte[]> ExportAsync(
            IEnumerable<TranslatableResource> resources,
            IEnumerable<ResourceTranslation> translations,
            string language);

        /// <summary>
        /// The file extension (e.g., ".resx", ".json", ".po", ".xliff") handled by this exporter.
        /// </summary>
        string FileExtension { get; }
    }
}
