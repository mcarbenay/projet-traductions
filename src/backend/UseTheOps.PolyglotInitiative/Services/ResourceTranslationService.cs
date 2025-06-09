using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UseTheOps.PolyglotInitiative.Data;
using UseTheOps.PolyglotInitiative.Models;

namespace UseTheOps.PolyglotInitiative.Services
{
    public class ResourceTranslationService
    {
        private readonly PolyglotInitiativeDbContext _db;
        public ResourceTranslationService(PolyglotInitiativeDbContext db) { _db = db; }

        public async Task<List<ResourceTranslation>> GetAllAsync() => await _db.ResourceTranslations.ToListAsync();
        public async Task<ResourceTranslation?> GetByIdAsync(Guid id) => await _db.ResourceTranslations.FindAsync(id);
        public async Task<ResourceTranslation> CreateAsync(ResourceTranslation translation) { _db.ResourceTranslations.Add(translation); await _db.SaveChangesAsync(); return translation; }
        public async Task<bool> UpdateAsync(Guid id, ResourceTranslation translation) { if (id != translation.Id) return false; _db.Entry(translation).State = EntityState.Modified; await _db.SaveChangesAsync(); return true; }
        public async Task<bool> DeleteAsync(Guid id) { var t = await _db.ResourceTranslations.FindAsync(id); if (t == null) return false; _db.ResourceTranslations.Remove(t); await _db.SaveChangesAsync(); return true; }
    }
}
