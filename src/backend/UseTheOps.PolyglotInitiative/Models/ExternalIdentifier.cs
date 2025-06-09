using System;
using System.Collections.Generic;

namespace UseTheOps.PolyglotInitiative.Models
{
    public class ExternalIdentifier
    {
        public Guid Id { get; set; }
        public string SourceType { get; set; } = string.Empty;
        public string FieldName { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public DateTime AddedOrModifiedDate { get; set; }
        public Guid? ModifiedById { get; set; }
        public User? ModifiedBy { get; set; }
        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
