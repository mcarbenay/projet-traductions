using Microsoft.AspNetCore.Mvc;
using UseTheOps.PolyglotInitiative.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UseTheOps.PolyglotInitiative.Services;
using Microsoft.Extensions.Logging;

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
        public async Task<ActionResult<ResourceTranslation>> Get(Guid id)
        {
            _logger.LogInformation($"Getting resource translation with ID: {id}.");
            var translation = await _service.GetByIdAsync(id);
            if (translation == null) 
            {
                _logger.LogWarning($"Resource translation with ID: {id} not found.");
                return NotFound();
            }
            _logger.LogInformation($"Successfully retrieved resource translation with ID: {id}.");
            return Ok(translation);
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
                return Forbid();
            }
            var success = await _service.UpdateAsync(id, translation);
            if (!success) 
            {
                _logger.LogError($"Error updating resource translation with ID: {id}.");
                return BadRequest();
            }
            _logger.LogInformation($"Successfully updated resource translation with ID: {id}.");
            return NoContent();
        }

        /// <summary>
        /// Delete a resource translation.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation($"Deleting resource translation with ID: {id}.");
            // You may need to fetch the translation to get the file/component context
            var translation = await _service.GetByIdAsync(id);
            if (translation == null) 
            {
                _logger.LogWarning($"Resource translation with ID: {id} not found for deletion.");
                return NotFound();
            }
            if (!await _authz.CanEditFileAsync(translation.TranslatableResourceId))
            {
                _logger.LogWarning($"Unauthorized attempt to delete resource translation for resource ID: {translation.TranslatableResourceId}.");
                return Forbid();
            }
            var success = await _service.DeleteAsync(id);
            if (!success) 
            {
                _logger.LogError($"Error deleting resource translation with ID: {id}.");
                return NotFound();
            }
            _logger.LogInformation($"Successfully deleted resource translation with ID: {id}.");
            return NoContent();
        }
    }
}
