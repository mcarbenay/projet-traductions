using System;
using System.Collections.Generic;

namespace UseTheOps.PolyglotInitiative.Models
{
    public class Solution
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? PresentationUrl { get; set; }
        public Guid OwnerId { get; set; }
        public User Owner { get; set; } = null!;
        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<TranslationNeed> TranslationNeeds { get; set; } = new List<TranslationNeed>();
    }
}
