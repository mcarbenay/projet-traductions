using Microsoft.AspNetCore.Mvc;
using UseTheOps.PolyglotInitiative.Models;
using UseTheOps.PolyglotInitiative.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UseTheOps.PolyglotInitiative.Controllers
{
    /// <summary>
    /// API endpoints for managing API keys.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ApiKeysController : ControllerBase
    {
        private readonly ApiKeyService _service;
        private readonly AuthorizationService _authz;
        public ApiKeysController(ApiKeyService service, AuthorizationService authz)
        {
            _service = service;
            _authz = authz;
        }

        /// <summary>
        /// Get all API keys.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApiKey>>> GetAll()
        {
            var keys = await _service.GetAllAsync();
            return Ok(keys);
        }

        /// <summary>
        /// Get an API key by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiKey>> Get(Guid id)
        {
            var key = await _service.GetByIdAsync(id);
            if (key == null) return NotFound();
            return Ok(key);
        }

        /// <summary>
        /// Create a new API key.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiKey>> Create(ApiKey key)
        {
            if (!await _authz.CanManageSolutionAsync(key.SolutionId))
                return Forbid();
            var created = await _service.CreateAsync(key);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update an API key.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ApiKey key)
        {
            var success = await _service.UpdateAsync(id, key);
            if (!success) return BadRequest();
            return NoContent();
        }

        /// <summary>
        /// Delete an API key.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var key = await _service.GetByIdAsync(id);
            if (key == null) return NotFound();
            if (!await _authz.CanManageSolutionAsync(key.SolutionId))
                return Forbid();
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
