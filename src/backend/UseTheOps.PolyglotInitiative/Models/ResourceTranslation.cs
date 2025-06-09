using System;

namespace UseTheOps.PolyglotInitiative.Models
{
    public class ResourceTranslation
    {
        public Guid Id { get; set; }
        public Guid TranslatableResourceId { get; set; }
        public TranslatableResource TranslatableResource { get; set; } = null!;
        public Guid TranslationNeedId { get; set; }
        public TranslationNeed TranslationNeed { get; set; } = null!;
        public string? ValidatedValue { get; set; }
        public string? SuggestedValue { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime LastModifiedDate { get; set; }
        public Guid? ModifiedById { get; set; }
        public User? ModifiedBy { get; set; }
    }
}
