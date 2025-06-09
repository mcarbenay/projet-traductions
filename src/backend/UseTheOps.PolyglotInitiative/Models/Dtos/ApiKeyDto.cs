using System;

namespace UseTheOps.PolyglotInitiative.Models.Dtos
{
    public class ApiKeyDto
    {
        public Guid Id { get; set; }
        public Guid SolutionId { get; set; }
        public string Scope { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? RevokedAt { get; set; }
    }
}
