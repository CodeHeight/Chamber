using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using Chamber.Domain.DomainModel;
using Chamber.Domain.Interfaces;
using Chamber.Domain.Interfaces.Services;
using Chamber.Services.Data.Context;
using Chamber.Utilities;

namespace Chamber.Services
{
    public partial class EmailService : IEmailService
    {
        private readonly ILoggingService _loggingService;
        private readonly ISettingsService _settingsService;
        private readonly ChamberContext _context;

        public EmailService(ILoggingService loggingService, ISettingsService settingsService, IChamberContext context)
        {
            _loggingService = loggingService;
            _settingsService = settingsService;
            _context = context as ChamberContext;
        }

        public Email Add(Email email)
        {
            return _context.Email.Add(email);
        }

        public string EmailTemplate(string to, string content)
        {
            var settings = _settingsService.GetSettings();
            return EmailTemplate(to, content, settings);
        }

        public string EmailTemplate(string to, string content, Settings settings)
        {
            using (var sr = File.OpenText(HostingEnvironment.MapPath(@"~/Content/emails/EmailNotification.htm")))
            {
                var sb = sr.ReadToEnd();
                sr.Close();
                sb = sb.Replace("#CONTENT#", content);
                sb = sb.Replace("#SITENAME#", settings.SiteName);
                sb = sb.Replace("#SITEURL#", settings.SiteUrl);
                if (!string.IsNullOrEmpty(to))
                {
                    to = $"<p>{to},</p>";
                    sb = sb.Replace("#TO#", to);
                }

                return sb;
            }
        }

        public void SendMail(Email email)
        {
            SendMail(new List<Email> { email });
        }

        public void SendMail(List<Email> emails)
        {
            var settings = _settingsService.GetSettings();
            SendMail(emails, settings);
        }

        public void SendMail(List<Email> emails, Settings settings)
        {
            // Add all the emails to the email table
            // They are sent every X seconds by the email sending task
            foreach (var email in emails)
            {

                // Sort local images in emails
                email.Body = StringUtils.AppendDomainToImageUrlInHtml(email.Body, settings.SiteUrl.TrimEnd('/'));
                Add(email);
            }
        }
    }
}