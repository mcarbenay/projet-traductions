using System;

namespace UseTheOps.PolyglotInitiative.Models.Dtos
{
    /// <summary>
    /// DTO representing an API key for output.
    /// </summary>
    public class ApiKeyDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the API key.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the solution associated with the API key.
        /// </summary>
        public Guid SolutionId { get; set; }

        /// <summary>
        /// Gets or sets the scope of the API key, defining its permissions.
        /// </summary>
        public string Scope { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the API key was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the API key was revoked, if applicable.
        /// </summary>
        public DateTime? RevokedAt { get; set; }
    }
}
