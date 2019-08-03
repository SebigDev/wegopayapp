using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using userprofile.core;
using userprofile.persistence;

namespace userprofile.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connStr = Configuration.GetConnectionString("UserProfileConnectionString");
            services.AddDbContext<UserProfileDbContext>(opt =>
                      opt.UseSqlServer(connStr, builder =>
                      {
                          builder.UseRowNumberForPaging();
                      }));

            services.AddSwaggerDocument(document =>
            {
                document.DocumentName = "Wegopay User Profile";
                document.Description = "API Endpoints for User Profiles";
                document.Title = "Wegopay Application API";
                document.Version = "1.0";

            });

            services.AddOptions();
            services.AddUserProfileServices();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Middleware  
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUi3();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
