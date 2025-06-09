using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UseTheOps.PolyglotInitiative.Data;
using UseTheOps.PolyglotInitiative.Models;

namespace UseTheOps.PolyglotInitiative.Services
{
    public class TranslatableResourceService
    {
        private readonly PolyglotInitiativeDbContext _db;
        public TranslatableResourceService(PolyglotInitiativeDbContext db) { _db = db; }

        public async Task<List<TranslatableResource>> GetAllAsync() => await _db.TranslatableResources.Include(tr => tr.ResourceTranslations).ToListAsync();
        public async Task<TranslatableResource?> GetByIdAsync(Guid id) => await _db.TranslatableResources.Include(tr => tr.ResourceTranslations).FirstOrDefaultAsync(tr => tr.Id == id);
        public async Task<TranslatableResource> CreateAsync(TranslatableResource resource) { _db.TranslatableResources.Add(resource); await _db.SaveChangesAsync(); return resource; }
        public async Task<bool> UpdateAsync(Guid id, TranslatableResource resource) { if (id != resource.Id) return false; _db.Entry(resource).State = EntityState.Modified; await _db.SaveChangesAsync(); return true; }
        public async Task<bool> DeleteAsync(Guid id) { var r = await _db.TranslatableResources.FindAsync(id); if (r == null) return false; _db.TranslatableResources.Remove(r); await _db.SaveChangesAsync(); return true; }
    }
}
