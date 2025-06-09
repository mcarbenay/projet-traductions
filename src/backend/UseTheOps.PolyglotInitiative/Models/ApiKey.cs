using System;

namespace UseTheOps.PolyglotInitiative.Models
{
    public class ApiKey
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public Guid SolutionId { get; set; }
        public Solution Solution { get; set; } = null!;
        public string KeyValue { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? RevokedAt { get; set; }
    }
}
