using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text.Encodings.Web;

namespace SmartWarehouse.Services
{
    public class IdentityEmailSender(IEmailService emailService) : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            
            string enhancedMessage = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden;'>
                        <div style='background-color: #0d6efd; padding: 20px; text-align: center; color: white;'>
                            <h1 style='margin: 0; font-size: 24px;'>Smart Warehouse</h1>
                        </div>
                        <div style='padding: 30px; line-height: 1.6; color: #333;'>
                            <h2 style='color: #0d6efd;'>{subject}</h2>
                            <p>Halo,</p>
                            <p>Kami menerima permintaan terkait akun Anda. Silakan klik link atau tombol di bawah ini untuk mengatur ulang kata sandi Anda:</p>
                
                            <div style='text-align: center; margin: 30px 0;'>
                                <div style='background-color: #198754; color: white; padding: 15px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>
                                    {htmlMessage}
                                </div>
                            </div>

                            <p style='font-size: 13px; color: #666;'>Jika Anda tidak merasa melakukan permintaan ini, abaikan email ini.</p>
                            <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
                            <p style='font-size: 12px; color: #999; text-align: center;'>
                                &copy; 2026 Smart Warehouse System.
                            </p>
                        </div>
                    </div>";

            await emailService.SendEmailAsync(email, subject, enhancedMessage);
        }
    }
}