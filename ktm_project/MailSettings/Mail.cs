using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;


namespace ktm_project.MailSettings
{
    public class Mail
    {
        private IConfiguration configuration;

        public Mail(IConfiguration config)
        {
            configuration = config;
        }

        public bool Send(string from, string to, string subject, string body)
        {
            try
            {
                //Retrieving mail setting from appsetting.json
                var host = configuration["Gmail:Host"];
                var port = int.Parse(configuration["Gmail:Port"]);
                var username = configuration["Gmail:Username"];
                var password = configuration["Gmail:Password"];
                var enable = bool.Parse(configuration["Gmail:SMTP:starttls:enable"]);

                var smtpClient = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    EnableSsl = enable,
                    Credentials = new NetworkCredential(username, password)
                };

                var mailMessage = new MailMessage(from, to);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;

                smtpClient.Send(mailMessage);

                return true;


            }
            catch
            {
                return false;
            }
        }
    }
}
