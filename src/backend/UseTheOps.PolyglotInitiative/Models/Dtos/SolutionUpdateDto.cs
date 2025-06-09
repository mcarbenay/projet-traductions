using System;

namespace UseTheOps.PolyglotInitiative.Models.Dtos
{
    /// <summary>
    /// DTO for updating an existing solution.
    /// </summary>
    public class SolutionUpdateDto
    {
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
        /// Gets or sets the owner ID of the solution.
        /// </summary>
        public Guid OwnerId { get; set; }
    }
}
