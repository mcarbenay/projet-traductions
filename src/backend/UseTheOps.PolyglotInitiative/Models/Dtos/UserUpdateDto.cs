using System;
using System.Collections.Generic;

namespace UseTheOps.PolyglotInitiative.Models.Dtos
{
    /// <summary>
    /// DTO for updating an existing user.
    /// </summary>
    public class UserUpdateDto
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
        public List<UseTheOps.PolyglotInitiative.Models.Dtos.UserSolutionAccessDto> SolutionAccesses { get; set; } = new();
    }
}
