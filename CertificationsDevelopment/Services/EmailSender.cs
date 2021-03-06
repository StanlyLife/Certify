﻿using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CertificationsDevelopment.Services {
	public class EmailSender : IEmailSender{

		public AuthMessageSenderOptions Options { get; }
		public EmailSender(IOptions<AuthMessageSenderOptions> optionsAcessor) {
			Options = optionsAcessor.Value;
		}

		public Task SendEmailAsync(string email, string subject, string message) {
			return Execute(Options.SendGridKey, subject, message, email);
		}

		public Task Execute(string apiKey, string subject, string message, string email) {
			var client = new SendGridClient(apiKey);
			var msg = new SendGridMessage() {
				From = new EmailAddress("Admin@Stianlife.com", Options.SendGridUser),
				Subject = subject,
				PlainTextContent = message,
				HtmlContent = message
			};
			msg.AddTo(new EmailAddress(email));

			// Disable click tracking.
			// See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
			msg.SetClickTracking(false, false);

			return client.SendEmailAsync(msg);
		}

	}
}
