using System;

namespace UseTheOps.PolyglotInitiative.Models
{
    /// <summary>
    /// Represents the access level of a user to a solution.
    /// </summary>
    public class UserSolutionAccess
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the user associated with this access.
        /// </summary>
        public User User { get; set; } = null!;

        /// <summary>
        /// Gets or sets the unique identifier of the solution.
        /// </summary>
        public Guid SolutionId { get; set; }

        /// <summary>
        /// Gets or sets the solution associated with this access.
        /// </summary>
        public Solution Solution { get; set; } = null!;

        /// <summary>
        /// Gets or sets the access level of the user to the solution.
        /// </summary>
        /// <remarks>
        /// Access levels can be product_owner, translator, or reader.
        /// </remarks>
        public string AccessLevel { get; set; } = string.Empty;
    }
}
