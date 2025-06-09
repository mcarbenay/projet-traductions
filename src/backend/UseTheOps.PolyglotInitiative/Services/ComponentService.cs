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
    /// Service for managing components and related business logic.
    /// </summary>
    public class ComponentService
    {
        private readonly PolyglotInitiativeDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentService"/> class.
        /// </summary>
        /// <param name="db">The database context.</param>
        public ComponentService(PolyglotInitiativeDbContext db) { _db = db; }

        /// <summary>
        /// Gets all components, including their resource files.
        /// </summary>
        /// <returns>A list of all components with their resource files.</returns>
        public async Task<List<Component>> GetAllAsync() => await _db.Components.Include(c => c.ResourceFiles).ToListAsync();

        /// <summary>
        /// Gets a component by its unique identifier, including its resource files.
        /// </summary>
        /// <param name="id">The component ID.</param>
        /// <returns>The component if found, otherwise null.</returns>
        public async Task<Component?> GetByIdAsync(Guid id) => await _db.Components.Include(c => c.ResourceFiles).FirstOrDefaultAsync(c => c.Id == id);

        /// <summary>
        /// Creates a new component.
        /// </summary>
        /// <param name="component">The component to create.</param>
        /// <returns>The created component.</returns>
        public async Task<Component> CreateAsync(Component component) { _db.Components.Add(component); await _db.SaveChangesAsync(); return component; }

        /// <summary>
        /// Updates an existing component.
        /// </summary>
        /// <param name="id">The component ID.</param>
        /// <param name="component">The updated component entity.</param>
        /// <returns>True if update succeeded, false otherwise.</returns>
        public async Task<bool> UpdateAsync(Guid id, Component component) { if (id != component.Id) return false; _db.Entry(component).State = EntityState.Modified; await _db.SaveChangesAsync(); return true; }

        /// <summary>
        /// Deletes a component by its unique identifier.
        /// </summary>
        /// <param name="id">The component ID.</param>
        /// <returns>True if deletion succeeded, false otherwise.</returns>
        public async Task<bool> DeleteAsync(Guid id) { var c = await _db.Components.FindAsync(id); if (c == null) return false; _db.Components.Remove(c); await _db.SaveChangesAsync(); return true; }

        /// <summary>
        /// Gets all components for a given project, including their resource files.
        /// </summary>
        /// <param name="projectId">The project ID.</param>
        /// <returns>A list of components for the project.</returns>
        public async Task<List<Component>> GetByProjectAsync(Guid projectId)
        {
            return await _db.Components
                .Where(c => c.ProjectId == projectId)
                .Include(c => c.ResourceFiles)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all components for a given solution, including their resource files.
        /// </summary>
        /// <param name="solutionId">The solution ID.</param>
        /// <returns>A list of components for the solution.</returns>
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
