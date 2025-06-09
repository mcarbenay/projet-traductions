using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UseTheOps.PolyglotInitiative.Data;
using UseTheOps.PolyglotInitiative.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading;
using System.Linq;
using UseTheOps.PolyglotInitiative.Services;

namespace UseTheOps.PolyglotInitiative.Services
{
    /// <summary>
    /// Service for managing resource files and related business logic.
    /// </summary>
    public partial class ResourceFileService
    {
        private readonly PolyglotInitiativeDbContext _db;
        private readonly List<ITranslationFileParser> _parsers;
        private readonly List<ITranslationFileExporter> _exporters;
        public ResourceFileService(PolyglotInitiativeDbContext db)
        {
            _db = db;
            _parsers = new List<ITranslationFileParser>
            {
                new ResxTranslationFileParser(),
                new JsonTranslationFileParser(),
                new PoTranslationFileParser(),
                new XliffTranslationFileParser()
            };
            _exporters = new List<ITranslationFileExporter>
            {
                new ResxTranslationFileExporter(),
                new JsonTranslationFileExporter(),
                new PoTranslationFileExporter(),
                new XliffTranslationFileExporter()
            };
        }

        /// <summary>
        /// Gets all resource files.
        /// </summary>
        public async Task<List<ResourceFile>> GetAllAsync() => await _db.ResourceFiles.Include(rf => rf.TranslatableResources).ToListAsync();
        /// <summary>
        /// Gets a specific resource file by ID.
        /// </summary>
        public async Task<ResourceFile?> GetByIdAsync(Guid id) => await _db.ResourceFiles.Include(rf => rf.TranslatableResources).FirstOrDefaultAsync(rf => rf.Id == id);
        /// <summary>
        /// Creates a new resource file entry.
        /// </summary>
        public async Task<ResourceFile> CreateAsync(ResourceFile file) { _db.ResourceFiles.Add(file); await _db.SaveChangesAsync(); return file; }
        /// <summary>
        /// Updates an existing resource file.
        /// </summary>
        public async Task<bool> UpdateAsync(Guid id, ResourceFile file) { if (id != file.Id) return false; _db.Entry(file).State = EntityState.Modified; await _db.SaveChangesAsync(); return true; }
        /// <summary>
        /// Deletes a resource file.
        /// </summary>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var f = await _db.ResourceFiles.FindAsync(id);
            if (f == null) return false;
            _db.ResourceFiles.Remove(f);
            await _db.SaveChangesAsync();
            return true;
        }

        public class FileUploadResult
        {
            public bool Success { get; set; }
            public string? Error { get; set; }
            public ResourceFile? ResourceFile { get; set; }
        }

        public class FileDownloadResult
        {
            public Stream Stream { get; set; } = null!;
            public string ContentType { get; set; } = "application/octet-stream";
            public string FileName { get; set; } = string.Empty;
        }

        /// <summary>
        /// Uploads a file for a specific component and language.
        /// </summary>
        public async Task<FileUploadResult> UploadFileAsync(Guid componentId, string? language, IFormFile file, CancellationToken cancellationToken = default)
        {
            var component = await _db.Components.Include(c => c.Project).FirstOrDefaultAsync(c => c.Id == componentId, cancellationToken);
            if (component == null)
                return new FileUploadResult { Success = false, Error = "Component not found." };

            string fileName = Path.GetFileName(file.FileName);
            string ext = Path.GetExtension(fileName).ToLowerInvariant();
            var parser = _parsers.FirstOrDefault(p => p.CanParse(ext));
            if (parser == null)
                return new FileUploadResult { Success = false, Error = "Unsupported file format." };

            // Uniqueness: name+component
            bool exists = await _db.ResourceFiles.AnyAsync(rf => rf.Name == fileName && rf.ComponentId == componentId, cancellationToken);
            if (exists)
                return new FileUploadResult { Success = false, Error = "A file with this name already exists for this component." };

            // Parse file (do not store)
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms, cancellationToken);
            ms.Position = 0;
            TranslationFileParseResult parseResult;
            try
            {
                parseResult = await parser.ParseAsync(ms);
            }
            catch (Exception ex)
            {
                return new FileUploadResult { Success = false, Error = $"File parsing failed: {ex.Message}" };
            }
            if (parseResult.Entries.Count == 0)
                return new FileUploadResult { Success = false, Error = "No translation entries found in file." };

