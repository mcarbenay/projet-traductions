using System;

namespace UseTheOps.PolyglotInitiative.Helpers
{
    /// <summary>
    /// Options for SMTP mail sending.
    /// </summary>
    public class SmtpMailOptions
    {
        /// <summary>SMTP server host.</summary>
        public string Host { get; set; } = string.Empty;
        /// <summary>SMTP server port.</summary>
        public int Port { get; set; } = 25;
        /// <summary>SMTP user name.</summary>
        public string User { get; set; } = string.Empty;
        /// <summary>SMTP password.</summary>
        public string Password { get; set; } = string.Empty;
        /// <summary>Sender email address.</summary>
        public string From { get; set; } = string.Empty;
        /// <summary>Use SSL for SMTP connection.</summary>
        public bool UseSsl { get; set; } = false;
    }
}
