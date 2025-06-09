using Microsoft.AspNetCore.Mvc;
using UseTheOps.PolyglotInitiative.Models;
using UseTheOps.PolyglotInitiative.Services;
using UseTheOps.PolyglotInitiative.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace UseTheOps.PolyglotInitiative.Controllers
{
    /// <summary>
    /// API endpoints for managing users.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserService _service;
        private readonly AuthorizationService _authz;
        private readonly ILogger<UsersController> _logger;
        public UsersController(UserService service, AuthorizationService authz, ILogger<UsersController> logger)
        {
            _service = service;
            _authz = authz;
            _logger = logger;
        }

        /// <summary>
        /// Get all users.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            _logger.LogInformation("Getting all users");
            var users = await _service.GetAllAsync();
            var dtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                IsAdministrator = u.IsAdministrator,
                Status = u.Status
            });
            _logger.LogInformation("Successfully retrieved all users");
            return Ok(dtos);
        }

        /// <summary>
        /// Get a user by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> Get(Guid id)
        {
            _logger.LogInformation($"Getting user by ID: {id}");
            var user = await _service.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning($"User not found: {id}");
                return NotFound();
            }
            var dto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                IsAdministrator = user.IsAdministrator,
                Status = user.Status
            };
            _logger.LogInformation($"Successfully retrieved user: {id}");
            return Ok(dto);
        }

        /// <summary>
        /// Create a new user.
        /// </summary>
        public class CreateUserResult
        {
            public UserDto User { get; set; } = null!;
            public string ActivationLink { get; set; } = string.Empty;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<CreateUserResult>> Create(UserCreateDto dto)
        {
            if (!_authz.IsAdmin())
                return Forbid();
            // Generate a random password for the new user
            var randomPassword = Guid.NewGuid().ToString("N").Substring(0, 12);
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = UserService.HashPassword(randomPassword),
                IsAdministrator = dto.IsAdministrator,
                Status = "pending"
            };
            // Map solution accesses
            foreach (UseTheOps.PolyglotInitiative.Models.Dtos.UserSolutionAccessDto access in dto.SolutionAccesses)
            {
                user.UserSolutionAccesses.Add(new UserSolutionAccess
                {
                    SolutionId = access.SolutionId,
                    AccessLevel = access.AccessLevel
                });
            }
            var created = await _service.CreateAsync(user);
            // Build activation link
            var tokenRaw = $"{created.Id}:{randomPassword}";
            var tokenBase64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(tokenRaw));
            var request = HttpContext.Request;
            var rootUrl = $"{request.Scheme}://{request.Host}";
            var activationLink = $"{rootUrl}/register/user?token={tokenBase64}";
            // TODO: Send email to user with account finalization link and random password
            var result = new CreateUserResult
            {
                User = new UserDto
                {
                    Id = created.Id,
                    Name = created.Name,
                    Email = created.Email,
                    IsAdministrator = created.IsAdministrator,
                    Status = created.Status
                },
                ActivationLink = activationLink
            };
            _logger.LogInformation($"User created: {created.Id}, ActivationLink: {activationLink}");
            return CreatedAtAction(nameof(Get), new { id = created.Id }, result);
        }

        /// <summary>
        /// Update a user.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UserUpdateDto dto)
        {
            if (!_authz.IsAdmin())
                return Forbid();
            var user = await _service.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning($"User not found for update: {id}");
                return NotFound();
            }
            user.Name = dto.Name;
            user.Email = dto.Email;
            user.IsAdministrator = dto.IsAdministrator;
            // Do not update status here
            user.UserSolutionAccesses.Clear();
            foreach (UseTheOps.PolyglotInitiative.Models.Dtos.UserSolutionAccessDto access in dto.SolutionAccesses)
            {
                user.UserSolutionAccesses.Add(new UserSolutionAccess
                {
                    SolutionId = access.SolutionId,
                    AccessLevel = access.AccessLevel
                });
            }
            var success = await _service.UpdateAsync(id, user);
            if (!success)
            {
                _logger.LogError($"Error updating user: {id}");
                return BadRequest();
            }
            _logger.LogInformation($"User updated: {id}");
            return NoContent();
        }

        /// <summary>
        /// Delete a user.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!_authz.IsAdmin())
                return Forbid();
            var success = await _service.DeleteAsync(id);
            if (!success)
            {
                _logger.LogWarning($"User not found for deletion: {id}");
                return NotFound();
            }
            _logger.LogInformation($"User deleted: {id}");
            return NoContent();
        }
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsAdministrator { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
