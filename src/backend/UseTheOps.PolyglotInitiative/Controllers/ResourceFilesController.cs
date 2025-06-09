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
using Microsoft.Extensions.Logging;
using UseTheOps.PolyglotInitiative.Helpers;

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
        private readonly ILogger<ResourceFilesController> _logger;
        public ResourceFilesController(ResourceFileService service, AuthorizationService authz, ILogger<ResourceFilesController> logger)
        {
            _service = service;
            _authz = authz;
            _logger = logger;
        }

        /// <summary>
        /// Get all resource files.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResourceFile>>> GetAll()
        {
            _logger.LogInformation("Getting all resource files.");
            var files = await _service.GetAllAsync();
            _logger.LogInformation($"Retrieved {files.Count()} resource files.");
            return Ok(files);
        }

        /// <summary>
        /// Get a resource file by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            _logger.LogInformation($"Getting resource file with ID {id}.");
            try
            {
                var file = await _service.GetByIdAsync(id);
                if (file == null)
                {
                    _logger.LogWarning($"Resource file with ID {id} not found.");
                    return ExceptionHelper.ToActionResult(new KeyNotFoundException($"Resource file not found: {id}"), this, nameof(Get));
                }
                _logger.LogInformation($"Retrieved resource file with ID {id}.");
                return Ok(file);
            }
            catch (Exception ex)
            {
                return ExceptionHelper.ToActionResult(ex, this, nameof(Get));
            }
        }

        /// <summary>
        /// Create a new resource file.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ResourceFile>> Create(ResourceFile file)
        {
            _logger.LogInformation("Creating a new resource file.");
            var created = await _service.CreateAsync(file);
            _logger.LogInformation($"Created resource file with ID {created.Id}.");
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update a resource file.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ResourceFile file)
        {
            if (!await _authz.CanEditFileAsync(id))
            {
                _logger.LogWarning($"Unauthorized update attempt for resource file ID {id}.");
                return ExceptionHelper.ToActionResult(new UnauthorizedAccessException($"Unauthorized update attempt for resource file: {id}"), this, nameof(Update));
            }
            _logger.LogInformation($"Updating resource file with ID {id}.");
            try
            {
                var success = await _service.UpdateAsync(id, file);
                if (!success)
                {
                    _logger.LogError($"Error updating resource file with ID {id}.");
                    return ExceptionHelper.ToActionResult(new ArgumentException($"Error updating resource file: {id}"), this, nameof(Update));
                }
                _logger.LogInformation($"Updated resource file with ID {id}.");
                return NoContent();
            }
            catch (Exception ex)
            {
                return ExceptionHelper.ToActionResult(ex, this, nameof(Update));
            }
        }

        /// <summary>
        /// Delete a resource file.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!await _authz.CanEditFileAsync(id))
            {
                _logger.LogWarning($"Unauthorized delete attempt for resource file ID {id}.");
                return ExceptionHelper.ToActionResult(new UnauthorizedAccessException($"Unauthorized delete attempt for resource file: {id}"), this, nameof(Delete));
            }
            _logger.LogInformation($"Deleting resource file with ID {id}.");
            try
            {
                var success = await _service.DeleteAsync(id);
                if (!success)
                {
                    _logger.LogError($"Error deleting resource file with ID {id}.");
                    return ExceptionHelper.ToActionResult(new KeyNotFoundException($"Resource file not found: {id}"), this, nameof(Delete));
                }
                _logger.LogInformation($"Deleted resource file with ID {id}.");
                return NoContent();
            }
            catch (Exception ex)
            {
                return ExceptionHelper.ToActionResult(ex, this, nameof(Delete));
            }
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
            {
                _logger.LogWarning($"Unauthorized upload attempt for component ID {componentId}.");
                return ExceptionHelper.ToActionResult(new UnauthorizedAccessException($"Unauthorized upload attempt for component: {componentId}"), this, nameof(Upload));
            }
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Upload attempt with no file provided.");
                return ExceptionHelper.ToActionResult(new ArgumentException("No file provided."), this, nameof(Upload));
            }
            _logger.LogInformation($"Uploading file for component ID {componentId}, language {language}.");
            try
            {
                var result = await _service.UploadFileAsync(componentId, language, file);
                if (!result.Success || result.ResourceFile == null)
                {
                    _logger.LogError($"Error uploading file: {result.Error}");
                    return ExceptionHelper.ToActionResult(new ArgumentException(result.Error ?? "Upload failed"), this, nameof(Upload));
                }
                _logger.LogInformation($"Uploaded file for component ID {componentId}, language {language}.");
                var dto = new ResourceFileDto {
                    Id = result.ResourceFile.Id,
                    Name = result.ResourceFile.Name,
                    ComponentId = result.ResourceFile.ComponentId,
                    ProjectId = result.ResourceFile.ProjectId
                };
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return ExceptionHelper.ToActionResult(ex, this, nameof(Upload));
            }
        }

        private class ResourceFileDto
        {
            public Guid? Id { get; set; }
            public string? Name { get; set; }
            public Guid? ComponentId { get; set; }
            public Guid? ProjectId { get; set; }
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
            {
                _logger.LogWarning($"Unauthorized download attempt for resource file ID {resourceFileId}.");
                return Forbid();
            }
            _logger.LogInformation($"Downloading resource file ID {resourceFileId}, format {format}, language {language}.");
            var fileResult = await _service.DownloadFileAsync(resourceFileId, format, language);
            if (fileResult == null) 
            {
                _logger.LogWarning($"Resource file ID {resourceFileId} not found for download.");
                return NotFound();
            }
            _logger.LogInformation($"Downloaded resource file ID {resourceFileId}.");
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
            _logger.LogInformation("Getting supported export file formats.");
            return Ok(formats);
        }

        /// <summary>
        /// Get the list of languages with at least one validated translation for a given resource file.
        /// </summary>
        /// <param name="resourceFileId">Resource file ID</param>
        [HttpGet("{resourceFileId}/languages-with-validated")]
        public async Task<ActionResult<IEnumerable<string>>> GetLanguagesWithValidatedTranslations(Guid resourceFileId)
        {
            _logger.LogInformation($"Getting languages with validated translations for resource file ID {resourceFileId}.");
            var file = await _service.GetByIdAsync(resourceFileId);
            if (file == null) 
            {
                _logger.LogWarning($"Resource file ID {resourceFileId} not found.");
                return NotFound();
            }
            var languages = file.TranslatableResources
                .SelectMany(r => r.ResourceTranslations)
                .Where(t => !string.IsNullOrEmpty(t.ValidatedValue))
                .Select(t => t.TranslationNeed?.Code)
                .Where(code => !string.IsNullOrEmpty(code))
                .Distinct()
                .ToList();
            _logger.LogInformation($"Found {languages.Count} languages with validated translations for resource file ID {resourceFileId}.");
            return Ok(languages);
        }
    }
}
