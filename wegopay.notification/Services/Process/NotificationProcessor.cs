
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using wegopay.notification.Services.MailService;
using wegopay.notification.Services.Templates;

namespace wegopay.notification.Services.Process
{
    public class NotificationProcessor : INotificationProcessor
    {
        private readonly ITemplateService _templateService;
        private readonly IMailAppService _mailAppService;
        private MailSettings _settings;

        private readonly IHostingEnvironment _env;

        public NotificationProcessor(ITemplateService templateService, IMailAppService mailAppService,
            IOptions<MailSettings> options, IHostingEnvironment env)
        {
            _templateService = templateService;
            _mailAppService = mailAppService;
            _settings = options.Value;
            _env = env;
        }


        public async Task<bool> ProcessNotificationAsync(dynamic response, int type)
        {
            var templateResponse = string.Empty;
            var messageTitle = string.Empty;

            var template = await _templateService.RetrieveTemplate(type);
            
            if(type == 1)
            {
                messageTitle = "Registration";
                templateResponse = template.Replace("{username}", $"{response.Username}")
                                           .Replace("{emailAddress}", $"{response.EmailAddress}")
                                           .Replace("{password}", $"{response.Password}");
            }
            else if(type == 2)
            {
                messageTitle = "Password Change";
                templateResponse = template.Replace("{username}", $"{response.Username}");
            }
            else if (type == 3)
            {
                messageTitle = "Password Reset";
                templateResponse = template.Replace("{username}", $"{response.Username}")
                                           .Replace("{password}", $"{response.Password}");
            }
            var sendToMailService = await _mailAppService.SendMail(messageTitle, response.EmailAddress, _settings.Username, templateResponse);

            return await Task.FromResult(true);
        }

        public async Task<bool> SendActivationCodeByMail(string name, string code, string email)
        {
            var templateResponse = string.Empty;
            var messageTitle = string.Empty;

            var template = "<div style=\"padding: 5px; \"><b>Dear {name}</b>,<br/><br/> Your registration was successful. <br/><br/> Your Activation Code" +
               " is <b>{code}</b> <br/>Thanks for choosing <b>WegoPay</b></div>"; ;

            messageTitle = "Activation Code";
            templateResponse = template.Replace("{name}", $"{name}")
                                       .Replace("{code}", $"{code}")
                                       .Replace("{email}", $"{email}");

            var sendToMailService = await _mailAppService.SendMail(messageTitle, email, _settings.Username, templateResponse);
            if (sendToMailService) return true;
            return false;
        }


        public async Task<bool> SendActivationCodeBySMS(string name, string code, string phone)
        {
            var templateResponse = string.Empty;
            var messageTitle = string.Empty;

            var template = " Dear {name}, Your registration was successful. Your Activation Code is {code}. Thanks for choosing WegoPay"; ;

            messageTitle = "Activation Code";
            templateResponse = template.Replace("{code}", $"{code}")
                                        .Replace("{name}", $"{name}")
                                       .Replace("{phone}", $"{phone}");

            
            var sendSMSService = await _mailAppService.SendSMS(messageTitle,phone,templateResponse);
            if (sendSMSService) return true;
            return false;
        }
    }
}
