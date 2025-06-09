using System;
using System.Collections.Generic;

namespace UseTheOps.PolyglotInitiative.Models.Dtos
{
    /// <summary>
    /// DTO for creating a new user.
    /// </summary>
    public class UserCreateDto
    {
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email of the user.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the user is an administrator.
        /// </summary>
        public bool IsAdministrator { get; set; }

        /// <summary>
        /// Gets or sets the solution accesses for the user.
        /// </summary>
        public List<UserSolutionAccessDto> SolutionAccesses { get; set; } = new();
    }

    /// <summary>
    /// DTO for user solution access.
    /// </summary>
    public class UserSolutionAccessDto
    {
        /// <summary>
        /// Gets or sets the solution identifier.
        /// </summary>
        public Guid SolutionId { get; set; }

        /// <summary>
        /// Gets or sets the access level for the solution.
        /// </summary>
        public string AccessLevel { get; set; } = string.Empty; // product_owner, translator, reader
    }
}
