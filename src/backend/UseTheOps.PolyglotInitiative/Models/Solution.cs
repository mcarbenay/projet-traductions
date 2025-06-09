using System;
using System.Collections.Generic;

namespace UseTheOps.PolyglotInitiative.Models
{
    /// <summary>
    /// Represents a solution grouping multiple projects.
    /// </summary>
    public class Solution
    {
        /// <summary>
        /// Gets or sets the unique identifier of the solution.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the code of the solution.
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the solution.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the solution.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the presentation URL of the solution.
        /// </summary>
        public string? PresentationUrl { get; set; }

        /// <summary>
        /// Gets or sets the owner identifier of the solution.
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the owner of the solution.
        /// </summary>
        public User Owner { get; set; } = null!;

        /// <summary>
        /// Gets or sets the projects associated with the solution.
        /// </summary>
        public ICollection<Project> Projects { get; set; } = new List<Project>();

        /// <summary>
        /// Gets or sets the translation needs of the solution.
        /// </summary>
        public ICollection<TranslationNeed> TranslationNeeds { get; set; } = new List<TranslationNeed>();
    }
}
