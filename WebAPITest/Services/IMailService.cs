using Microsoft.Extensions.Options;
using MailKit.Security;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPITest.Model;

namespace WebAPITest.Services
{
    public interface IMailService
    {
        Task<bool> SendEmailAsync(MailRequest mailRequest);
    }

    public class MailService : IMailService
    {
        private readonly MailSetting _mailSetting;

        public MailService(IOptions<MailSetting> mailSetting)
        {
            _mailSetting = mailSetting.Value;
        }

        public async Task<bool> SendEmailAsync(MailRequest mailRequest)
        {
            try
            {
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(_mailSetting.Mail);
                email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
                email.Subject = mailRequest.Subject;
                var builder = new BodyBuilder();

                builder.HtmlBody = mailRequest.Body;
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();

                smtp.Connect(_mailSetting.Host, _mailSetting.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSetting.Mail, _mailSetting.Password);

                await smtp.SendAsync(email);
                smtp.Disconnect(true);
                return true;
            }
            catch(Exception ex)
            {
                string error = ex.Message;
                return false;
            }
        }
    }
}
