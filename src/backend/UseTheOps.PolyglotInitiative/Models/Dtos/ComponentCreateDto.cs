using System;

namespace UseTheOps.PolyglotInitiative.Models.Dtos
{
    /// <summary>
    /// DTO for creating a new component.
    /// </summary>
    public class ComponentCreateDto
    {
        /// <summary>
        /// Gets or sets the name of the component.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the code of the component.
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the project identifier.
        /// </summary>
        public Guid ProjectId { get; set; }
    }
}
