
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Data;
using AutoMapper;
using UptimeAPI.Controllers.Mapping;
using Data.Repositories;
using UptimeAPI.Controllers.Repositories;
using UptimeAPI.Services.Email;
using UptimeAPI.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace UptimeAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly string AppCorsPolicy = "_appCorsPolicy";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<UptimeContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString("Default"), s => s.MigrationsAssembly("Data")));
            services.AddScoped<IEndPointRepository, EndPointRepository>();
            services.AddScoped<IResultDataRepository, ResultDataRepository>();
            services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
            services.AddScoped<IEmailService, EmailService>();

            services.AddAutoMapper(typeof(MappingProfile))
                        .AddCustomIdentity()
                        .AddCustomAuthentication(Configuration)
                        .AddCustomAuthorization()
                        .AddCustomSwagger()
                        .AddCustomCors(AppCorsPolicy);

            //caching
            //services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();
            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("v1/swagger.json", "UptimeAP v1");
            });
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(AppCorsPolicy);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            logger.LogInformation("Database: " + Configuration.GetConnectionString("DisplayName"));
        }
    }
}
