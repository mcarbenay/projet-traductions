using Microsoft.AspNetCore.Mvc;
using UseTheOps.PolyglotInitiative.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UseTheOps.PolyglotInitiative.Services;
using Microsoft.Extensions.Logging;
using UseTheOps.PolyglotInitiative.Helpers;

namespace UseTheOps.PolyglotInitiative.Controllers
{
    /// <summary>
    /// API endpoints for managing resource translations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ResourceTranslationsController : ControllerBase
    {
        private readonly ResourceTranslationService _service;
        private readonly AuthorizationService _authz;
        private readonly ILogger<ResourceTranslationsController> _logger;
        public ResourceTranslationsController(ResourceTranslationService service, AuthorizationService authz, ILogger<ResourceTranslationsController> logger)
        {
            _service = service;
            _authz = authz;
            _logger = logger;
        }

        /// <summary>
        /// Get all resource translations.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResourceTranslation>>> GetAll()
        {
            _logger.LogInformation("Getting all resource translations.");
            var translations = await _service.GetAllAsync();
            _logger.LogInformation("Successfully retrieved all resource translations.");
            return Ok(translations);
        }

        /// <summary>
        /// Get a resource translation by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            _logger.LogInformation($"Getting resource translation with ID: {id}.");
            try
            {
                var translation = await _service.GetByIdAsync(id);
                if (translation == null)
                {
                    _logger.LogWarning($"Resource translation with ID: {id} not found.");
                    return ExceptionHelper.ToActionResult(new KeyNotFoundException($"Resource translation not found: {id}"), this, nameof(Get));
                }
                _logger.LogInformation($"Successfully retrieved resource translation with ID: {id}.");
                return Ok(translation);
            }
            catch (Exception ex)
            {
                return ExceptionHelper.ToActionResult(ex, this, nameof(Get));
            }
        }

        /// <summary>
        /// Create a new resource translation.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ResourceTranslation>> Create(ResourceTranslation translation)
        {
            _logger.LogInformation("Creating a new resource translation.");
            if (!await _authz.CanEditFileAsync(translation.TranslatableResourceId))
            {
                _logger.LogWarning($"Unauthorized attempt to edit resource translation for resource ID: {translation.TranslatableResourceId}.");
                return Forbid();
            }
            var created = await _service.CreateAsync(translation);
            _logger.LogInformation($"Successfully created a new resource translation with ID: {created.Id}.");
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update a resource translation.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ResourceTranslation translation)
        {
            _logger.LogInformation($"Updating resource translation with ID: {id}.");
            if (!await _authz.CanEditFileAsync(translation.TranslatableResourceId))
            {
                _logger.LogWarning($"Unauthorized attempt to update resource translation for resource ID: {translation.TranslatableResourceId}.");
                return ExceptionHelper.ToActionResult(new UnauthorizedAccessException($"Unauthorized update attempt for resource translation: {id}"), this, nameof(Update));
            }
            try
            {
                var success = await _service.UpdateAsync(id, translation);
                if (!success)
                {
                    _logger.LogError($"Error updating resource translation with ID: {id}.");
                    return ExceptionHelper.ToActionResult(new ArgumentException($"Error updating resource translation: {id}"), this, nameof(Update));
                }
                _logger.LogInformation($"Successfully updated resource translation with ID: {id}.");
                return NoContent();
            }
            catch (Exception ex)
            {
                return ExceptionHelper.ToActionResult(ex, this, nameof(Update));
            }
        }

        /// <summary>
        /// Delete a resource translation.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation($"Deleting resource translation with ID: {id}.");
            try
            {
                var translation = await _service.GetByIdAsync(id);
                if (translation == null)
                {
                    _logger.LogWarning($"Resource translation with ID: {id} not found for deletion.");
                    return ExceptionHelper.ToActionResult(new KeyNotFoundException($"Resource translation not found: {id}"), this, nameof(Delete));
                }
                if (!await _authz.CanEditFileAsync(translation.TranslatableResourceId))
                {
                    _logger.LogWarning($"Unauthorized attempt to delete resource translation for resource ID: {translation.TranslatableResourceId}.");
                    return ExceptionHelper.ToActionResult(new UnauthorizedAccessException($"Unauthorized delete attempt for resource translation: {id}"), this, nameof(Delete));
                }
                var success = await _service.DeleteAsync(id);
                if (!success)
                {
                    _logger.LogError($"Error deleting resource translation with ID: {id}.");
                    return ExceptionHelper.ToActionResult(new ArgumentException($"Error deleting resource translation: {id}"), this, nameof(Delete));
                }
                _logger.LogInformation($"Successfully deleted resource translation with ID: {id}.");
                return NoContent();
            }
            catch (Exception ex)
            {
                return ExceptionHelper.ToActionResult(ex, this, nameof(Delete));
            }
        }
    }
}
