using Data;
using Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UptimeAPI.Services;
using UptimeAPI.Services.Token;

namespace UptimeAPI.Extensions
{
    public static class ServiceCollectionExtension
    {

        public static IServiceCollection AddCustomCors(this IServiceCollection service, string corsPolicy)
        {
            service.AddCors(options =>
            {
                options.AddPolicy(corsPolicy, builder =>
                builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithExposedHeaders("X-Pagination"));
            });
            return service;
        }
        public static IServiceCollection AddCustomSwagger(this IServiceCollection service)
        {
            service.AddSwaggerGen(doc =>
            {
                doc.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "UptimeAPI", Version = "v1" });
            });
            return service;
        }
        public static IServiceCollection AddCustomAuthorization(this IServiceCollection service)
        {
            service.AddAuthorization(options =>
            {
                options.AddPolicy(nameof(Operations), policy => policy.Requirements.Add(new OperationAuthorizationRequirement()));
                options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            });
            service.AddSingleton<IAuthorizationHandler, ModelAuthorizationHandler>();

            return service;
        }
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection service, IConfiguration config)
        {
            var jwtSettings = config.GetSection("Jwt").Get<JwtSettings>();
            service.Configure<JwtSettings>(config.GetSection("Jwt"));
            service.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
            });
            return service;
        }
        public static IServiceCollection AddCustomIdentity(this IServiceCollection service)
        {
            service.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddEntityFrameworkStores<UptimeContext>()
                    .AddDefaultTokenProviders();
            service.AddHttpContextAccessor();

            return service;
        }



    }
}
