using System;
using System.Collections.Generic;

namespace UseTheOps.PolyglotInitiative.Models
{
    /// <summary>
    /// Represents an external identifier for integration with other systems.
    /// </summary>
    public class ExternalIdentifier
    {
        /// <summary>
        /// Gets or sets the unique identifier for the external identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the source type of the external identifier (e.g., "CRM", "ERP").
        /// </summary>
        public string SourceType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the field name of the external identifier in the source system.
        /// </summary>
        public string FieldName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the value of the external identifier.
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the external identifier was added or modified.
        /// </summary>
        public DateTime AddedOrModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who last modified the external identifier.
        /// </summary>
        public Guid? ModifiedById { get; set; }

        /// <summary>
        /// Gets or sets the user who last modified the external identifier.
        /// </summary>
        public User? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the collection of projects associated with the external identifier.
        /// </summary>
        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
