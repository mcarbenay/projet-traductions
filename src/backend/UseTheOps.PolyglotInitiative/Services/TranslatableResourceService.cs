using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UseTheOps.PolyglotInitiative.Data;
using UseTheOps.PolyglotInitiative.Models;

namespace UseTheOps.PolyglotInitiative.Services
{
    /// <summary>
    /// Service for managing translatable resources and related business logic.
    /// </summary>
    public class TranslatableResourceService
    {
        private readonly PolyglotInitiativeDbContext _db;
        public TranslatableResourceService(PolyglotInitiativeDbContext db) { _db = db; }

        /// <summary>
        /// Retrieves all translatable resources.
        /// </summary>
        public async Task<List<TranslatableResource>> GetAllAsync() => await _db.TranslatableResources.Include(tr => tr.ResourceTranslations).ToListAsync();

        /// <summary>
        /// Retrieves a specific translatable resource by its identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the resource.</param>
        public async Task<TranslatableResource?> GetByIdAsync(Guid id) => await _db.TranslatableResources.Include(tr => tr.ResourceTranslations).FirstOrDefaultAsync(tr => tr.Id == id);

        /// <summary>
        /// Creates a new translatable resource.
        /// </summary>
        /// <param name="resource">The resource to be created.</param>
        public async Task<TranslatableResource> CreateAsync(TranslatableResource resource) { _db.TranslatableResources.Add(resource); await _db.SaveChangesAsync(); return resource; }

        /// <summary>
        /// Updates an existing translatable resource.
        /// </summary>
        /// <param name="id">The unique identifier of the resource.</param>
        /// <param name="resource">The updated resource data.</param>
        public async Task<bool> UpdateAsync(Guid id, TranslatableResource resource) { if (id != resource.Id) return false; _db.Entry(resource).State = EntityState.Modified; await _db.SaveChangesAsync(); return true; }

        /// <summary>
        /// Deletes a translatable resource.
        /// </summary>
        /// <param name="id">The unique identifier of the resource.</param>
        public async Task<bool> DeleteAsync(Guid id) { var r = await _db.TranslatableResources.FindAsync(id); if (r == null) return false; _db.TranslatableResources.Remove(r); await _db.SaveChangesAsync(); return true; }
    }
}
