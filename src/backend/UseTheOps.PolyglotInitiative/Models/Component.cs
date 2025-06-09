using System;
using System.Collections.Generic;

namespace UseTheOps.PolyglotInitiative.Models
{
    /// <summary>
    /// Represents a component within a project.
    /// </summary>
    public class Component
    {
        /// <summary>
        /// Gets or sets the unique identifier for the component.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the component.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the code associated with the component.
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the identifier of the project that the component belongs to.
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Gets or sets the project that the component is associated with.
        /// </summary>
        public Project Project { get; set; } = null!;

        /// <summary>
        /// Gets or sets the collection of resource files associated with the component.
        /// </summary>
        public ICollection<ResourceFile> ResourceFiles { get; set; } = new List<ResourceFile>();
    }
}
