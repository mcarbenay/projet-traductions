using System;
using System.Collections.Generic;

namespace UseTheOps.PolyglotInitiative.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsAdministrator { get; set; }
        public string Status { get; set; } = "pending";
        public ICollection<Solution> OwnedSolutions { get; set; } = new List<Solution>();
        public ICollection<UserSolutionAccess> UserSolutionAccesses { get; set; } = new List<UserSolutionAccess>();
        public ICollection<ResourceTranslation> ModifiedTranslations { get; set; } = new List<ResourceTranslation>();
        public ICollection<ExternalIdentifier> ModifiedExternalIdentifiers { get; set; } = new List<ExternalIdentifier>();
    }
}
