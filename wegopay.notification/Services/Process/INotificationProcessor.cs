using System.Threading.Tasks;

namespace wegopay.notification.Services.Process
{
    public interface INotificationProcessor
    {
        /// <summary>
        /// Processes the Notification based on the response and notification type
        /// </summary>
        /// <param name="response"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<bool> ProcessNotificationAsync(dynamic response, int type);
        Task<bool> SendActivationCodeByMail(string name, string code, string email);
        Task<bool> SendActivationCodeBySMS(string name, string code, string phone);
    }
}
