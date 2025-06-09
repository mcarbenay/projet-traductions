using System;
using System.Collections.Generic;

namespace UseTheOps.PolyglotInitiative.Models
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Origin { get; set; }
        public string? OriginUrl { get; set; }
        public Guid? ExternalIdentifierId { get; set; }
        public ExternalIdentifier? ExternalIdentifier { get; set; }
        public Guid SolutionId { get; set; }
        public Solution Solution { get; set; } = null!;
        public ICollection<Component> Components { get; set; } = new List<Component>();
    }
}
