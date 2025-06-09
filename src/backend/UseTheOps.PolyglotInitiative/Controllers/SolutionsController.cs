using Microsoft.AspNetCore.Mvc;
using UseTheOps.PolyglotInitiative.Models;
using UseTheOps.PolyglotInitiative.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public SolutionsController(SolutionService service, AuthorizationService authz)
        {
            _service = service;
            _authz = authz;
        }

        /// <summary>
        /// Get all solutions.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Solution>>> GetAll()
        {
            var solutions = await _service.GetAllAsync();
            return Ok(solutions);
        }

        /// <summary>
        /// Get a solution by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Solution>> Get(Guid id)
        {
            var solution = await _service.GetByIdAsync(id);
            if (solution == null) return NotFound();
            return Ok(solution);
        }

        /// <summary>
        /// Create a new solution.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Solution>> Create(Solution solution)
        {
            if (!_authz.IsAdmin())
                return Forbid();
            var created = await _service.CreateAsync(solution);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update a solution.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Solution solution)
        {
            if (!_authz.IsAdmin())
                return Forbid();
            var success = await _service.UpdateAsync(id, solution);
            if (!success) return BadRequest();
            return NoContent();
        }

        /// <summary>
        /// Delete a solution.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!_authz.IsAdmin())
                return Forbid();
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
