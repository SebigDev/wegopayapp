using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using userprofile.core.Managers.UserManager;
using userprofile.persistence;
using wegopay.common.Helpers;
using wegopay.common.Repository;

namespace userprofile.core
{
   public  static class ServiceCollectionExtension
    {
        public static IServiceCollection AddUserProfileServices(this IServiceCollection services)
        {
            services.AddTransient<DbContext, UserProfileDbContext>();
            services.AddScoped(typeof(ISimpleRepository<>), typeof(SimpleRepository<>));
            services.AddTransient<IUserManagerService, UserManagerService>();

            services.AddTransient<HttpClient>();
            services.AddTransient<HttpUtil>();
         

           // string proxyAddress = IConfiguration.GetValue<string>("GlobalConfig:NetworkProxy");
           // services.AddTransient<IHttpClientUtil>(a => new HttpClientUtil(proxyAddress));

            return services;
        }
   }
}
