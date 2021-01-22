
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Data;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using UptimeAPI.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using UptimeAPI.Services;
using UptimeAPI.Controllers.Mapping;
using Data.Repositories;
using Data.Models;
using UptimeAPI.Controllers.Repositories;

namespace UptimeAPI
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        public IConfiguration Configuration { get; }
        private readonly string AppCorsPolicy = "_appCorsPolicy";

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _environment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //for entity framework
            if (_environment.IsDevelopment())
                services.AddDbContext<UptimeContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString("Development"), s => s.MigrationsAssembly("Data")));
            else
                services.AddDbContext<UptimeContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString("Production"), s => s.MigrationsAssembly("Data")));
            services.AddScoped<IEndPointRepository, EndPointRepository>();
            services.AddScoped<IHttpResultRepository, HttpResultRepository>();
            services.AddScoped<IWebUserRepository, WebUserRepository>();

            //mapper
            services.AddAutoMapper(typeof(MappingProfile));

            //for identity
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<UptimeContext>()
                .AddDefaultTokenProviders();
            services.AddHttpContextAccessor();

            //for Tokens
            var jwtSettings = Configuration.GetSection("Jwt").Get<JwtSettings>();
            services.Configure<JwtSettings>(Configuration.GetSection("Jwt"));
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
            });

            //user authorization to resource
            services.AddAuthorization(options=>
            {
                options.AddPolicy(nameof(Operations), policy => policy.Requirements.Add(new OperationAuthorizationRequirement()));
            });
            services.AddSingleton<IAuthorizationHandler, EndpointAuthorizationHandler>();

            //Register swagger 
            services.AddSwaggerGen(doc =>
            {
                doc.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "UptimeAPI", Version = "v1" });
            });

            //Cors policy 
            services.AddCors(options =>
            {
                options.AddPolicy(AppCorsPolicy, builder =>
                builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithExposedHeaders("X-Pagination"));
            });

            //caching
            services.AddMemoryCache();

        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

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
        }
    }
}
