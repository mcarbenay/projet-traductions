using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace UseTheOps.PolyglotInitiative.Helpers
{
    /// <summary>
    /// Helper class for sending emails via SMTP.
    /// </summary>
    public class SmtpMailHelper
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _fromAddress;
        private readonly string? _fromDisplayName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmtpMailHelper"/> class.
        /// </summary>
        /// <param name="host">SMTP server host.</param>
        /// <param name="port">SMTP server port.</param>
        /// <param name="username">SMTP username.</param>
        /// <param name="password">SMTP password.</param>
        /// <param name="fromAddress">Sender email address.</param>
        /// <param name="enableSsl">Enable SSL.</param>
        /// <param name="fromDisplayName">Sender display name (optional).</param>
        public SmtpMailHelper(string host, int port, string username, string password, string fromAddress, bool enableSsl, string? fromDisplayName = null)
        {
            _smtpClient = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = enableSsl
            };
            _fromAddress = fromAddress;
            _fromDisplayName = fromDisplayName;
        }

        /// <summary>
        /// Event raised just before sending an email. Allows test interception and simulation.
        /// </summary>
        public static event EventHandler<MailSendingEventArgs>? MailSending;

        /// <summary>
        /// Sends an email asynchronously, with optional embedded images.
        /// </summary>
        /// <param name="to">Recipient email address.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="body">Email body (HTML allowed).</param>
        /// <param name="embeddedImages">Optional dictionary of contentId => image bytes for embedding images.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SendMailAsync(string to, string subject, string body, IDictionary<string, byte[]>? embeddedImages = null)
        {
            var from = string.IsNullOrWhiteSpace(_fromDisplayName)
                ? new MailAddress(_fromAddress)
                : new MailAddress(_fromAddress, _fromDisplayName);
            var mail = new MailMessage(from, new MailAddress(to))
            {
                Subject = subject,
                IsBodyHtml = true
            };
            if (embeddedImages != null && embeddedImages.Any())
            {
                var htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                foreach (var kvp in embeddedImages)
                {
                    var imageStream = new System.IO.MemoryStream(kvp.Value);
                    var linkedResource = new LinkedResource(imageStream)
                    {
                        ContentId = kvp.Key,
                        TransferEncoding = System.Net.Mime.TransferEncoding.Base64
                    };
                    htmlView.LinkedResources.Add(linkedResource);
                }
                mail.AlternateViews.Add(htmlView);
            }
            else
            {
                mail.Body = body;
            }
            // --- Event for test interception ---
            var args = new MailSendingEventArgs(mail, embeddedImages);
            MailSending?.Invoke(this, args);
            if (args.Sent)
                return; // Simulate sent, do not send for real
            await _smtpClient.SendMailAsync(mail);
        }

        /// <summary>
        /// EventArgs for the MailSending event, allows test interception and simulation.
        /// </summary>
        public class MailSendingEventArgs : EventArgs
        {
            /// <summary>The MailMessage about to be sent.</summary>
            public MailMessage MailMessage { get; }
            /// <summary>Dictionary of embedded images (contentId → bytes), if any.</summary>
            public IDictionary<string, byte[]>? EmbeddedImages { get; }
            /// <summary>If set to true, the mail will not be sent for real.</summary>
            public bool Sent { get; set; }
            public MailSendingEventArgs(MailMessage mailMessage, IDictionary<string, byte[]>? embeddedImages)
            {
                MailMessage = mailMessage;
                EmbeddedImages = embeddedImages;
                Sent = false;
            }
        }
    }
}
