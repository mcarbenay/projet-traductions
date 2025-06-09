using Microsoft.AspNetCore.Mvc;
using UseTheOps.PolyglotInitiative.Models;
using UseTheOps.PolyglotInitiative.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UseTheOps.PolyglotInitiative.Controllers
{
    /// <summary>
    /// API endpoints for managing projects.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectService _service;
        private readonly AuthorizationService _authz;
        public ProjectsController(ProjectService service, AuthorizationService authz)
        {
            _service = service;
            _authz = authz;
        }

        /// <summary>
        /// Get all projects.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetAll()
        {
            var projects = await _service.GetAllAsync();
            return Ok(projects);
        }

        /// <summary>
        /// Get a project by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> Get(Guid id)
        {
            var project = await _service.GetByIdAsync(id);
            if (project == null) return NotFound();
            return Ok(project);
        }

        /// <summary>
        /// Create a new project.
        /// </summary>
        /// <param name="project">The project to create.</param>
        /// <returns>The created project.</returns>
        [HttpPost]
        public async Task<ActionResult<Project>> Create(Project project)
        {
            if (!await _authz.CanManageSolutionAsync(project.SolutionId))
                return Forbid();
            var (success, error, created) = await _service.CreateAsync(project);
            if (!success)
            {
                if (error == "Solution does not exist.") return BadRequest(error);
                if (error != null && error.Contains("exists")) return Conflict(error);
                return BadRequest(error);
            }
            return CreatedAtAction(nameof(Get), new { id = created!.Id }, created);
        }

        /// <summary>
        /// Update a project.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Project project)
        {
            if (!await _authz.CanManageProjectAsync(id))
                return Forbid();
            var (success, error) = await _service.UpdateAsync(id, project);
            if (!success)
            {
                if (error == "ID mismatch.") return BadRequest(error);
                if (error == "Project not found.") return NotFound();
                if (error != null && error.Contains("exists")) return Conflict(error);
                return BadRequest(error);
            }
            return NoContent();
        }

        /// <summary>
        /// Delete a project.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!await _authz.CanManageProjectAsync(id))
                return Forbid();
            var (success, error) = await _service.DeleteAsync(id);
            if (!success)
            {
                if (error == "Project not found.") return NotFound();
                return BadRequest(error);
            }
            return NoContent();
        }

        /// <summary>
        /// Get all projects for a given solution, including their components.
        /// </summary>
        [HttpGet("by-solution/{solutionId}")]
        public async Task<ActionResult<IEnumerable<Project>>> GetBySolution(Guid solutionId)
        {
            var projects = await _service.GetBySolutionWithComponentsAsync(solutionId);
            return Ok(projects);
        }
    }
}
