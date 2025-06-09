using Microsoft.AspNetCore.Mvc;
using UseTheOps.PolyglotInitiative.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UseTheOps.PolyglotInitiative.Services;

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
        public TranslatableResourcesController(TranslatableResourceService service, AuthorizationService authz)
        {
            _service = service;
            _authz = authz;
        }

        /// <summary>
        /// Get all translatable resources.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TranslatableResource>>> GetAll()
        {
            var resources = await _service.GetAllAsync();
            return Ok(resources);
        }

        /// <summary>
        /// Get a translatable resource by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TranslatableResource>> Get(Guid id)
        {
            var resource = await _service.GetByIdAsync(id);
            if (resource == null) return NotFound();
            return Ok(resource);
        }

        /// <summary>
        /// Create a new translatable resource.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TranslatableResource>> Create(TranslatableResource resource)
        {
            if (!await _authz.CanEditFileAsync(resource.ResourceFileId))
                return Forbid();
            var created = await _service.CreateAsync(resource);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update a translatable resource.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, TranslatableResource resource)
        {
            if (!await _authz.CanEditFileAsync(resource.ResourceFileId))
                return Forbid();
            var success = await _service.UpdateAsync(id, resource);
            if (!success) return BadRequest();
            return NoContent();
        }

        /// <summary>
        /// Delete a translatable resource.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var resource = await _service.GetByIdAsync(id);
            if (resource == null) return NotFound();
            if (!await _authz.CanEditFileAsync(resource.ResourceFileId))
                return Forbid();
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
