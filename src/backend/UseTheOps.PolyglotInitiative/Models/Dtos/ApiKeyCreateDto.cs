using System;

namespace UseTheOps.PolyglotInitiative.Models.Dtos
{
    /// <summary>
    /// DTO for creating a new API key.
    /// </summary>
    public class ApiKeyCreateDto
    {
        /// <summary>
        /// Gets or sets the solution identifier.
        /// </summary>
        public Guid SolutionId { get; set; }

        /// <summary>
        /// Gets or sets the scope of the API key.
        /// </summary>
        public string Scope { get; set; } = string.Empty;
    }
}
