using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace wegopay.notification.Services.MailService
{
    public interface IMailAppService
    {
        Task<bool> SendMail(string title, string emailDestination, string sender, string message);
       // Task<bool> SendSMS(string title, string phoneNo, string message);
    }
}
