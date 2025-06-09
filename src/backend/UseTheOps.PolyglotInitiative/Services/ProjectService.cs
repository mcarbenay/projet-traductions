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
    /// Service for managing projects and enforcing business rules.
    /// </summary>
    public class ProjectService
    {
        private readonly PolyglotInitiativeDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectService"/> class.
        /// </summary>
        /// <param name="db">The database context.</param>
        public ProjectService(PolyglotInitiativeDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Gets all projects, including their components.
        /// </summary>
        /// <returns>A list of all projects with their components.</returns>
        public async Task<List<Project>> GetAllAsync()
        {
            return await _db.Projects.Include(p => p.Components).ToListAsync();
        }

        /// <summary>
        /// Gets a project by its unique identifier, including its components.
        /// </summary>
        /// <param name="id">The project ID.</param>
        /// <returns>The project if found, otherwise null.</returns>
        public async Task<Project?> GetByIdAsync(Guid id)
        {
            return await _db.Projects.Include(p => p.Components).FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// Creates a new project after checking business rules.
        /// </summary>
        /// <param name="project">The project to create.</param>
        /// <returns>A tuple indicating success, error message, and the created project.</returns>
        public async Task<(bool Success, string? Error, Project? Project)> CreateAsync(Project project)
        {
            // Check if the solution exists
            var solution = await _db.Solutions.FindAsync(project.SolutionId);
            if (solution == null)
                return (false, "Solution does not exist.", null);
            // Ensure code and name are unique within the solution
            bool codeExists = await _db.Projects.AnyAsync(p => p.SolutionId == project.SolutionId && p.Code == project.Code);
            if (codeExists)
                return (false, "A project with this code already exists in the solution.", null);
            bool nameExists = await _db.Projects.AnyAsync(p => p.SolutionId == project.SolutionId && p.Name == project.Name);
            if (nameExists)
                return (false, "A project with this name already exists in the solution.", null);
            _db.Projects.Add(project);
            await _db.SaveChangesAsync();
            return (true, null, project);
        }

        /// <summary>
        /// Updates an existing project after checking business rules.
        /// </summary>
        /// <param name="id">The project ID.</param>
        /// <param name="project">The updated project entity.</param>
        /// <returns>A tuple indicating success and error message if any.</returns>
        public async Task<(bool Success, string? Error)> UpdateAsync(Guid id, Project project)
        {
            if (id != project.Id)
                return (false, "ID mismatch.");
            var existing = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (existing == null)
                return (false, "Project not found.");
            // Ensure code and name are unique within the solution (excluding self)
            bool codeExists = await _db.Projects.AnyAsync(p => p.SolutionId == project.SolutionId && p.Code == project.Code && p.Id != id);
            if (codeExists)
                return (false, "A project with this code already exists in the solution.");
            bool nameExists = await _db.Projects.AnyAsync(p => p.SolutionId == project.SolutionId && p.Name == project.Name && p.Id != id);
            if (nameExists)
                return (false, "A project with this name already exists in the solution.");
            _db.Entry(project).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return (true, null);
        }

        /// <summary>
        /// Deletes a project by its unique identifier.
        /// </summary>
        /// <param name="id">The project ID.</param>
        /// <returns>A tuple indicating success and error message if any.</returns>
        public async Task<(bool Success, string? Error)> DeleteAsync(Guid id)
        {
            var project = await _db.Projects.FindAsync(id);
            if (project == null)
                return (false, "Project not found.");
            _db.Projects.Remove(project);
            await _db.SaveChangesAsync();
            return (true, null);
        }

        /// <summary>
        /// Gets all projects for a given solution, including their components.
        /// </summary>
        /// <param name="solutionId">The solution ID.</param>
        /// <returns>A list of projects for the solution.</returns>
        public async Task<List<Project>> GetBySolutionWithComponentsAsync(Guid solutionId)
        {
            return await _db.Projects
                .AsQueryable()
                .Where(p => p.SolutionId == solutionId)
                .Include(p => p.Components)
                .ToListAsync();
        }
    }
}
