using Microsoft.AspNetCore.Mvc;
using UseTheOps.PolyglotInitiative.Models;
using UseTheOps.PolyglotInitiative.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public TranslationNeedsController(TranslationNeedService service, AuthorizationService authz)
        {
            _service = service;
            _authz = authz;
        }

        /// <summary>
        /// Get all translation needs.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TranslationNeed>>> GetAll()
        {
            var needs = await _service.GetAllAsync();
            return Ok(needs);
        }

        /// <summary>
        /// Get a translation need by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TranslationNeed>> Get(Guid id)
        {
            var need = await _service.GetByIdAsync(id);
            if (need == null) return NotFound();
            return Ok(need);
        }

        /// <summary>
        /// Create a new translation need.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TranslationNeed>> Create(TranslationNeed need)
        {
            if (!await _authz.CanManageSolutionAsync(need.SolutionId))
                return Forbid();
            var created = await _service.CreateAsync(need);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update a translation need.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, TranslationNeed need)
        {
            if (!await _authz.CanManageSolutionAsync(need.SolutionId))
                return Forbid();
            var success = await _service.UpdateAsync(id, need);
            if (!success) return BadRequest();
            return NoContent();
        }

        /// <summary>
        /// Delete a translation need.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var need = await _service.GetByIdAsync(id);
            if (need == null) return NotFound();
            if (!await _authz.CanManageSolutionAsync(need.SolutionId))
                return Forbid();
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Get all translation needs for a given resource file.
        /// </summary>
        [HttpGet("by-file/{resourceFileId}")]
        public async Task<ActionResult<IEnumerable<TranslationNeed>>> GetByFile(Guid resourceFileId)
        {
            var needs = await _service.GetByFileAsync(resourceFileId);
            return Ok(needs);
        }

        /// <summary>
        /// Get all translation needs with at least one ResourceTranslation pending validation, filtered by solution, project, or file.
        /// </summary>
        [HttpGet("with-pending")]
        public async Task<ActionResult<IEnumerable<TranslationNeed>>> GetWithPending([FromQuery] Guid? solutionId, [FromQuery] Guid? projectId, [FromQuery] Guid? resourceFileId)
        {
            var needs = await _service.GetWithPendingAsync(solutionId, projectId, resourceFileId);
            return Ok(needs);
        }
    }
}
