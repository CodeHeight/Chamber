using System.Collections.Generic;
using Chamber.Domain.DomainModel;

namespace Chamber.Domain.Interfaces.Services
{
    public partial interface IEmailService
    {
        Email Add(Email email);
        string EmailTemplate(string to, string content);
        string EmailTemplate(string to, string content, Settings settings);
        void SendMail(Email email);
        void SendMail(List<Email> email);
        void SendMail(List<Email> email, Settings settings);
    }
}