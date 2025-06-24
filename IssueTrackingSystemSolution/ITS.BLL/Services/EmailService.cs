using System.Net;
using System.Net.Mail;

namespace ITS.BLL.Services
{
    public static class EmailService
    {
        // Configure your SMTP server here
        private static readonly string smtpHost = "smtp.your-email.com";
        private static readonly int smtpPort = 587; // Or your SMTP port
        private static readonly string smtpUser = "your-email@example.com";
        private static readonly string smtpPass = "your-email-password";
        private static readonly string fromEmail = "no-reply@example.com";
        private static readonly string fromName = "Issue Tracking System";

        public static void SendEmail(string toEmail, string subject, string body)
        {
            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(smtpUser, smtpPass);

                var mail = new MailMessage();
                mail.From = new MailAddress(fromEmail, fromName);
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                client.Send(mail);
            }
        }
    }
}
