using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UseTheOps.PolyglotInitiative.Models;
using UseTheOps.PolyglotInitiative.Services;
using UseTheOps.PolyglotInitiative.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace UseTheOps.PolyglotInitiative.Controllers
{
    /// <summary>
    /// API endpoints for managing API keys.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ApiKeysController : ControllerBase
    {
        private readonly ApiKeyService _service;
        private readonly AuthorizationService _authz;
        private readonly ILogger<ApiKeysController> _logger;
        public ApiKeysController(ApiKeyService service, AuthorizationService authz, ILogger<ApiKeysController> logger)
        {
            _service = service;
            _authz = authz;
            _logger = logger;
        }

        /// <summary>
        /// Get all API keys.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApiKeyDto>>> GetAll()
        {
            _logger.LogInformation("GetAll called");
            try {
                var keys = await _service.GetAllAsync();
                var dtos = keys.Select(k => new ApiKeyDto
                {
                    Id = k.Id,
                    SolutionId = k.SolutionId,
                    Scope = k.Scope,
                    CreatedAt = k.CreatedAt,
                    RevokedAt = k.RevokedAt
                });
                _logger.LogInformation("GetAll returned {Count} keys", dtos.Count());
                return Ok(dtos);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error in GetAll");
                throw;
            }
        }

        /// <summary>
        /// Get an API key by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiKeyDto>> Get(Guid id)
        {
            _logger.LogInformation("Get called for id {Id}", id);
            try {
                var key = await _service.GetByIdAsync(id);
                if (key == null) {
                    _logger.LogWarning("Get: NotFound for id {Id}", id);
                    return NotFound();
                }
                var dto = new ApiKeyDto
                {
                    Id = key.Id,
                    SolutionId = key.SolutionId,
                    Scope = key.Scope,
                    CreatedAt = key.CreatedAt,
                    RevokedAt = key.RevokedAt
                };
                return Ok(dto);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error in Get for id {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Create a new API key.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ApiKeyDto>> Create(ApiKeyCreateDto dto)
        {
            _logger.LogInformation("Create called for SolutionId {SolutionId}", dto.SolutionId);
            try {
                if (!await _authz.CanManageSolutionAsync(dto.SolutionId)) {
                    _logger.LogWarning("Create: Forbid for SolutionId {SolutionId}", dto.SolutionId);
                    return Forbid();
                }
                var key = new ApiKey
                {
                    Id = Guid.NewGuid(),
                    SolutionId = dto.SolutionId,
                    KeyValue = Guid.NewGuid().ToString("N"),
                    Scope = dto.Scope,
                    CreatedAt = DateTime.UtcNow
                };
                var created = await _service.CreateAsync(key);
                var result = new ApiKeyDto
                {
                    Id = created.Id,
                    SolutionId = created.SolutionId,
                    Scope = created.Scope,
                    CreatedAt = created.CreatedAt,
                    RevokedAt = created.RevokedAt
                };
                _logger.LogInformation("Create: ApiKey {Id} created", created.Id);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, result);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error in Create for SolutionId {SolutionId}", dto.SolutionId);
                throw;
            }
        }

        /// <summary>
        /// Update an API key.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ApiKeyCreateDto dto)
        {
            _logger.LogInformation("Update called for id {Id}", id);
            try {
                var key = await _service.GetByIdAsync(id);
                if (key == null) {
                    _logger.LogWarning("Update: NotFound for id {Id}", id);
                    return NotFound();
                }
                if (!await _authz.CanManageSolutionAsync(key.SolutionId)) {
                    _logger.LogWarning("Update: Forbid for SolutionId {SolutionId}", key.SolutionId);
                    return Forbid();
                }
                key.Scope = dto.Scope;
                await _service.UpdateAsync(id, key);
                _logger.LogInformation("Update: ApiKey {Id} updated", id);
                return NoContent();
            } catch (Exception ex) {
                _logger.LogError(ex, "Error in Update for id {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete an API key.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Delete called for id {Id}", id);
            try {
                var key = await _service.GetByIdAsync(id);
                if (key == null) {
                    _logger.LogWarning("Delete: NotFound for id {Id}", id);
                    return NotFound();
                }
                if (!await _authz.CanManageSolutionAsync(key.SolutionId)) {
                    _logger.LogWarning("Delete: Forbid for SolutionId {SolutionId}", key.SolutionId);
                    return Forbid();
                }
                var success = await _service.DeleteAsync(id);
                if (!success) {
                    _logger.LogWarning("Delete: DeleteAsync failed for id {Id}", id);
                    return NotFound();
                }
                _logger.LogInformation("Delete: ApiKey {Id} deleted", id);
                return NoContent();
            } catch (Exception ex) {
                _logger.LogError(ex, "Error in Delete for id {Id}", id);
                throw;
            }
        }
    }
}
