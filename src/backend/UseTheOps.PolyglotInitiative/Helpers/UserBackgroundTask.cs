using System;

namespace UseTheOps.PolyglotInitiative.Helpers
{
    /// <summary>
    /// Représente une tâche de fond utilisateur (ex: notification email).
    /// </summary>
    public class UserBackgroundTask
    {
        /// <summary>Type de tâche (ex: "UserCreated").</summary>
        public string TaskType { get; set; } = string.Empty;
        /// <summary>Identifiant de l'utilisateur concerné.</summary>
        public Guid UserId { get; set; }
        /// <summary>Email de l'utilisateur (si applicable).</summary>
        public string? Email { get; set; }
        /// <summary>Nom de l'utilisateur (si applicable).</summary>
        public string? UserName { get; set; }
        /// <summary>Lien d’activation pour finaliser l’inscription (si applicable).</summary>
        public string? ActivationLink { get; set; }
        /// <summary>Code langue de l'utilisateur (RFC 5646, ex: "fr-FR", "en-US").</summary>
        public string? Language { get; set; }
        // Ajoutez d'autres propriétés selon les besoins.
    }
}
