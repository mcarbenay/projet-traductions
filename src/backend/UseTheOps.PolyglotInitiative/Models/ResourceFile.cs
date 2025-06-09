using System;
using System.Collections.Generic;

namespace UseTheOps.PolyglotInitiative.Models
{
    /// <summary>
    /// Represents a resource file containing translatable content.
    /// </summary>
    public class ResourceFile
    {
        /// <summary>
        /// Gets or sets the unique identifier for the resource file.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the resource file.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the path to the resource file.
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the project identifier that the resource file is associated with.
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Gets or sets the project that the resource file is associated with.
        /// </summary>
        public Project Project { get; set; } = null!;

        /// <summary>
        /// Gets or sets the component identifier that the resource file is associated with.
        /// </summary>
        public Guid ComponentId { get; set; }

        /// <summary>
        /// Gets or sets the component that the resource file is associated with.
        /// </summary>
        public Component Component { get; set; } = null!;

        /// <summary>
        /// Gets or sets the collection of translatable resources in the resource file.
        /// </summary>
        public ICollection<TranslatableResource> TranslatableResources { get; set; } = new List<TranslatableResource>();
    }
}
