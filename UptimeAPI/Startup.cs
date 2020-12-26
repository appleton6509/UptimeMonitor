using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Data;
using AutoMapper;
using Data.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using UptimeAPI.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;

namespace UptimeAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private readonly string AppCorsPolicy = "_appCorsPolicy";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            //for entity framework
            services.AddDbContext<UptimeContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("Default"), s => s.MigrationsAssembly("Data")));
            services.AddAutoMapper(typeof(MappingProfile));

            //for identity
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<UptimeContext>()
                .AddDefaultTokenProviders();

            //for Tokens
            var jwtSettings = Configuration.GetSection("Jwt").Get<JwtSettings>();
            services.Configure<JwtSettings>(Configuration.GetSection("Jwt"));
            services.AddAuthentication(opt => {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
                });

            //Cors policy 
            services.AddCors(options =>
            {
                options.AddPolicy(AppCorsPolicy, builder => builder.AllowAnyOrigin().AllowAnyHeader());
            });

        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(AppCorsPolicy);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
