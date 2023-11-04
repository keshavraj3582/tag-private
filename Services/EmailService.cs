using MimeKit;
using MailKit.Net.Smtp;

namespace School_Login_SignUp.Services
{
    public class EmailService
    {
        public async Task<bool> SendOtpByEmailAsync(string email, string otp, string messageBody)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Tech-Avant-Garde", "saheranadaf11@gmail.com"));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = "Registration Code";
                message.Body = new TextPart("plain")
                {
                    Text = messageBody + otp
                };
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("saheranadaf11@gmail.com", "fjkkdiqzcjjulfal");
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
