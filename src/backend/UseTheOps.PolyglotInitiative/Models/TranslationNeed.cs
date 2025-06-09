using System;
using System.Collections.Generic;

namespace UseTheOps.PolyglotInitiative.Models
{
    public class TranslationNeed
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public Guid SolutionId { get; set; }
        public Solution Solution { get; set; } = null!;
        public ICollection<ResourceTranslation> ResourceTranslations { get; set; } = new List<ResourceTranslation>();
    }
}
