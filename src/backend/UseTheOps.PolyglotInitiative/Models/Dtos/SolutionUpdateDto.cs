using System;

namespace UseTheOps.PolyglotInitiative.Models.Dtos
{
    public class SolutionUpdateDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? PresentationUrl { get; set; }
        public Guid OwnerId { get; set; }
    }
}
