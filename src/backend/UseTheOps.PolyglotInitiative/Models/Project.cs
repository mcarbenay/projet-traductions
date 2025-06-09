using System;
using System.Collections.Generic;

namespace UseTheOps.PolyglotInitiative.Models
{
    /// <summary>
    /// Represents a project in the application.
    /// </summary>
    public class Project
    {
        /// <summary>
        /// Gets or sets the unique identifier of the project.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the code of the project.
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the project.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the origin of the project.
        /// </summary>
        public string? Origin { get; set; }

        /// <summary>
        /// Gets or sets the origin URL of the project.
        /// </summary>
        public string? OriginUrl { get; set; }

        /// <summary>
        /// Gets or sets the external identifier ID associated with the project.
        /// </summary>
        public Guid? ExternalIdentifierId { get; set; }

        /// <summary>
        /// Gets or sets the external identifier associated with the project.
        /// </summary>
        public ExternalIdentifier? ExternalIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the solution ID that the project is associated with.
        /// </summary>
        public Guid SolutionId { get; set; }

        /// <summary>
        /// Gets or sets the solution that the project is associated with.
        /// </summary>
        public Solution Solution { get; set; } = null!;

        /// <summary>
        /// Gets or sets the collection of components associated with the project.
        /// </summary>
        public ICollection<Component> Components { get; set; } = new List<Component>();
    }
}
