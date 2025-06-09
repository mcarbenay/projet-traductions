using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace UseTheOps.PolyglotInitiative.Helpers
{
    public static class ExceptionHelper
    {
        /// <summary>
        /// Génère un message d'erreur contextualisé pour les exceptions courantes.
        /// </summary>
        /// <param name="ex">L'exception à traiter</param>
        /// <param name="controller">Nom du contrôleur</param>
        /// <param name="action">Nom de l'action</param>
        /// <returns>Tuple (message, codeErreur)</returns>
        public static (string message, string errorCode) GetFriendlyMessage(Exception ex, string controller, string action)
        {
            if (ex is ArgumentNullException argNull)
            {
                var param = argNull.ParamName ?? "parameter";
                return (LocalizationHelper.GetString("Error_ArgumentNull", param, action, controller), "DataValidationError");
            }
            if (ex is ArgumentException arg)
            {
                var param = arg.ParamName ?? "parameter";
                return (LocalizationHelper.GetString("Error_ArgumentInvalid", param, action, controller), "DataValidationError");
            }
            if (ex is UnauthorizedAccessException)
            {
                return (LocalizationHelper.GetString("Error_UnauthorizedOperation", action, controller), "UnauthorizedError");
            }
            if (ex is NotImplementedException)
            {
                return (LocalizationHelper.GetString("Error_NotImplemented", action, controller), "NotImplementedError");
            }
            if (ex is KeyNotFoundException)
            {
                return (LocalizationHelper.GetString("Error_NotFoundOperation", action, controller), "NotFoundError");
            }
            // Ajout d'autres cas si besoin
            // Gestion des erreurs Entity Framework / PostgreSQL
            if (ex.GetType().Name == "DbUpdateException" || ex.GetType().Name == "DbUpdateConcurrencyException")
            {
                var inner = ex.InnerException;
                if (inner != null && inner.GetType().Name == "PostgresException")
                {
                    var pgEx = inner;
                    var codeProp = inner.GetType().GetProperty("SqlState");
                    var code = codeProp?.GetValue(inner)?.ToString() ?? "";
                    var detailProp = inner.GetType().GetProperty("Detail");
                    var detail = detailProp?.GetValue(inner)?.ToString() ?? "";
                    var columnProp = inner.GetType().GetProperty("ColumnName");
                    var column = columnProp?.GetValue(inner)?.ToString() ?? "";
                    switch (code)
                    {
                        case "23503": // Foreign key violation
                            return (LocalizationHelper.GetString("Error_ForeignKeyViolation", column, action, controller), "ForeignKeyViolation");
                        case "23505": // Unique violation
                            return (LocalizationHelper.GetString("Error_UniqueViolation", action, controller), "UniqueConstraintViolation");
                        case "22001": // String data, right truncation
                            return (LocalizationHelper.GetString("Error_StringTruncation", column, action, controller), "StringTruncationError");
                        case "23514": // Check violation
                            return (LocalizationHelper.GetString("Error_CheckConstraint", action, controller), "CheckConstraintViolation");
                        case "23502": // Not null violation
                            return (LocalizationHelper.GetString("Error_NotNullViolation", column, action, controller), "NotNullViolation");
                        case "22P02": // Invalid text representation (type error)
                            return (LocalizationHelper.GetString("Error_InvalidDataType", column, action, controller), "InvalidDataType");
                        case "40P01": // Deadlock detected
                            return (LocalizationHelper.GetString("Error_Deadlock", action, controller), "DeadlockDetected");
                        case "08001": // Connection exception
                        case "08006":
                            return (LocalizationHelper.GetString("Error_DbCommunication", action, controller), "DbCommunicationError");
                        default:
                            return (LocalizationHelper.GetString("Error_DbCommunication", action, controller), "DbCommunicationError");
                    }
                }
                // Si pas de PostgresException, message générique EF
                return (LocalizationHelper.GetString("Error_DbGeneric", action, controller), "DbError");
            }
            // Gestion explicite des erreurs d'optimistic concurrency
            if (ex.GetType().Name == "DbUpdateConcurrencyException")
            {
                return (LocalizationHelper.GetString("Error_OptimisticConcurrency", action, controller), "OptimisticConcurrencyError");
            }
            // Timeout
            if (ex.GetType().Name == "NpgsqlException" && ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase))
            {
                return (LocalizationHelper.GetString("Error_DbTimeout", action, controller), "DbTimeout");
            }
            if (ex is TaskCanceledException)
            {
                return (LocalizationHelper.GetString("Error_Timeout", action, controller), "Timeout");
            }
            // Ajout d'autres cas si besoin
            return (LocalizationHelper.GetString("Error_Internal"), "InternalError");
        }

        /// <summary>
        /// Génère un IActionResult (ProblemDetails) adapté à l'exception et au contexte d'appel.
        /// </summary>
        public static IActionResult ToActionResult(Exception ex, ControllerBase controller, string action)
        {
            var (message, code) = GetFriendlyMessage(ex, controller.GetType().Name.Replace("Controller", string.Empty), action);
            int status = 500;
            string title = controller.LocalizerOrDefault("Error_Internal");
            // Mapping code -> status
            switch (code)
            {
                case "DataValidationError":
                case "NotNullViolation":
                case "StringTruncationError":
                case "CheckConstraintViolation":
                case "InvalidDataType":
                    status = 400;
                    title = controller.LocalizerOrDefault("Error_Validation");
                    break;
                case "UnauthorizedError":
                    status = 403;
                    title = controller.LocalizerOrDefault("Error_Unauthorized");
                    break;
                case "NotFoundError":
                    status = 404;
                    title = controller.LocalizerOrDefault("Error_NotFound");
                    break;
                case "OptimisticConcurrencyError":
                    status = 409;
                    title = controller.LocalizerOrDefault("Error_OptimisticConcurrency");
                    break;
                case "DbTimeout":
                case "Timeout":
                    status = 504;
                    title = controller.LocalizerOrDefault("Error_Timeout");
                    break;
                // autres cas spécifiques si besoin
            }
            var problem = new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = title,
                Status = status,
                Detail = message,
                Instance = controller.HttpContext?.Request?.Path.Value
            };
            problem.Extensions["code"] = code;
            return controller.StatusCode(status, problem);
        }
    }

    // Extension pour simplifier l'accès à la localisation dans le helper
    public static class ExceptionHelperExtensions
    {
        public static string LocalizerOrDefault(this ControllerBase controller, string key)
        {
            try
            {
                return UseTheOps.PolyglotInitiative.LocalizationHelper.GetString(key);
            }
            catch { return key; }
        }
    }
}
