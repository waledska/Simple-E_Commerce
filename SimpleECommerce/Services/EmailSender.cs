using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using SimpleECommerce.Helpers;

namespace SimpleECommerce.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly usedDataForEmailSender _usedDataForEmailSender;
        public EmailSender(IOptions<usedDataForEmailSender> usedDataForEmailSender)
        {
            _usedDataForEmailSender = usedDataForEmailSender.Value;
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var senderMail = _usedDataForEmailSender.email;
            // Sender password
            var pw = _usedDataForEmailSender.senderPass;

            // Create a new SMTP client using Gmail's SMTP server settings
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true, // Use SSL for secure email transmission
                Credentials = new NetworkCredential(senderMail, pw),
                UseDefaultCredentials = false // Do not use default credentials
            };

            // Create a new MailMessage object
            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderMail), // Set the sender's address
                Subject = subject, // Set the email's subject
                Body = htmlMessage, // Set the email's body to the HTML message
                IsBodyHtml = true // Specify that the email body is HTML
            };

            mailMessage.To.Add(email); // Add the recipient's email address

            // Send the email asynchronously
            return client.SendMailAsync(mailMessage);
        }
    }
}
