using System;

namespace UseTheOps.PolyglotInitiative.Models.Dtos
{
    public class ProjectCreateDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Origin { get; set; }
        public string? OriginUrl { get; set; }
        public Guid? ExternalIdentifierId { get; set; }
        public Guid SolutionId { get; set; }
    }
}
