using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UseTheOps.PolyglotInitiative.Data;
using UseTheOps.PolyglotInitiative.Models;
using Microsoft.EntityFrameworkCore;

namespace UseTheOps.PolyglotInitiative.Services
{
    /// <summary>
    /// Service for handling authorization logic and access control.
    /// </summary>
    public class AuthorizationService
    {
        private readonly PolyglotInitiativeDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthorizationService(PolyglotInitiativeDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets the current user as a ClaimsPrincipal.
        /// </summary>
        public ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        /// <summary>
        /// Checks if the current user is an admin.
        /// </summary>
        public bool IsAdmin() => User?.IsInRole("Admin") ?? false;

        /// <summary>
        /// Checks if the current user is a product owner.
        /// </summary>
        public bool IsProductOwner() => User?.IsInRole("ProductOwner") ?? false;

        /// <summary>
        /// Checks if the current user is a translator.
        /// </summary>
        public bool IsTranslator() => User?.IsInRole("Translator") ?? false;

        /// <summary>
        /// Checks if the current user is a reader.
        /// </summary>
        public bool IsReader() => User?.IsInRole("Reader") ?? false;

        /// <summary>
        /// Checks if the current user is using an API key.
        /// </summary>
        public bool IsApiKey() => User?.HasClaim("ApiKey", "true") ?? false;

        /// <summary>
        /// Vérifie si l'utilisateur (ou API key) a le droit d'effectuer une action sur une solution donnée
        /// </summary>
        /// <param name="solutionId">L'identifiant de la solution</param>
        /// <returns>Vrai si l'utilisateur a le droit, sinon faux</returns>
        public async Task<bool> CanManageSolutionAsync(Guid solutionId)
        {
            if (IsAdmin()) return true; // L'admin a tous les droits
            if (IsApiKey()) return true;
            if (IsProductOwner())
            {
                var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return false;
                return await _db.UserSolutionAccesses.AnyAsync(x => x.UserId == Guid.Parse(userId) && x.SolutionId == solutionId && x.AccessLevel == "ProductOwner");
            }
            return false;
        }

        /// <summary>
        /// Vérifie si l'utilisateur (ou API key) a le droit d'effectuer une action sur un projet
        /// </summary>
        /// <param name="projectId">L'identifiant du projet</param>
        /// <returns>Vrai si l'utilisateur a le droit, sinon faux</returns>
        public async Task<bool> CanManageProjectAsync(Guid projectId)
        {
            if (IsAdmin()) return true; // L'admin a tous les droits
            var project = await _db.Projects.FindAsync(projectId);
            if (project == null) return false;
            return await CanManageSolutionAsync(project.SolutionId);
        }

        /// <summary>
        /// Vérifie si l'utilisateur (ou API key) a le droit d'effectuer une action sur un composant
        /// </summary>
        /// <param name="componentId">L'identifiant du composant</param>
        /// <returns>Vrai si l'utilisateur a le droit, sinon faux</returns>
        public async Task<bool> CanManageComponentAsync(Guid componentId)
        {
            if (IsAdmin()) return true; // L'admin a tous les droits
            var component = await _db.Components.FindAsync(componentId);
            if (component == null) return false;
            return await CanManageProjectAsync(component.ProjectId);
        }

        /// <summary>
        /// Vérifie si l'utilisateur (ou API key) a le droit d'uploader, downloader ou modifier un fichier
        /// </summary>
        /// <param name="resourceFileId">L'identifiant du fichier</param>
        /// <returns>Vrai si l'utilisateur a le droit, sinon faux</returns>
        public async Task<bool> CanEditFileAsync(Guid resourceFileId)
        {
            if (IsAdmin()) return true; // L'admin a tous les droits
            var file = await _db.ResourceFiles.FindAsync(resourceFileId);
            if (file == null) return false;
            return await CanManageComponentAsync(file.ComponentId);
        }
    }
}
