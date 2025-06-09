using System;
using System.Collections.Generic;

namespace UseTheOps.PolyglotInitiative.Models.Dtos
{
    public class UserUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsAdministrator { get; set; }
        public List<UseTheOps.PolyglotInitiative.Models.Dtos.UserSolutionAccessDto> SolutionAccesses { get; set; } = new();
    }
}
