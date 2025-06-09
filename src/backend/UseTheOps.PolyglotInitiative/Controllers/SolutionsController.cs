using Microsoft.AspNetCore.Mvc;
using UseTheOps.PolyglotInitiative.Models;
using UseTheOps.PolyglotInitiative.Models.Dtos;
using UseTheOps.PolyglotInitiative.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UseTheOps.PolyglotInitiative.Helpers;

namespace UseTheOps.PolyglotInitiative.Controllers
{
    /// <summary>
    /// API endpoints for managing solutions.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SolutionsController : ControllerBase
    {
        private readonly SolutionService _service;
        private readonly AuthorizationService _authz;
        private readonly ILogger<SolutionsController> _logger;
        public SolutionsController(SolutionService service, AuthorizationService authz, ILogger<SolutionsController> logger)
        {
            _service = service;
            _authz = authz;
            _logger = logger;
        }

        /// <summary>
        /// Get all solutions.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Solution>>> GetAll()
        {
            _logger.LogInformation("Getting all solutions");
            var solutions = await _service.GetAllAsync();
            _logger.LogInformation("Retrieved {Count} solutions", solutions.Count);
            return Ok(solutions);
        }

        /// <summary>
        /// Get a solution by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            _logger.LogInformation($"Getting solution with ID {id}");
            try
            {
                var solution = await _service.GetByIdAsync(id);
                if (solution == null)
                {
                    _logger.LogWarning($"Solution with ID {id} not found");
                    return ExceptionHelper.ToActionResult(new KeyNotFoundException($"Solution not found: {id}"), this, nameof(Get));
                }
                _logger.LogInformation($"Retrieved solution with ID {id}");
                return Ok(solution);
            }
            catch (Exception ex)
            {
                return ExceptionHelper.ToActionResult(ex, this, nameof(Get));
            }
        }

        /// <summary>
        /// Create a new solution.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(SolutionCreateDto dto)
        {
            if (!_authz.IsAdmin())
            {
                _logger.LogWarning("Unauthorized attempt to create solution by user {UserId}", dto.OwnerId);
                return ExceptionHelper.ToActionResult(new UnauthorizedAccessException($"Unauthorized create attempt for solution by user: {dto.OwnerId}"), this, nameof(Create));
            }
            _logger.LogInformation("Creating solution {@Solution}", dto);
            try
            {
                var solution = new Solution
                {
                    Code = dto.Code,
                    Name = dto.Name,
                    Description = dto.Description,
                    PresentationUrl = dto.PresentationUrl,
                    OwnerId = dto.OwnerId
                };
                var created = await _service.CreateAsync(solution);
                _logger.LogInformation("Created solution with ID {Id}", created.Id);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return ExceptionHelper.ToActionResult(ex, this, nameof(Create));
            }
        }

        /// <summary>
        /// Update a solution.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, SolutionUpdateDto dto)
        {
            if (!_authz.IsAdmin())
            {
                _logger.LogWarning("Unauthorized attempt to update solution ID {Id} by user {UserId}", id, dto.OwnerId);
                return ExceptionHelper.ToActionResult(new UnauthorizedAccessException($"Unauthorized update attempt for solution: {id}"), this, nameof(Update));
            }
            _logger.LogInformation("Updating solution ID {Id} with data {@Solution}", id, dto);
            try
            {
                var solution = new Solution
                {
                    Id = id,
                    Code = dto.Code,
                    Name = dto.Name,
                    Description = dto.Description,
                    PresentationUrl = dto.PresentationUrl,
                    OwnerId = dto.OwnerId
                };
                var success = await _service.UpdateAsync(id, solution);
                if (!success)
                {
                    _logger.LogError("Error updating solution ID {Id}", id);
                    return ExceptionHelper.ToActionResult(new ArgumentException($"Error updating solution: {id}"), this, nameof(Update));
                }
                _logger.LogInformation("Updated solution ID {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return ExceptionHelper.ToActionResult(ex, this, nameof(Update));
            }
        }

        /// <summary>
        /// Delete a solution.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!_authz.IsAdmin())
            {
                _logger.LogWarning("Unauthorized attempt to delete solution ID {Id} by user {UserId}", id, User?.Identity?.Name ?? "unknown");
                return ExceptionHelper.ToActionResult(new UnauthorizedAccessException($"Unauthorized delete attempt for solution: {id}"), this, nameof(Delete));
            }
            _logger.LogInformation("Deleting solution ID {Id}", id);
            try
            {
                var success = await _service.DeleteAsync(id);
                if (!success)
                {
                    _logger.LogError("Error deleting solution ID {Id}", id);
                    return ExceptionHelper.ToActionResult(new KeyNotFoundException($"Solution not found: {id}"), this, nameof(Delete));
                }
                _logger.LogInformation("Deleted solution ID {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return ExceptionHelper.ToActionResult(ex, this, nameof(Delete));
            }
        }
    }
}
