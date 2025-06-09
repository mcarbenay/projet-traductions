using System;

namespace UseTheOps.PolyglotInitiative.Models
{
    /// <summary>
    /// Represents a translation of a resource in a specific language.
    /// </summary>
    public class ResourceTranslation
    {
        /// <summary>
        /// Gets or sets the unique identifier for the translation resource.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the translatable resource.
        /// </summary>
        public Guid TranslatableResourceId { get; set; }

        /// <summary>
        /// Gets or sets the translatable resource associated with this translation.
        /// </summary>
        public TranslatableResource TranslatableResource { get; set; } = null!;

        /// <summary>
        /// Gets or sets the unique identifier for the translation need.
        /// </summary>
        public Guid TranslationNeedId { get; set; }

        /// <summary>
        /// Gets or sets the translation need associated with this translation.
        /// </summary>
        public TranslationNeed TranslationNeed { get; set; } = null!;

        /// <summary>
        /// Gets or sets the validated value of the translation.
        /// </summary>
        public string? ValidatedValue { get; set; }

        /// <summary>
        /// Gets or sets the suggested value for the translation.
        /// </summary>
        public string? SuggestedValue { get; set; }

        /// <summary>
        /// Gets or sets the status of the translation.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the last modified date of the translation.
        /// </summary>
        public DateTime LastModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the user who last modified the translation.
        /// </summary>
        public Guid? ModifiedById { get; set; }

        /// <summary>
        /// Gets or sets the user who last modified the translation.
        /// </summary>
        public User? ModifiedBy { get; set; }
    }
}
