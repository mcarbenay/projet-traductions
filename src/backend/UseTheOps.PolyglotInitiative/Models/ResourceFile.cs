using System;
using System.Collections.Generic;

namespace UseTheOps.PolyglotInitiative.Models
{
    public class ResourceFile
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public Guid ComponentId { get; set; }
        public Component Component { get; set; } = null!;
        public ICollection<TranslatableResource> TranslatableResources { get; set; } = new List<TranslatableResource>();
    }
}
