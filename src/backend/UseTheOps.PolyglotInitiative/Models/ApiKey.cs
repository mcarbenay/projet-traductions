using System;

namespace UseTheOps.PolyglotInitiative.Models
{
    public class ApiKey
    {
        public Guid Id { get; set; }
        public Guid SolutionId { get; set; }
        public Solution Solution { get; set; } = null!;
        public string KeyValue { get; set; } = string.Empty; // Stocké de façon sécurisée
        public string Scope { get; set; } = string.Empty; // Optionnel, pour intégration future
        public DateTime CreatedAt { get; set; }
        public DateTime? RevokedAt { get; set; }
    }
}
