using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using MailKit.Security;

namespace ChessTournamentMate.Shared
{

	/// <summary>
	/// Sends confirmation emails during registration
	/// </summary>
	public class AccountEmailSender : IEmailSender
    {

		private readonly MailKit.Net.Smtp.SmtpClient _smtpClient;
		private readonly AccountEmailSenderSettings _settings;

		public AccountEmailSender(AccountEmailSenderSettings settings)
		{
			_smtpClient = new MailKit.Net.Smtp.SmtpClient();
			_settings = settings;
		}


		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
			var emailMessage = new MimeMessage();
			emailMessage.From.Add(MailboxAddress.Parse(_settings.From));
			emailMessage.To.Add(MailboxAddress.Parse(email));
			emailMessage.Subject = subject;

            var builder = new BodyBuilder
            {
                TextBody = htmlMessage
            };
            emailMessage.Body = builder.ToMessageBody();

			await _smtpClient.ConnectAsync(_settings.Smtp, _settings.Port, SecureSocketOptions.SslOnConnect);
			await _smtpClient.AuthenticateAsync(_settings.From, _settings.Password);
			await _smtpClient.SendAsync(emailMessage);
			await _smtpClient.DisconnectAsync(true);
        }
    }
}
