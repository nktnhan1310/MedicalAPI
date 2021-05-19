using Medical.Entities;
using Medical.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Interface
{
    public interface IEmailConfigurationService : IDomainService<EmailConfiguration, BaseSearch>
    {
        Task<EmailSendConfigure> GetEmailConfig();
        EmailContent GetEmailContent();
        void Send(string subject, string body, string[] Tos);
        void Send(string subject, string body, string[] Tos, string[] CCs);
        void Send(string subject, string body, string[] Tos, string[] CCs, string[] BCCs);
        void Send(string subject, string[] Tos, string[] CCs, string[] BCCs, EmailContent emailContent);
        void SendMail(string subject, string Tos, string[] CCs, string[] BCCs, EmailContent emailContent);

    }
}
