using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Mail;
using AIResumeAnalyzer.Application.Configuration;
using AIResumeAnalyzer.Application.DTOs.Email;
using AIResumeAnalyzer.Application.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace AIResumeAnalyzer.Infrastructure.Services.Email;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendAsync(SendEmailRequest request)
    {
        using var message = new MailMessage();

        message.From = new MailAddress(
            _settings.SenderEmail,
            _settings.SenderName);

        message.To.Add(request.To);

        message.Subject = request.Subject;

        message.Body = request.Body;

        message.IsBodyHtml = request.IsHtml;

        using var smtp = new SmtpClient(
            _settings.Host,
            _settings.Port);

        smtp.Credentials = new NetworkCredential(
            _settings.Username,
            _settings.Password);

        smtp.EnableSsl = _settings.EnableSsl;

        await smtp.SendMailAsync(message);
    }
}
