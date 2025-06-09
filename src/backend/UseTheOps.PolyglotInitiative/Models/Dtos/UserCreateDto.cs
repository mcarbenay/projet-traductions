using System;
using System.Collections.Generic;

namespace UseTheOps.PolyglotInitiative.Models.Dtos
{
    public class UserCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsAdministrator { get; set; }
        public List<UserSolutionAccessDto> SolutionAccesses { get; set; } = new();
    }

    public class UserSolutionAccessDto
    {
        public Guid SolutionId { get; set; }
        public string AccessLevel { get; set; } = string.Empty; // product_owner, translator, reader
    }
}
