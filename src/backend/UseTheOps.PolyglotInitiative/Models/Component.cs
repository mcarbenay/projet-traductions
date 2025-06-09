using System;
using System.Collections.Generic;

namespace UseTheOps.PolyglotInitiative.Models
{
    public class Component
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public ICollection<ResourceFile> ResourceFiles { get; set; } = new List<ResourceFile>();
    }
}
