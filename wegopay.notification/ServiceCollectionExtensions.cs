using Microsoft.Extensions.DependencyInjection;
using wegopay.notification.Services.MailService;
using wegopay.notification.Services.Process;
using wegopay.notification.Services.Templates;

namespace wegopay.notification
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWegopayNotification(this IServiceCollection services)
        {
            services.AddTransient<ITemplateService, TemplateService>();
            services.AddTransient<IMailAppService, MailAppService>();
            services.AddTransient<INotificationProcessor, NotificationProcessor>();
            return services;
        }
    }
}
