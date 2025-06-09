using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UseTheOps.PolyglotInitiative.Helpers;
using System.Globalization;

namespace UseTheOps.PolyglotInitiative.Services
{
    /// <summary>
    /// Service de fond pour le traitement des tâches utilisateurs (ex: envoi d'email à la création d'un utilisateur).
    /// </summary>
    public class UserBackgroundTaskService : BackgroundService
    {
        private readonly Channel<UserBackgroundTask> _channel;
        private readonly SmtpMailHelper _smtpMailHelper;
        private readonly SmtpMailOptions _smtpOptions;
        private readonly ILogger<UserBackgroundTaskService> _logger;

        public UserBackgroundTaskService(
            Channel<UserBackgroundTask> channel,
            SmtpMailHelper smtpMailHelper,
            IOptions<SmtpMailOptions> smtpOptions,
            ILogger<UserBackgroundTaskService> logger)
        {
            _channel = channel;
            _smtpMailHelper = smtpMailHelper;
            _smtpOptions = smtpOptions.Value;
            _logger = logger;
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var task in _channel.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    if (task.TaskType == "UserCreated" && !string.IsNullOrEmpty(task.Email))
                    {
                        // Détermination de la langue (par défaut fr-FR)
                        var culture = string.IsNullOrWhiteSpace(task.UserName) ? "fr-FR" : "fr-FR"; // TODO: détecter la langue utilisateur
                        var templatePath = $"Resources/MailTemplates/UserCreated.{culture}.liquid";
                        var model = new { userName = task.UserName, activationLink = task.ActivationLink };
                        var htmlBody = await LiquidTemplateHelper.RenderTemplateAsync(templatePath, model);
                        await _smtpMailHelper.SendMailAsync(
                            task.Email!,
                            culture.StartsWith("fr") ? "Bienvenue sur Polyglot Initiative" : "Welcome to Polyglot Initiative",
                            htmlBody);
                        _logger.LogInformation($"Sent welcome email to {task.Email}");
                    }
                    // Ajoutez d'autres types de tâches ici.
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing user background task: {task.TaskType}");
                }
            }
        }
    }
}
