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
        public ProjectService(PolyglotInitiativeDbContext db)
        {
            _db = db;
        }

        public async Task<List<Project>> GetAllAsync()
        {
            return await _db.Projects.Include(p => p.Components).ToListAsync();
        }

        public async Task<Project?> GetByIdAsync(Guid id)
        {
            return await _db.Projects.Include(p => p.Components).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<(bool Success, string? Error, Project? Project)> CreateAsync(Project project)
        {
            var solution = await _db.Solutions.FindAsync(project.SolutionId);
            if (solution == null)
                return (false, "Solution does not exist.", null);
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

        public async Task<(bool Success, string? Error)> UpdateAsync(Guid id, Project project)
        {
            if (id != project.Id)
                return (false, "ID mismatch.");
            var existing = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (existing == null)
                return (false, "Project not found.");
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

        public async Task<(bool Success, string? Error)> DeleteAsync(Guid id)
        {
            var project = await _db.Projects.FindAsync(id);
            if (project == null)
                return (false, "Project not found.");
            _db.Projects.Remove(project);
            await _db.SaveChangesAsync();
            return (true, null);
        }

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
