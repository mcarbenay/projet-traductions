using System;

namespace UseTheOps.PolyglotInitiative.Models
{
    /// <summary>
    /// Represents an API key for programmatic access.
    /// </summary>
    public class ApiKey
    {
        /// <summary>
        /// Gets or sets the unique identifier for the API key.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the solution associated with the API key.
        /// </summary>
        public Guid SolutionId { get; set; }

        /// <summary>
        /// Gets or sets the solution associated with the API key.
        /// </summary>
        public Solution Solution { get; set; } = null!;

        /// <summary>
        /// Gets or sets the value of the API key. Stored securely.
        /// </summary>
        public string KeyValue { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the scope of the API key. Optional, for future integration.
        /// </summary>
        public string Scope { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the API key was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the API key was revoked. Nullable if the key is active.
        /// </summary>
        public DateTime? RevokedAt { get; set; }
    }
}
