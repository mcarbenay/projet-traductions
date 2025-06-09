using System;

namespace UseTheOps.PolyglotInitiative.Models.Dtos
{
    /// <summary>
    /// DTO for updating an existing project.
    /// </summary>
    public class ProjectUpdateDto
    {
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
        /// Gets or sets the external identifier ID of the project.
        /// </summary>
        public Guid? ExternalIdentifierId { get; set; }

        /// <summary>
        /// Gets or sets the solution ID associated with the project.
        /// </summary>
        public Guid SolutionId { get; set; }
    }
}
