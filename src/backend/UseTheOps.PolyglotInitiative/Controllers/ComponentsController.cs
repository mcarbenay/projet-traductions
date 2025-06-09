using Microsoft.AspNetCore.Mvc;
using UseTheOps.PolyglotInitiative.Models;
using UseTheOps.PolyglotInitiative.Models.Dtos;
using UseTheOps.PolyglotInitiative.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace UseTheOps.PolyglotInitiative.Controllers
{
    /// <summary>
    /// API endpoints for managing components.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ComponentsController : ControllerBase
    {
        private readonly ComponentService _service;
        private readonly AuthorizationService _authz;
        private readonly ILogger<ComponentsController> _logger;
        public ComponentsController(ComponentService service, AuthorizationService authz, ILogger<ComponentsController> logger)
        {
            _service = service;
            _authz = authz;
            _logger = logger;
        }

        /// <summary>
        /// Get all components.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Component>>> GetAll()
        {
            _logger.LogInformation("Getting all components");
            var components = await _service.GetAllAsync();
            _logger.LogInformation("Retrieved {Count} components", components.Count);
            return Ok(components);
        }

        /// <summary>
        /// Get a component by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Component>> Get(Guid id)
        {
            _logger.LogInformation("Getting component by ID: {Id}", id);
            var component = await _service.GetByIdAsync(id);
            if (component == null) 
            {
                _logger.LogWarning("Component not found: {Id}", id);
                return NotFound();
            }
            _logger.LogInformation("Retrieved component: {Id}", component.Id);
            return Ok(component);
        }

        /// <summary>
        /// Create a new component.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Component>> Create(ComponentCreateDto dto)
        {
            _logger.LogInformation("Creating component: {@Component}", dto);
            if (!await _authz.CanManageProjectAsync(dto.ProjectId))
            {
                _logger.LogWarning("Unauthorized create attempt for project: {ProjectId}", dto.ProjectId);
                return Forbid();
            }
            var component = new Component
            {
                Name = dto.Name,
                Code = dto.Code,
                ProjectId = dto.ProjectId
            };
            var created = await _service.CreateAsync(component);
            _logger.LogInformation("Created component: {Id}", created.Id);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update a component.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ComponentUpdateDto dto)
        {
            _logger.LogInformation("Updating component: {Id}", id);
            if (!await _authz.CanManageComponentAsync(id))
            {
                _logger.LogWarning("Unauthorized update attempt for component: {Id}", id);
                return Forbid();
            }
            var component = new Component
            {
                Id = id,
                Name = dto.Name,
                Code = dto.Code,
                ProjectId = dto.ProjectId
            };
            var success = await _service.UpdateAsync(id, component);
            if (!success) 
            {
                _logger.LogError("Error updating component, not found: {Id}", id);
                return BadRequest();
            }
            _logger.LogInformation("Updated component: {Id}", id);
            return NoContent();
        }

        /// <summary>
        /// Delete a component.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Deleting component: {Id}", id);
            if (!await _authz.CanManageComponentAsync(id))
            {
                _logger.LogWarning("Unauthorized delete attempt for component: {Id}", id);
                return Forbid();
            }
            var success = await _service.DeleteAsync(id);
            if (!success) 
            {
                _logger.LogError("Error deleting component, not found: {Id}", id);
                return NotFound();
            }
            _logger.LogInformation("Deleted component: {Id}", id);
            return NoContent();
        }

        /// <summary>
        /// Get all components for a given project.
        /// </summary>
        [HttpGet("by-project/{projectId}")]
        public async Task<ActionResult<IEnumerable<Component>>> GetByProject(Guid projectId)
        {
            _logger.LogInformation("Getting components by project: {ProjectId}", projectId);
            var components = await _service.GetByProjectAsync(projectId);
            _logger.LogInformation("Retrieved {Count} components for project: {ProjectId}", components.Count, projectId);
            return Ok(components);
        }

        /// <summary>
        /// Get all components for a given solution.
        /// </summary>
        [HttpGet("by-solution/{solutionId}")]
        public async Task<ActionResult<IEnumerable<Component>>> GetBySolution(Guid solutionId)
        {
            _logger.LogInformation("Getting components by solution: {SolutionId}", solutionId);
            var components = await _service.GetBySolutionAsync(solutionId);
            _logger.LogInformation("Retrieved {Count} components for solution: {SolutionId}", components.Count, solutionId);
            return Ok(components);
        }
    }
}
