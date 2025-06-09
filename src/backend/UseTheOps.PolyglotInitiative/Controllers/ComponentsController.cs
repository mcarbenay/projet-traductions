using Microsoft.AspNetCore.Mvc;
using UseTheOps.PolyglotInitiative.Models;
using UseTheOps.PolyglotInitiative.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public ComponentsController(ComponentService service, AuthorizationService authz)
        {
            _service = service;
            _authz = authz;
        }

        /// <summary>
        /// Get all components.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Component>>> GetAll()
        {
            var components = await _service.GetAllAsync();
            return Ok(components);
        }

        /// <summary>
        /// Get a component by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Component>> Get(Guid id)
        {
            var component = await _service.GetByIdAsync(id);
            if (component == null) return NotFound();
            return Ok(component);
        }

        /// <summary>
        /// Create a new component.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Component>> Create(Component component)
        {
            if (!await _authz.CanManageProjectAsync(component.ProjectId))
                return Forbid();
            var created = await _service.CreateAsync(component);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update a component.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Component component)
        {
            if (!await _authz.CanManageComponentAsync(id))
                return Forbid();
            var success = await _service.UpdateAsync(id, component);
            if (!success) return BadRequest();
            return NoContent();
        }

        /// <summary>
        /// Delete a component.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!await _authz.CanManageComponentAsync(id))
                return Forbid();
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Get all components for a given project.
        /// </summary>
        [HttpGet("by-project/{projectId}")]
        public async Task<ActionResult<IEnumerable<Component>>> GetByProject(Guid projectId)
        {
            var components = await _service.GetByProjectAsync(projectId);
            return Ok(components);
        }

        /// <summary>
        /// Get all components for a given solution.
        /// </summary>
        [HttpGet("by-solution/{solutionId}")]
        public async Task<ActionResult<IEnumerable<Component>>> GetBySolution(Guid solutionId)
        {
            var components = await _service.GetBySolutionAsync(solutionId);
            return Ok(components);
        }
    }
}