            // Create ResourceFile entry (no file path)
            var resourceFile = new ResourceFile
            {
                Id = Guid.NewGuid(),
                Name = fileName,
                Path = string.Empty, // No file storage
                ProjectId = component.ProjectId,
                ComponentId = componentId,
            };
            _db.ResourceFiles.Add(resourceFile);
            await _db.SaveChangesAsync(cancellationToken);

            // For each unique key, create a TranslatableResource
            var grouped = parseResult.Entries.GroupBy(e => e.Key);
            foreach (var group in grouped)
            {
                var first = group.First();
                var tr = new TranslatableResource
                {
                    Id = Guid.NewGuid(),
                    ResourceFileId = resourceFile.Id,
                    Key = group.Key,
                    SourceValue = first.Value,
                };
                _db.TranslatableResources.Add(tr);
                // For each language, create a ResourceTranslation
                foreach (var entry in group)
                {
                    if (!string.IsNullOrEmpty(entry.Language))
                    {
                        // Find or create TranslationNeed for this language
                        var tn = await _db.TranslationNeeds.FirstOrDefaultAsync(t => t.Code == entry.Language && t.SolutionId == component.Project.SolutionId, cancellationToken);
                        if (tn == null)
                        {
                            tn = new Models.TranslationNeed
                            {
                                Id = Guid.NewGuid(),
                                Code = entry.Language!,
                                Label = entry.Language!,
                                IsDefault = false,
                                SolutionId = component.Project.SolutionId
                            };
                            _db.TranslationNeeds.Add(tn);
                            await _db.SaveChangesAsync(cancellationToken);
                        }
                        var rt = new ResourceTranslation
                        {
                            Id = Guid.NewGuid(),
                            TranslatableResourceId = tr.Id,
                            TranslationNeedId = tn.Id,
                            ValidatedValue = entry.Value,
                            Status = "imported",
                            LastModifiedDate = DateTime.UtcNow
                        };
                        _db.ResourceTranslations.Add(rt);
                    }
                }
            }
            await _db.SaveChangesAsync(cancellationToken);

            return new FileUploadResult { Success = true, ResourceFile = resourceFile };
        }

        /// <summary>
        /// Downloads a resource file in the specified format and language.
        /// </summary>
        public async Task<FileDownloadResult?> DownloadFileAsync(Guid resourceFileId, string? format = null, string? language = null)
        {
            var resourceFile = await _db.ResourceFiles
                .Include(rf => rf.TranslatableResources)
                    .ThenInclude(tr => tr.ResourceTranslations)
                .FirstOrDefaultAsync(rf => rf.Id == resourceFileId);
            if (resourceFile == null)
                return null;

            var resources = resourceFile.TranslatableResources.ToList();
            var translations = resources.SelectMany(r => r.ResourceTranslations).ToList();

            // Determine export format
            string ext = format ?? ".resx";
            var exporter = _exporters.FirstOrDefault(e => e.FileExtension.Equals(ext, StringComparison.OrdinalIgnoreCase));
            if (exporter == null)
                return null;

            // Determine language
            string lang = language ?? translations.FirstOrDefault()?.TranslationNeed?.Code ?? "en";

            var bytes = await exporter.ExportAsync(resources, translations, lang);
            var stream = new MemoryStream(bytes);
            return new FileDownloadResult
            {
                Stream = stream,
                ContentType = GetContentType("file" + ext),
                FileName = resourceFile.Name.Replace(Path.GetExtension(resourceFile.Name), ext)
            };
        }

        private bool ValidateFileFormat(string fileName, IFormFile file)
        {
            string ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext == ".resx" || ext == ".json" || ext == ".po" || ext == ".xliff";
        }

        private string GetContentType(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".resx" => "application/xml",
                ".json" => "application/json",
                ".po" => "text/plain",
                ".xliff" => "application/xml",
                _ => "application/octet-stream"
            };
        }
    }
}
