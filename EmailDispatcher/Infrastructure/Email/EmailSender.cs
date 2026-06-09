using System;
using System.Net;
using System.Net.Mail;

namespace EmailDispatcher.Infrastructure.Email
{
    public class EmailSender
    {
        public virtual async Task Send(string to, string subject, string body, string fromEmail, string NomeCampanha, string password)
        {
            using var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true,
                Timeout = 10000
            };

            using var mail = new MailMessage
            {
                From = new MailAddress(fromEmail, NomeCampanha),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(to);

            try
            {
                await smtp.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao enviar email para {to}: {ex.Message}", ex);
            }
        }
    }
}
