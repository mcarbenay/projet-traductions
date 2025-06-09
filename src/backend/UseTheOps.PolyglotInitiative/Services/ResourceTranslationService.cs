using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UseTheOps.PolyglotInitiative.Data;
using UseTheOps.PolyglotInitiative.Models;

namespace UseTheOps.PolyglotInitiative.Services
{
    /// <summary>
    /// Service for managing resource translations and related business logic.
    /// </summary>
    public class ResourceTranslationService
    {
        private readonly PolyglotInitiativeDbContext _db;
        public ResourceTranslationService(PolyglotInitiativeDbContext db) { _db = db; }

        /// <summary>
        /// Retrieves all resource translations from the database.
        /// </summary>
        public async Task<List<ResourceTranslation>> GetAllAsync() => await _db.ResourceTranslations.ToListAsync();

        /// <summary>
        /// Retrieves a specific resource translation by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the resource translation.</param>
        public async Task<ResourceTranslation?> GetByIdAsync(Guid id) => await _db.ResourceTranslations.FindAsync(id);

        /// <summary>
        /// Creates a new resource translation and saves it to the database.
        /// </summary>
        /// <param name="translation">The resource translation to create.</param>
        public async Task<ResourceTranslation> CreateAsync(ResourceTranslation translation) { _db.ResourceTranslations.Add(translation); await _db.SaveChangesAsync(); return translation; }

        /// <summary>
        /// Updates an existing resource translation in the database.
        /// </summary>
        /// <param name="id">The unique identifier of the resource translation to update.</param>
        /// <param name="translation">The updated resource translation data.</param>
        public async Task<bool> UpdateAsync(Guid id, ResourceTranslation translation) { if (id != translation.Id) return false; _db.Entry(translation).State = EntityState.Modified; await _db.SaveChangesAsync(); return true; }

        /// <summary>
        /// Deletes a resource translation from the database.
        /// </summary>
        /// <param name="id">The unique identifier of the resource translation to delete.</param>
        public async Task<bool> DeleteAsync(Guid id) { var t = await _db.ResourceTranslations.FindAsync(id); if (t == null) return false; _db.ResourceTranslations.Remove(t); await _db.SaveChangesAsync(); return true; }
    }
}
