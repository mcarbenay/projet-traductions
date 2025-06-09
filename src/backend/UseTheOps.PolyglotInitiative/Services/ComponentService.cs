using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UseTheOps.PolyglotInitiative.Data;
using UseTheOps.PolyglotInitiative.Models;

namespace UseTheOps.PolyglotInitiative.Services
{
    public class ComponentService
    {
        private readonly PolyglotInitiativeDbContext _db;
        public ComponentService(PolyglotInitiativeDbContext db) { _db = db; }

        public async Task<List<Component>> GetAllAsync() => await _db.Components.Include(c => c.ResourceFiles).ToListAsync();
        public async Task<Component?> GetByIdAsync(Guid id) => await _db.Components.Include(c => c.ResourceFiles).FirstOrDefaultAsync(c => c.Id == id);
        public async Task<Component> CreateAsync(Component component) { _db.Components.Add(component); await _db.SaveChangesAsync(); return component; }
        public async Task<bool> UpdateAsync(Guid id, Component component) { if (id != component.Id) return false; _db.Entry(component).State = EntityState.Modified; await _db.SaveChangesAsync(); return true; }
        public async Task<bool> DeleteAsync(Guid id) { var c = await _db.Components.FindAsync(id); if (c == null) return false; _db.Components.Remove(c); await _db.SaveChangesAsync(); return true; }

        public async Task<List<Component>> GetByProjectAsync(Guid projectId)
        {
            return await _db.Components
                .Where(c => c.ProjectId == projectId)
                .Include(c => c.ResourceFiles)
                .ToListAsync();
        }

        public async Task<List<Component>> GetBySolutionAsync(Guid solutionId)
        {
            return await _db.Components
                .Join(_db.Projects,
                      c => c.ProjectId,
                      p => p.Id,
                      (c, p) => new { c, p })
                .Where(x => x.p.SolutionId == solutionId)
                .Select(x => x.c)
                .Include(c => c.ResourceFiles)
                .ToListAsync();
        }
    }
}
