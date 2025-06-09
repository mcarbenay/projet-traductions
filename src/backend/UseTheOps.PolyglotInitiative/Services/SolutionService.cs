using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UseTheOps.PolyglotInitiative.Data;
using UseTheOps.PolyglotInitiative.Models;

namespace UseTheOps.PolyglotInitiative.Services
{
    public class SolutionService
    {
        private readonly PolyglotInitiativeDbContext _db;
        public SolutionService(PolyglotInitiativeDbContext db) { _db = db; }

        public async Task<List<Solution>> GetAllAsync() => await _db.Solutions.Include(s => s.Projects).ToListAsync();
        public async Task<Solution?> GetByIdAsync(Guid id) => await _db.Solutions.Include(s => s.Projects).FirstOrDefaultAsync(s => s.Id == id);
        public async Task<Solution> CreateAsync(Solution solution) { _db.Solutions.Add(solution); await _db.SaveChangesAsync(); return solution; }
        public async Task<bool> UpdateAsync(Guid id, Solution solution) { if (id != solution.Id) return false; _db.Entry(solution).State = EntityState.Modified; await _db.SaveChangesAsync(); return true; }
        public async Task<bool> DeleteAsync(Guid id) { var s = await _db.Solutions.FindAsync(id); if (s == null) return false; _db.Solutions.Remove(s); await _db.SaveChangesAsync(); return true; }
    }
}
