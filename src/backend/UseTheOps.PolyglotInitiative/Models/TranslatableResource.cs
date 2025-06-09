using System;
using System.Collections.Generic;

namespace UseTheOps.PolyglotInitiative.Models
{
    public class TranslatableResource
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string SourceValue { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid ResourceFileId { get; set; }
        public ResourceFile ResourceFile { get; set; } = null!;
        public ICollection<ResourceTranslation> ResourceTranslations { get; set; } = new List<ResourceTranslation>();
    }
}
