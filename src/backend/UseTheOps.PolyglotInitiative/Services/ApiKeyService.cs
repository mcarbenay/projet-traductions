using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UseTheOps.PolyglotInitiative.Data;
using UseTheOps.PolyglotInitiative.Models;

namespace UseTheOps.PolyglotInitiative.Services
{
    public class ApiKeyService
    {
        private readonly PolyglotInitiativeDbContext _db;
        public ApiKeyService(PolyglotInitiativeDbContext db) { _db = db; }

        public async Task<List<ApiKey>> GetAllAsync() => await _db.ApiKeys.ToListAsync();
        public async Task<ApiKey?> GetByIdAsync(Guid id) => await _db.ApiKeys.FindAsync(id);
        public async Task<ApiKey> CreateAsync(ApiKey key) { _db.ApiKeys.Add(key); await _db.SaveChangesAsync(); return key; }
        public async Task<bool> UpdateAsync(Guid id, ApiKey key) {
            var existing = await _db.ApiKeys.FindAsync(id);
            if (existing == null) return false;
            existing.Scope = key.Scope;
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteAsync(Guid id) { var k = await _db.ApiKeys.FindAsync(id); if (k == null) return false; _db.ApiKeys.Remove(k); await _db.SaveChangesAsync(); return true; }
    }
}
