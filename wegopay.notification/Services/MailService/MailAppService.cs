using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using IO.Swagger.Client;
using IO.Swagger.Api;
using IO.Swagger.Model;

namespace wegopay.notification.Services.MailService
{
    public class MailAppService : IMailAppService
    {
        private MailSettings _settings;
        public MailAppService(IOptions<MailSettings> options)
        {
            _settings = options.Value;
        }
        public async Task<bool> SendMail(string title, string emailDestination, string sender, string message)
        {
            SmtpClient smtpClient = new SmtpClient();
            
            smtpClient = new SmtpClient
            {
                Host = _settings.Host,
                Port = _settings.Port,
                EnableSsl = _settings.EnableSsl,
                Credentials = new NetworkCredential
                {
                    UserName = _settings.Username,
                    Password = _settings.Password
                }
            };
            var mailMessage = new MailMessage(sender, emailDestination)
            {
                Subject = title,
                Body = message,
                IsBodyHtml = true
            };

            await smtpClient.SendMailAsync(mailMessage);
            return await Task.FromResult(true);
        }

        public async Task<bool> SendSMS(string title, string phoneNo, string message)
        {
            //var accountSid = "AC1d94bb3c2f7f978d2b95bbfcd978e938";
            //var authToken = "e26bd9bb1b8212a3e27957039a371459";
            //TwilioClient.Init(accountSid, authToken);

            //var messageOptions = new CreateMessageOptions(
            //    new PhoneNumber(phoneNo));
            //messageOptions.From = new PhoneNumber("+19803658079");
            //messageOptions.Body = message;

            //var smsMessage = MessageResource.Create(messageOptions);
            //Console.WriteLine(smsMessage.Body);

            ApiClient apiclient = new ApiClient();
            SmsApi smsapi = new SmsApi(apiclient);
            SmsUniqueRequest susreq = new SmsUniqueRequest();
            susreq.Keyid = "7a76d7b5848434e1c3c115ec41fbe05f";
            susreq.Num = phoneNo;
            susreq.Sms = message;

            SMSReponse smsreponse = smsapi.SendSms(susreq);

            return await Task.FromResult(true);
        }
    }
}
