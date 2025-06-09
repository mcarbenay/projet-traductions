using System;

namespace UseTheOps.PolyglotInitiative.Models
{
    public class UserSolutionAccess
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public Guid SolutionId { get; set; }
        public Solution Solution { get; set; } = null!;
        public string AccessLevel { get; set; } = string.Empty; // product_owner, translator, reader
    }
}
