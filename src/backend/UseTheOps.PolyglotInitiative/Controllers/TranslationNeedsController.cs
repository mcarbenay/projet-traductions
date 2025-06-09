using Microsoft.AspNetCore.Mvc;
using UseTheOps.PolyglotInitiative.Models;
using UseTheOps.PolyglotInitiative.Services;
using UseTheOps.PolyglotInitiative.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace UseTheOps.PolyglotInitiative.Controllers
{
    /// <summary>
    /// API endpoints for managing translation needs (languages/variants).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TranslationNeedsController : ControllerBase
    {
        private readonly TranslationNeedService _service;
        private readonly AuthorizationService _authz;
        private readonly ILogger<TranslationNeedsController> _logger;
        public TranslationNeedsController(TranslationNeedService service, AuthorizationService authz, ILogger<TranslationNeedsController> logger)
        {
            _service = service;
            _authz = authz;
            _logger = logger;
        }

        /// <summary>
        /// Get all translation needs.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TranslationNeed>>> GetAll()
        {
            _logger.LogInformation("Getting all translation needs.");
            var needs = await _service.GetAllAsync();
            _logger.LogInformation("Retrieved {Count} translation needs.", needs.Count);
            return Ok(needs);
        }

        /// <summary>
        /// Get a translation need by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            _logger.LogInformation($"Getting translation need by ID: {id}");
            try
            {
                var need = await _service.GetByIdAsync(id);
                if (need == null)
                {
                    _logger.LogWarning($"Translation need not found: {id}");
                    return ExceptionHelper.ToActionResult(new KeyNotFoundException($"Translation need not found: {id}"), this, nameof(Get));
                }
                _logger.LogInformation($"Retrieved translation need: {id}");
                return Ok(need);
            }
            catch (Exception ex)
            {
                return ExceptionHelper.ToActionResult(ex, this, nameof(Get));
            }
        }

        /// <summary>
        /// Create a new translation need.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TranslationNeed>> Create(TranslationNeed need)
        {
            _logger.LogInformation("Creating new translation need for solution: {SolutionId}", need.SolutionId);
            if (!await _authz.CanManageSolutionAsync(need.SolutionId))
            {
                _logger.LogWarning("Unauthorized create attempt for solution: {SolutionId}", need.SolutionId);
                return Forbid();
            }
            var created = await _service.CreateAsync(need);
            _logger.LogInformation("Created translation need: {Id}", created.Id);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update a translation need.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, TranslationNeed need)
        {
            _logger.LogInformation($"Updating translation need: {id}");
            if (!await _authz.CanManageSolutionAsync(need.SolutionId))
            {
                _logger.LogWarning($"Unauthorized update attempt for solution: {need.SolutionId}");
                return ExceptionHelper.ToActionResult(new UnauthorizedAccessException($"Unauthorized update attempt for translation need: {id}"), this, nameof(Update));
            }
            try
            {
                var success = await _service.UpdateAsync(id, need);
                if (!success)
                {
                    _logger.LogError($"Error updating translation need (not found or conflict): {id}");
                    return ExceptionHelper.ToActionResult(new ArgumentException($"Error updating translation need: {id}"), this, nameof(Update));
                }
                _logger.LogInformation($"Updated translation need: {id}");
                return NoContent();
            }
            catch (Exception ex)
            {
                return ExceptionHelper.ToActionResult(ex, this, nameof(Update));
            }
        }

        /// <summary>
        /// Delete a translation need.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation($"Deleting translation need: {id}");
            try
            {
                var need = await _service.GetByIdAsync(id);
                if (need == null)
                {
                    _logger.LogWarning($"Delete attempt for non-existing translation need: {id}");
                    return ExceptionHelper.ToActionResult(new KeyNotFoundException($"Translation need not found: {id}"), this, nameof(Delete));
                }
                if (!await _authz.CanManageSolutionAsync(need.SolutionId))
                {
                    _logger.LogWarning($"Unauthorized delete attempt for solution: {need.SolutionId}");
                    return ExceptionHelper.ToActionResult(new UnauthorizedAccessException($"Unauthorized delete attempt for translation need: {id}"), this, nameof(Delete));
                }
                var success = await _service.DeleteAsync(id);
                if (!success)
                {
                    _logger.LogError($"Error deleting translation need (not found or conflict): {id}");
                    return ExceptionHelper.ToActionResult(new ArgumentException($"Error deleting translation need: {id}"), this, nameof(Delete));
                }
                _logger.LogInformation($"Deleted translation need: {id}");
                return NoContent();
            }
            catch (Exception ex)
            {
                return ExceptionHelper.ToActionResult(ex, this, nameof(Delete));
            }
        }

        /// <summary>
        /// Get all translation needs for a given resource file.
        /// </summary>
        [HttpGet("by-file/{resourceFileId}")]
        public async Task<ActionResult<IEnumerable<TranslationNeed>>> GetByFile(Guid resourceFileId)
        {
            _logger.LogInformation("Getting translation needs by resource file ID: {ResourceFileId}", resourceFileId);
            var needs = await _service.GetByFileAsync(resourceFileId);
            _logger.LogInformation("Retrieved {Count} translation needs for resource file: {ResourceFileId}", needs.Count, resourceFileId);
            return Ok(needs);
        }

        /// <summary>
        /// Get all translation needs with at least one ResourceTranslation pending validation, filtered by solution, project, or file.
        /// </summary>
        [HttpGet("with-pending")]
        public async Task<ActionResult<IEnumerable<TranslationNeed>>> GetWithPending([FromQuery] Guid? solutionId, [FromQuery] Guid? projectId, [FromQuery] Guid? resourceFileId)
        {
            _logger.LogInformation("Getting translation needs with pending validation. SolutionId: {SolutionId}, ProjectId: {ProjectId}, ResourceFileId: {ResourceFileId}", solutionId, projectId, resourceFileId);
            var needs = await _service.GetWithPendingAsync(solutionId, projectId, resourceFileId);
            _logger.LogInformation("Retrieved {Count} translation needs with pending validation.", needs.Count);
            return Ok(needs);
        }
    }
}
