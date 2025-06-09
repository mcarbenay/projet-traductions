using Microsoft.AspNetCore.Mvc;
using UseTheOps.PolyglotInitiative.Models;
using UseTheOps.PolyglotInitiative.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UseTheOps.PolyglotInitiative.Controllers
{
    /// <summary>
    /// API endpoints for managing users.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _service;
        private readonly AuthorizationService _authz;
        public UsersController(UserService service, AuthorizationService authz)
        {
            _service = service;
            _authz = authz;
        }

        /// <summary>
        /// Get all users.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            var users = await _service.GetAllAsync();
            return Ok(users);
        }

        /// <summary>
        /// Get a user by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get(Guid id)
        {
            var user = await _service.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        /// <summary>
        /// Create a new user.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<User>> Create(User user)
        {
            if (!_authz.IsAdmin())
                return Forbid();
            var created = await _service.CreateAsync(user);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update a user.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, User user)
        {
            if (!_authz.IsAdmin())
                return Forbid();
            var success = await _service.UpdateAsync(id, user);
            if (!success) return BadRequest();
            return NoContent();
        }

        /// <summary>
        /// Delete a user.
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
