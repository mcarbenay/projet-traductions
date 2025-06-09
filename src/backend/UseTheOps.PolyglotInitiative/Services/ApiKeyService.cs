using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UseTheOps.PolyglotInitiative.Data;
using UseTheOps.PolyglotInitiative.Models;

namespace UseTheOps.PolyglotInitiative.Services
{
    /// <summary>
    /// Service for managing API keys and related business logic.
    /// </summary>
    public class ApiKeyService
    {
        private readonly PolyglotInitiativeDbContext _db;
        public ApiKeyService(PolyglotInitiativeDbContext db) { _db = db; }

        /// <summary>
        /// Retrieves all API keys from the database.
        /// </summary>
        public async Task<List<ApiKey>> GetAllAsync() => await _db.ApiKeys.ToListAsync();

        /// <summary>
        /// Retrieves a specific API key by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the API key.</param>
        public async Task<ApiKey?> GetByIdAsync(Guid id) => await _db.ApiKeys.FindAsync(id);

        /// <summary>
        /// Creates a new API key and saves it to the database.
        /// </summary>
        /// <param name="key">The API key to be created.</param>
        public async Task<ApiKey> CreateAsync(ApiKey key) { _db.ApiKeys.Add(key); await _db.SaveChangesAsync(); return key; }

        /// <summary>
        /// Updates an existing API key in the database.
        /// </summary>
        /// <param name="id">The unique identifier of the API key to be updated.</param>
        /// <param name="key">The updated API key data.</param>
        public async Task<bool> UpdateAsync(Guid id, ApiKey key) {
            var existing = await _db.ApiKeys.FindAsync(id);
            if (existing == null) return false;
            existing.Scope = key.Scope;
            await _db.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Deletes an API key from the database.
        /// </summary>
        /// <param name="id">The unique identifier of the API key to be deleted.</param>
        public async Task<bool> DeleteAsync(Guid id) { var k = await _db.ApiKeys.FindAsync(id); if (k == null) return false; _db.ApiKeys.Remove(k); await _db.SaveChangesAsync(); return true; }
    }
}
