using Microsoft.AspNetCore.Mvc;
using UseTheOps.PolyglotInitiative.Models;
using UseTheOps.PolyglotInitiative.Services;
using UseTheOps.PolyglotInitiative.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UseTheOps.PolyglotInitiative.Helpers;

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
        private readonly ILogger<ProjectsController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectsController"/> class.
        /// </summary>
        public ProjectsController(ProjectService service, AuthorizationService authz, ILogger<ProjectsController> logger)
        {
            _service = service;
            _authz = authz;
            _logger = logger;
        }

        /// <summary>
        /// Get all projects.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetAll()
        {
            _logger.LogInformation("Getting all projects.");
            var projects = await _service.GetAllAsync();
            _logger.LogInformation("Retrieved {ProjectCount} projects.", projects.Count);
            return Ok(projects);
        }

        /// <summary>
        /// Get a project by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            _logger.LogInformation("Getting project by ID: {Id}", id);
            try
            {
                var project = await _service.GetByIdAsync(id);
                if (project == null)
                {
                    _logger.LogWarning("Project not found: {Id}", id);
                    return ExceptionHelper.ToActionResult(new KeyNotFoundException($"Project not found: {id}"), this, nameof(Get));
                }
                _logger.LogInformation("Retrieved project: {ProjectName}", project.Name);
                return Ok(project);
            }
            catch (Exception ex)
            {
                return ExceptionHelper.ToActionResult(ex, this, nameof(Get));
            }
        }

        /// <summary>
        /// Create a new project.
        /// </summary>
        /// <param name="dto">The project to create.</param>
        /// <returns>The created project.</returns>
        [HttpPost]
        public async Task<IActionResult> Create(ProjectCreateDto dto)
        {
            _logger.LogInformation("Creating project: {ProjectName}", dto.Name);
            try
            {
                if (!await _authz.CanManageSolutionAsync(dto.SolutionId))
                {
                    _logger.LogWarning("Unauthorized attempt to create project for solution: {SolutionId}", dto.SolutionId);
                    return ExceptionHelper.ToActionResult(new UnauthorizedAccessException($"Unauthorized create attempt for solution: {dto.SolutionId}"), this, nameof(Create));
                }
                var project = new Project
                {
                    Code = dto.Code,
                    Name = dto.Name,
                    Description = dto.Description,
                    Origin = dto.Origin,
                    OriginUrl = dto.OriginUrl,
                    ExternalIdentifierId = dto.ExternalIdentifierId,
                    SolutionId = dto.SolutionId
                };
                var (success, error, created) = await _service.CreateAsync(project);
                if (!success)
                {
                    _logger.LogError("Error creating project: {Error}", error);
                    if (error == "Solution does not exist.")
                        return ExceptionHelper.ToActionResult(new KeyNotFoundException(error), this, nameof(Create));
                    if (error != null && error.Contains("exists"))
                        return Conflict(new Microsoft.AspNetCore.Mvc.ProblemDetails {
                            Type = "https://tools.ietf.org/html/rfc7807",
                            Title = this.LocalizerOrDefault("Error_Conflict"),
                            Status = 409,
                            Detail = this.LocalizerOrDefault("Error_Conflict"),
                            Instance = HttpContext.Request.Path,
                            Extensions = { ["code"] = "ConflictError" }
                        });
                    return ExceptionHelper.ToActionResult(new ArgumentException(error), this, nameof(Create));
                }
                _logger.LogInformation("Created project: {ProjectName}", created?.Name ?? "null");
                return CreatedAtAction(nameof(Get), new { id = created!.Id }, created);
            }
            catch (Exception ex)
            {
                return ExceptionHelper.ToActionResult(ex, this, nameof(Create));
            }
        }

        /// <summary>
        /// Update a project.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ProjectUpdateDto dto)
        {
            _logger.LogInformation("Updating project: {Id}", id);
            try
            {
                // On ne peut pas comparer dto.Id, il faut utiliser seulement l'id du paramètre
                if (!await _authz.CanManageProjectAsync(id))
                {
                    _logger.LogWarning("Unauthorized attempt to update project: {Id}", id);
                    return ExceptionHelper.ToActionResult(new UnauthorizedAccessException($"Unauthorized update attempt for project: {id}"), this, nameof(Update));
                }
                var project = new Project
                {
                    Id = id,
                    Code = dto.Code,
                    Name = dto.Name,
                    Description = dto.Description,
                    Origin = dto.Origin,
                    OriginUrl = dto.OriginUrl,
                    ExternalIdentifierId = dto.ExternalIdentifierId,
                    SolutionId = dto.SolutionId
                };
                var (success, error) = await _service.UpdateAsync(id, project);
                if (!success)
                {
                    _logger.LogError("Error updating project: {Error}", error);
                    if (error == "ID mismatch.")
                        return ExceptionHelper.ToActionResult(new ArgumentException(error), this, nameof(Update));
                    if (error == "Project not found.")
                        return ExceptionHelper.ToActionResult(new KeyNotFoundException(error), this, nameof(Update));
                    if (error != null && error.Contains("exists"))
                        return Conflict(new Microsoft.AspNetCore.Mvc.ProblemDetails {
                            Type = "https://tools.ietf.org/html/rfc7807",
                            Title = this.LocalizerOrDefault("Error_Conflict"),
                            Status = 409,
                            Detail = this.LocalizerOrDefault("Error_Conflict"),
                            Instance = HttpContext.Request.Path,
                            Extensions = { ["code"] = "ConflictError" }
                        });
                    return ExceptionHelper.ToActionResult(new ArgumentException(error), this, nameof(Update));
                }
                _logger.LogInformation("Updated project: {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return ExceptionHelper.ToActionResult(ex, this, nameof(Update));
            }
        }

        /// <summary>
        /// Delete a project.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Deleting project: {Id}", id);
            try
            {
                if (!await _authz.CanManageProjectAsync(id))
                {
                    _logger.LogWarning("Unauthorized attempt to delete project: {Id}", id);
                    return ExceptionHelper.ToActionResult(new UnauthorizedAccessException($"Unauthorized delete attempt for project: {id}"), this, nameof(Delete));
                }
                var (success, error) = await _service.DeleteAsync(id);
                if (!success)
                {
                    _logger.LogError("Error deleting project: {Error}", error);
                    if (error == "Project not found.")
                        return ExceptionHelper.ToActionResult(new KeyNotFoundException(error), this, nameof(Delete));
                    return ExceptionHelper.ToActionResult(new ArgumentException(error), this, nameof(Delete));
                }
                _logger.LogInformation("Deleted project: {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return ExceptionHelper.ToActionResult(ex, this, nameof(Delete));
            }
        }

        /// <summary>
        /// Get all projects for a given solution, including their components.
        /// </summary>
        [HttpGet("by-solution/{solutionId}")]
        public async Task<ActionResult<IEnumerable<Project>>> GetBySolution(Guid solutionId)
        {
            _logger.LogInformation("Getting projects by solution: {SolutionId}", solutionId);
            var projects = await _service.GetBySolutionWithComponentsAsync(solutionId);
            _logger.LogInformation("Retrieved {ProjectCount} projects for solution: {SolutionId}", projects.Count, solutionId);
            return Ok(projects);
        }
    }
}
