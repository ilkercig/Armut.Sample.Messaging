using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Armut.Sample.Messaging.Data;
using Armut.Sample.Messaging.Filters;
using Armut.Sample.Messaging.JWT;
using Armut.Sample.Messaging.Model;
using Armut.Sample.Messaging.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace Armut.Sample.Messaging
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
            ConfigureJwt(services);

            services.AddDbContext<MessagingContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<IMessageRepository, MessageRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IBlockingRepository, BlockingRepository>();
            services.AddTransient<IModelFactory, ModelFactory>();
            services.AddTransient<IDateTimeService, DateTimeService>();



            services.AddMvc();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseExceptionHandler(
             options => {
                 options.Run(
                 async context =>
                 {
                     context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                     context.Response.ContentType = "text/html";
                     var ex = context.Features.Get<IExceptionHandlerFeature>();
                     if (ex != null)
                     {
                         var err = $"<h1>Error: {ex.Error.Message}</h1>{ex.Error.StackTrace }";
                         Log.Fatal(err);
                         await context.Response.WriteAsync("Internal Server Error").ConfigureAwait(false);
                     }
                 });
                }
            );


            app.UseAuthentication();
            app.UseMvc();
        }

        private void ConfigureJwt(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = "Armut.Sample.Messaging",
                    ValidAudience = "Armut.Sample.Messaging",
                    IssuerSigningKey = JwtSecurityKey.Create("armut.sample.messaging.secret")
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("OnAuthenticationFailed: " + context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("OnTokenValidated: " + context.SecurityToken);
                        return Task.CompletedTask;
                    }
                };
        });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("User",
                    policy => policy.RequireClaim("Username"));
            });
        }

    }
}
