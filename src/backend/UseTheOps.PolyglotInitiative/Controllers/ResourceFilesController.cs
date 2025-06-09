using Microsoft.AspNetCore.Mvc;
using UseTheOps.PolyglotInitiative.Models;
using UseTheOps.PolyglotInitiative.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace UseTheOps.PolyglotInitiative.Controllers
{
    /// <summary>
    /// API endpoints for managing resource files.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for all endpoints by default
    public class ResourceFilesController : ControllerBase
    {
        private readonly ResourceFileService _service;
        private readonly AuthorizationService _authz;
        public ResourceFilesController(ResourceFileService service, AuthorizationService authz)
        {
            _service = service;
            _authz = authz;
        }

        /// <summary>
        /// Get all resource files.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResourceFile>>> GetAll()
        {
            var files = await _service.GetAllAsync();
            return Ok(files);
        }

        /// <summary>
        /// Get a resource file by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ResourceFile>> Get(Guid id)
        {
            var file = await _service.GetByIdAsync(id);
            if (file == null) return NotFound();
            return Ok(file);
        }

        /// <summary>
        /// Create a new resource file.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ResourceFile>> Create(ResourceFile file)
        {
            var created = await _service.CreateAsync(file);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update a resource file.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ResourceFile file)
        {
            if (!await _authz.CanEditFileAsync(id))
                return Forbid();
            var success = await _service.UpdateAsync(id, file);
            if (!success) return BadRequest();
            return NoContent();
        }

        /// <summary>
        /// Delete a resource file.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!await _authz.CanEditFileAsync(id))
                return Forbid();
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Upload a translation file (creates a new version).
        /// </summary>
        /// <param name="componentId">Component ID</param>
        /// <param name="language">Language code (optional)</param>
        /// <param name="file">The file to upload</param>
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromQuery] Guid componentId, [FromQuery] string? language, IFormFile file)
        {
            if (!await _authz.CanManageComponentAsync(componentId))
                return Forbid();
            if (file == null || file.Length == 0) return BadRequest("No file provided.");
            var result = await _service.UploadFileAsync(componentId, language, file);
            if (!result.Success) return BadRequest(result.Error);
            return Ok(result.ResourceFile);
        }

        /// <summary>
        /// Download a translation file (latest version, selectable format and language).
        /// </summary>
        /// <param name="resourceFileId">Resource file ID</param>
        /// <param name="format">Export format (e.g. .resx, .json, .po, .xliff)</param>
        /// <param name="language">Language code (optional)</param>
        [HttpGet("download/{resourceFileId}")]
        public async Task<IActionResult> Download(Guid resourceFileId, [FromQuery] string? format, [FromQuery] string? language)
        {
            if (!await _authz.CanEditFileAsync(resourceFileId))
                return Forbid();
            var fileResult = await _service.DownloadFileAsync(resourceFileId, format, language);
            if (fileResult == null) return NotFound();
            return File(fileResult.Stream, fileResult.ContentType, fileResult.FileName);
        }

        /// <summary>
        /// Get the list of supported export file formats.
        /// </summary>
        [HttpGet("supported-formats")]
        public ActionResult<IEnumerable<string>> GetSupportedFormats()
        {
            // This should match the extensions in ResourceFileService exporters
            var formats = new List<string> { ".resx", ".json", ".po", ".xliff" };
            return Ok(formats);
        }

        /// <summary>
        /// Get the list of languages with at least one validated translation for a given resource file.
        /// </summary>
        /// <param name="resourceFileId">Resource file ID</param>
        [HttpGet("{resourceFileId}/languages-with-validated")]
        public async Task<ActionResult<IEnumerable<string>>> GetLanguagesWithValidatedTranslations(Guid resourceFileId)
        {
            var file = await _service.GetByIdAsync(resourceFileId);
            if (file == null) return NotFound();
            var languages = file.TranslatableResources
                .SelectMany(r => r.ResourceTranslations)
                .Where(t => !string.IsNullOrEmpty(t.ValidatedValue))
                .Select(t => t.TranslationNeed?.Code)
                .Where(code => !string.IsNullOrEmpty(code))
                .Distinct()
                .ToList();
            return Ok(languages);
        }
    }
}
