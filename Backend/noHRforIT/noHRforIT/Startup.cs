using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using noHRforIT.Business.Extensions;
using noHRforIT.Business.Helpers;
using noHRforIT.Business.Services;
using noHRforIT.Data;
using noHRforIT.Data.Models;

namespace noHRforIT
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (!_env.IsDevelopment())
            {
                services.Configure<MvcOptions>(o => o.Filters.Add(new RequireHttpsAttribute()));
            }

            services.AddMvc()
                    .AddNewtonsoftJson();
                    //.AddJsonOptions(options =>
                    //{
                    //    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    //    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    //});

            services.AddCors();
            services.AddControllers();
            services.AddApplicationDbContext();            

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            var appSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.JWTSecret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                        var userId = context.Principal.Identity.Name;
                        var user = userService.GetUserById(userId).Result;
                        if (user == null)
                        {
                            context.Fail("Unauthorized");
                        }
                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            //services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
            //services.AddScoped<IUserService, UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //below configuration of Content security policy - tells what files can be included form outside of domain
            //reportUris sends reports of valoiation of policy

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseCsp(option => option.DefaultSources(s => s.Self())
                    .StyleSources(s => s.Self().CustomSources("maxcdn.bootstrapcdn.com"))
                    .ReportUris(r => r.Uris("/reports")));

                app.UseXfo(o => o.Deny());
                app.UseHsts(hsts => hsts.MaxAge(days: 72));
            }

            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
