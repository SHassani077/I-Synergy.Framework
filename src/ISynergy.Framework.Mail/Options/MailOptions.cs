﻿using System.Runtime.CompilerServices;

namespace ISynergy.Framework.Mail.Options
{
    /// <summary>
    /// Mail service options.
    /// </summary>
    public class MailOptions
    {
        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        /// <value>The email address.</value>
        public string EmailAddress { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the description or name of the sender.
        /// </summary>
        /// <value>The email sender.</value>
        public string Sender { get; set; } = string.Empty;
    }
}
