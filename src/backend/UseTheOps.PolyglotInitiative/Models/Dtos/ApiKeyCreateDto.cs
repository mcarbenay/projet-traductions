using System;

namespace UseTheOps.PolyglotInitiative.Models.Dtos
{
    public class ApiKeyCreateDto
    {
        public Guid SolutionId { get; set; }
        public string Scope { get; set; } = string.Empty;
    }
}
