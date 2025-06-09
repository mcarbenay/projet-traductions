using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UseTheOps.PolyglotInitiative.Data;
using UseTheOps.PolyglotInitiative.Models;

namespace UseTheOps.PolyglotInitiative.Services
{
    /// <summary>
    /// Service for managing translation needs (languages/variants) and related business logic.
    /// </summary>
    public class TranslationNeedService
    {
        private readonly PolyglotInitiativeDbContext _db;
        public TranslationNeedService(PolyglotInitiativeDbContext db) { _db = db; }

        /// <summary>
        /// Get all translation needs.
        /// </summary>
        public async Task<List<TranslationNeed>> GetAllAsync() => await _db.TranslationNeeds.ToListAsync();

        /// <summary>
        /// Get a specific translation need by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the translation need.</param>
        public async Task<TranslationNeed?> GetByIdAsync(Guid id) => await _db.TranslationNeeds.FindAsync(id);

        /// <summary>
        /// Create a new translation need.
        /// </summary>
        /// <param name="need">The translation need to create.</param>
        public async Task<TranslationNeed> CreateAsync(TranslationNeed need) { _db.TranslationNeeds.Add(need); await _db.SaveChangesAsync(); return need; }

        /// <summary>
        /// Update an existing translation need.
        /// </summary>
        /// <param name="id">The identifier of the translation need to update.</param>
        /// <param name="need">The updated translation need data.</param>
        public async Task<bool> UpdateAsync(Guid id, TranslationNeed need) { if (id != need.Id) return false; _db.Entry(need).State = EntityState.Modified; await _db.SaveChangesAsync(); return true; }

        /// <summary>
        /// Delete a translation need.
        /// </summary>
        /// <param name="id">The identifier of the translation need to delete.</param>
        public async Task<bool> DeleteAsync(Guid id) { var n = await _db.TranslationNeeds.FindAsync(id); if (n == null) return false; _db.TranslationNeeds.Remove(n); await _db.SaveChangesAsync(); return true; }

        /// <summary>
        /// Get translation needs associated with a specific file.
        /// </summary>
        /// <param name="resourceFileId">The identifier of the resource file.</param>
        public async Task<List<TranslationNeed>> GetByFileAsync(Guid resourceFileId)
        {
            return await _db.ResourceTranslations
                .Where(rt => rt.TranslatableResourceId != Guid.Empty)
                .Join(_db.TranslatableResources,
                      rt => rt.TranslatableResourceId,
                      tr => tr.Id,
                      (rt, tr) => new { rt, tr })
                .Where(x => x.tr.ResourceFileId == resourceFileId)
                .Select(x => x.rt.TranslationNeed)
                .Distinct()
                .ToListAsync();
        }

        /// <summary>
        /// Get translation needs with a pending status based on solution, project, or file.
        /// </summary>
        /// <param name="solutionId">The identifier of the solution.</param>
        /// <param name="projectId">The identifier of the project.</param>
        /// <param name="resourceFileId">The identifier of the resource file.</param>
        public async Task<List<TranslationNeed>> GetWithPendingAsync(Guid? solutionId, Guid? projectId, Guid? resourceFileId)
        {
            var query = _db.ResourceTranslations.AsQueryable();

            if (resourceFileId.HasValue)
            {
                query = query.Join(_db.TranslatableResources,
                                   rt => rt.TranslatableResourceId,
                                   tr => tr.Id,
                                   (rt, tr) => new { rt, tr })
                             .Where(x => x.tr.ResourceFileId == resourceFileId.Value)
                             .Select(x => x.rt);
            }
            else if (projectId.HasValue)
            {
                query = query.Join(_db.TranslatableResources,
                                   rt => rt.TranslatableResourceId,
                                   tr => tr.Id,
                                   (rt, tr) => new { rt, tr })
                             .Join(_db.ResourceFiles,
                                   x => x.tr.ResourceFileId,
                                   rf => rf.Id,
                                   (x, rf) => new { x.rt, rf })
                             .Where(x => x.rf.ProjectId == projectId.Value)
                             .Select(x => x.rt);
            }
            else if (solutionId.HasValue)
            {
                query = query.Join(_db.TranslatableResources,
                                   rt => rt.TranslatableResourceId,
                                   tr => tr.Id,
                                   (rt, tr) => new { rt, tr })
                             .Join(_db.ResourceFiles,
                                   x => x.tr.ResourceFileId,
                                   rf => rf.Id,
                                   (x, rf) => new { x.rt, rf })
                             .Join(_db.Projects,
                                   x => x.rf.ProjectId,
                                   p => p.Id,
                                   (x, p) => new { x.rt, p })
                             .Where(x => x.p.SolutionId == solutionId.Value)
                             .Select(x => x.rt);
            }

            var pendingNeeds = await query
                .Where(rt => rt.Status == "pending" || rt.Status == "suggested")
                .Select(rt => rt.TranslationNeed)
                .Distinct()
                .ToListAsync();
            return pendingNeeds;
        }
    }
}
