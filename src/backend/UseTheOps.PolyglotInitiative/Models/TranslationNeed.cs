using System;
using System.Collections.Generic;

namespace UseTheOps.PolyglotInitiative.Models
{
    /// <summary>
    /// Represents a translation need (language/variant) for a solution or project.
    /// </summary>
    public class TranslationNeed
    {
        /// <summary>
        /// Gets or sets the unique identifier of the translation need.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the code that identifies the translation need.
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the label for the translation need, describing its purpose.
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this translation need is the default one.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the solution associated with this translation need.
        /// </summary>
        public Guid SolutionId { get; set; }

        /// <summary>
        /// Gets or sets the solution associated with this translation need.
        /// </summary>
        public Solution Solution { get; set; } = null!;

        /// <summary>
        /// Gets or sets the collection of resource translations for this translation need.
        /// </summary>
        public ICollection<ResourceTranslation> ResourceTranslations { get; set; } = new List<ResourceTranslation>();
    }
}
