using Microsoft.AspNetCore.Mvc;
using UseTheOps.PolyglotInitiative.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UseTheOps.PolyglotInitiative.Services;

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
        public ResourceTranslationsController(ResourceTranslationService service, AuthorizationService authz)
        {
            _service = service;
            _authz = authz;
        }

        /// <summary>
        /// Get all resource translations.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResourceTranslation>>> GetAll()
        {
            var translations = await _service.GetAllAsync();
            return Ok(translations);
        }

        /// <summary>
        /// Get a resource translation by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ResourceTranslation>> Get(Guid id)
        {
            var translation = await _service.GetByIdAsync(id);
            if (translation == null) return NotFound();
            return Ok(translation);
        }

        /// <summary>
        /// Create a new resource translation.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ResourceTranslation>> Create(ResourceTranslation translation)
        {
            if (!await _authz.CanEditFileAsync(translation.TranslatableResourceId))
                return Forbid();
            var created = await _service.CreateAsync(translation);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update a resource translation.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ResourceTranslation translation)
        {
            if (!await _authz.CanEditFileAsync(translation.TranslatableResourceId))
                return Forbid();
            var success = await _service.UpdateAsync(id, translation);
            if (!success) return BadRequest();
            return NoContent();
        }

        /// <summary>
        /// Delete a resource translation.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            // You may need to fetch the translation to get the file/component context
            var translation = await _service.GetByIdAsync(id);
            if (translation == null) return NotFound();
            if (!await _authz.CanEditFileAsync(translation.TranslatableResourceId))
                return Forbid();
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
