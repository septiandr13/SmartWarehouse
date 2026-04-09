using MailKit.Net.Smtp;
using MimeKit;

namespace SmartWarehouse.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    public class EmailService(IConfiguration config) : IEmailService
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Smart Warehouse System", config["EmailSettings:Sender"]));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                // Hubungkan ke Gmail menggunakan Port 587
                await client.ConnectAsync(config["EmailSettings:SmtpServer"], 587, MailKit.Security.SecureSocketOptions.StartTls);

                // Autentikasi menggunakan App Password
                await client.AuthenticateAsync(config["EmailSettings:Username"], config["EmailSettings:Password"]);

                await client.SendAsync(emailMessage);
            }
            catch (Exception ex)
            {
                // Penting untuk menangkap error saat testing
                throw new Exception($"Gagal mengirim email: {ex.Message}");
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}