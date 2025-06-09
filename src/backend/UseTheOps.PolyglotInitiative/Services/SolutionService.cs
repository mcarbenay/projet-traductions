using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UseTheOps.PolyglotInitiative.Data;
using UseTheOps.PolyglotInitiative.Models;

namespace UseTheOps.PolyglotInitiative.Services
{
    /// <summary>
    /// Service for managing solutions and related business logic.
    /// </summary>
    public class SolutionService
    {
        private readonly PolyglotInitiativeDbContext _db;
        public SolutionService(PolyglotInitiativeDbContext db) { _db = db; }

        /// <summary>
        /// Retrieves all solutions from the database.
        /// </summary>
        public async Task<List<Solution>> GetAllAsync() => await _db.Solutions.Include(s => s.Projects).ToListAsync();

        /// <summary>
        /// Retrieves a specific solution by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the solution.</param>
        public async Task<Solution?> GetByIdAsync(Guid id) => await _db.Solutions.Include(s => s.Projects).FirstOrDefaultAsync(s => s.Id == id);

        /// <summary>
        /// Adds a new solution to the database.
        /// </summary>
        /// <param name="solution">The solution to be added.</param>
        public async Task<Solution> CreateAsync(Solution solution) { _db.Solutions.Add(solution); await _db.SaveChangesAsync(); return solution; }

        /// <summary>
        /// Updates an existing solution in the database.
        /// </summary>
        /// <param name="id">The unique identifier of the solution.</param>
        /// <param name="solution">The solution data to update.</param>
        public async Task<bool> UpdateAsync(Guid id, Solution solution) { if (id != solution.Id) return false; _db.Entry(solution).State = EntityState.Modified; await _db.SaveChangesAsync(); return true; }

        /// <summary>
        /// Removes a solution from the database.
        /// </summary>
        /// <param name="id">The unique identifier of the solution.</param>
        public async Task<bool> DeleteAsync(Guid id) { var s = await _db.Solutions.FindAsync(id); if (s == null) return false; _db.Solutions.Remove(s); await _db.SaveChangesAsync(); return true; }
    }
}
