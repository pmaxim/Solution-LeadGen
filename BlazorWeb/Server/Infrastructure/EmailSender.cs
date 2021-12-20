using BlazorWeb.Server.Models;
using Domain.Entities;
using Domain.Models;
using Domain.Repositories;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

//https://kenhaggerty.com/articles/article/aspnet-core-22-smtp-emailsender-implementation
namespace BlazorWeb.Server.Infrastructure
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly ISettingRepository _settingRepository;

        public EmailSender(IOptions<EmailSettings> emailSettings,
            ISettingRepository settingRepository)
        {
            _emailSettings = emailSettings.Value;
            _settingRepository = settingRepository;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var mimeMessage = new MimeMessage();

                mimeMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.Sender));

                mimeMessage.To.Add(new MailboxAddress(email));

                mimeMessage.Subject = subject;

                mimeMessage.Body = new TextPart("html")
                {
                    Text = message
                };

                using var client = new MailKit.Net.Smtp.SmtpClient { ServerCertificateValidationCallback = (s, c, h, e) => true};
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)

                await client.ConnectAsync(_emailSettings.MailServer, _emailSettings.MailPort);
                
                await client.AuthenticateAsync(_emailSettings.Sender, _emailSettings.Password);

                await client.SendAsync(mimeMessage);

                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _settingRepository.Create(new Setting
                {
                    Name = Constants.ErrorEmail, Value = ex.Message
                });
                await _settingRepository.SaveChangesAsync();
            }
        }
    }
}
