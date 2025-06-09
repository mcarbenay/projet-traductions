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
    /// API endpoints for managing translatable resources.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TranslatableResourcesController : ControllerBase
    {
        private readonly TranslatableResourceService _service;
        private readonly AuthorizationService _authz;
        private readonly ILogger<TranslatableResourcesController> _logger;
        public TranslatableResourcesController(TranslatableResourceService service, AuthorizationService authz, ILogger<TranslatableResourcesController> logger)
        {
            _service = service;
            _authz = authz;
            _logger = logger;
        }

        /// <summary>
        /// Get all translatable resources.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TranslatableResource>>> GetAll()
        {
            _logger.LogInformation("Getting all translatable resources.");
            var resources = await _service.GetAllAsync();
            _logger.LogInformation("Successfully retrieved all translatable resources.");
            return Ok(resources);
        }

        /// <summary>
        /// Get a translatable resource by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TranslatableResource>> Get(Guid id)
        {
            _logger.LogInformation($"Getting translatable resource with ID: {id}.");
            var resource = await _service.GetByIdAsync(id);
            if (resource == null) 
            {
                _logger.LogWarning($"Translatable resource with ID: {id} not found.");
                return NotFound();
            }
            _logger.LogInformation($"Successfully retrieved translatable resource with ID: {id}.");
            return Ok(resource);
        }

        /// <summary>
        /// Create a new translatable resource.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TranslatableResource>> Create(TranslatableResource resource)
        {
            _logger.LogInformation("Creating a new translatable resource.");
            if (!await _authz.CanEditFileAsync(resource.ResourceFileId))
            {
                _logger.LogWarning("Unauthorized attempt to create a translatable resource.");
                return Forbid();
            }
            var created = await _service.CreateAsync(resource);
            _logger.LogInformation($"Successfully created a new translatable resource with ID: {created.Id}.");
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update a translatable resource.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, TranslatableResource resource)
        {
            _logger.LogInformation($"Updating translatable resource with ID: {id}.");
            if (!await _authz.CanEditFileAsync(resource.ResourceFileId))
            {
                _logger.LogWarning("Unauthorized attempt to update a translatable resource.");
                return Forbid();
            }
            var success = await _service.UpdateAsync(id, resource);
            if (!success) 
            {
                _logger.LogError($"Error updating translatable resource with ID: {id}.");
                return BadRequest();
            }
            _logger.LogInformation($"Successfully updated translatable resource with ID: {id}.");
            return NoContent();
        }

        /// <summary>
        /// Delete a translatable resource.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation($"Deleting translatable resource with ID: {id}.");
            var resource = await _service.GetByIdAsync(id);
            if (resource == null) 
            {
                _logger.LogWarning($"Translatable resource with ID: {id} not found for deletion.");
                return NotFound();
            }
            if (!await _authz.CanEditFileAsync(resource.ResourceFileId))
            {
                _logger.LogWarning("Unauthorized attempt to delete a translatable resource.");
                return Forbid();
            }
            var success = await _service.DeleteAsync(id);
            if (!success) 
            {
                _logger.LogError($"Error deleting translatable resource with ID: {id}.");
                return NotFound();
            }
            _logger.LogInformation($"Successfully deleted translatable resource with ID: {id}.");
            return NoContent();
        }
    }
}
