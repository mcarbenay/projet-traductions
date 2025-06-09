using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UseTheOps.PolyglotInitiative.Data;
using UseTheOps.PolyglotInitiative.Models;
using Microsoft.EntityFrameworkCore;

namespace UseTheOps.PolyglotInitiative.Services
{
    public class AuthorizationService
    {
        private readonly PolyglotInitiativeDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthorizationService(PolyglotInitiativeDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public bool IsAdmin() => User?.IsInRole("Admin") ?? false;
        public bool IsProductOwner() => User?.IsInRole("ProductOwner") ?? false;
        public bool IsTranslator() => User?.IsInRole("Translator") ?? false;
        public bool IsReader() => User?.IsInRole("Reader") ?? false;
        public bool IsApiKey() => User?.HasClaim("ApiKey", "true") ?? false;

        // Vérifie si l'utilisateur (ou API key) a le droit d'effectuer une action sur une solution donnée
        public async Task<bool> CanManageSolutionAsync(Guid solutionId)
        {
            if (IsAdmin() || (IsApiKey())) return true;
            if (IsProductOwner())
            {
                var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return false;
                return await _db.UserSolutionAccesses.AnyAsync(x => x.UserId == Guid.Parse(userId) && x.SolutionId == solutionId && x.AccessLevel == "ProductOwner");
            }
            return false;
        }

        // Vérifie si l'utilisateur (ou API key) a le droit d'effectuer une action sur un projet
        public async Task<bool> CanManageProjectAsync(Guid projectId)
        {
            var project = await _db.Projects.FindAsync(projectId);
            if (project == null) return false;
            return await CanManageSolutionAsync(project.SolutionId);
        }

        // Vérifie si l'utilisateur (ou API key) a le droit d'effectuer une action sur un composant
        public async Task<bool> CanManageComponentAsync(Guid componentId)
        {
            var component = await _db.Components.FindAsync(componentId);
            if (component == null) return false;
            return await CanManageProjectAsync(component.ProjectId);
        }

        // Vérifie si l'utilisateur (ou API key) a le droit d'uploader, downloader ou modifier un fichier
        public async Task<bool> CanEditFileAsync(Guid resourceFileId)
        {
            var file = await _db.ResourceFiles.FindAsync(resourceFileId);
            if (file == null) return false;
            return await CanManageComponentAsync(file.ComponentId);
        }
    }
}
