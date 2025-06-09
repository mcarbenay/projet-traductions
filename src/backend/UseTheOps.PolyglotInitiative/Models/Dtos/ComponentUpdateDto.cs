using System;

namespace UseTheOps.PolyglotInitiative.Models.Dtos
{
    public class ComponentUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
    }
}
