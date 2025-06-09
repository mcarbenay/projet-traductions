using System;
using System.Collections.Generic;

namespace UseTheOps.PolyglotInitiative.Models
{
    /// <summary>
    /// Represents an application user.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email of the user.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password hash for the user.
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the user is an administrator.
        /// </summary>
        public bool IsAdministrator { get; set; }

        /// <summary>
        /// Gets or sets the status of the user.
        /// </summary>
        public string Status { get; set; } = "pending";

        /// <summary>
        /// Gets or sets the solutions owned by the user.
        /// </summary>
        public ICollection<Solution> OwnedSolutions { get; set; } = new List<Solution>();

        /// <summary>
        /// Gets or sets the solution accesses for the user.
        /// </summary>
        public ICollection<UserSolutionAccess> UserSolutionAccesses { get; set; } = new List<UserSolutionAccess>();

        /// <summary>
        /// Gets or sets the translations modified by the user.
        /// </summary>
        public ICollection<ResourceTranslation> ModifiedTranslations { get; set; } = new List<ResourceTranslation>();

        /// <summary>
        /// Gets or sets the external identifiers modified by the user.
        /// </summary>
        public ICollection<ExternalIdentifier> ModifiedExternalIdentifiers { get; set; } = new List<ExternalIdentifier>();
    }
}
