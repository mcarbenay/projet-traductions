using System;
using System.Collections.Generic;

namespace UseTheOps.PolyglotInitiative.Models
{
    /// <summary>
    /// Represents a translatable resource (string, label, etc.).
    /// </summary>
    public class TranslatableResource
    {
        /// <summary>
        /// Gets or sets the unique identifier of the translatable resource.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the key of the translatable resource.
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the source value of the translatable resource.
        /// </summary>
        public string SourceValue { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the translatable resource.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the resource file identifier that this translatable resource belongs to.
        /// </summary>
        public Guid ResourceFileId { get; set; }

        /// <summary>
        /// Gets or sets the resource file that this translatable resource belongs to.
        /// </summary>
        public ResourceFile ResourceFile { get; set; } = null!;

        /// <summary>
        /// Gets or sets the collection of translations for this translatable resource.
        /// </summary>
        public ICollection<ResourceTranslation> ResourceTranslations { get; set; } = new List<ResourceTranslation>();
    }
}
