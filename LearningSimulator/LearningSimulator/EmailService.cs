using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;

namespace LearningSimulator
{
    public class EmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message, string password)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Learning Simulator", "shulayonok@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("Dear friend", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("Plain")
            {
                Text = message
            };
                                                                                                    
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 465, true);
                await client.AuthenticateAsync("shulayonok@gmail.com", password);
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
    

