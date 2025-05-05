using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using LibraVerse.Settings;
using LibraVerse.DTOs.Email;
using Microsoft.Extensions.Options;
using LibraVerse.Services.Interface;

namespace LibraVerse.Services;

public class MailService(IOptions<EmailSettings> settings) : IMailService
{
    private readonly EmailSettings _settings = settings.Value;

    public void SendEmail(EmailDto email)
    {
        var emailRequest = new MimeMessage();
        
        emailRequest.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        emailRequest.To.Add(MailboxAddress.Parse(email.ToEmail));
        emailRequest.Subject = email.Subject;

        var builder = new BodyBuilder();
        
        if (email.IsHtml)
            builder.HtmlBody = email.Body;
        else
            builder.TextBody = email.Body;

        emailRequest.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        smtp.Connect(_settings.SmtpServer, _settings.SmtpPort, SecureSocketOptions.StartTls);
        smtp.Authenticate(_settings.Username, _settings.Password);
        smtp.Send(emailRequest);
        smtp.Disconnect(true);
    }
}