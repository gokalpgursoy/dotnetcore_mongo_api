using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using netcore_webapi.Contracts;

namespace netcore_webapi
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
      services.AddCors();
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // required for domain layer (Service katmanında IHttpContextAccessor'a erişemek için)
      IConfigurationSection appSettingsSection = Configuration.GetSection("AppSettings");
      services.Configure<AppSettings>(appSettingsSection);

      AppSettings appSettings = appSettingsSection.Get<AppSettings>();
      byte[] key = Encoding.ASCII.GetBytes(appSettings.Secret);
      services.AddAuthentication(x =>
      {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(x =>
      {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(key),
          ValidateIssuer = false,
          ValidateAudience = false,
        };
      });

      services.Configure<AppSettings>(options =>
      {
        options.ConnectionString
            = Configuration.GetSection("MongoConnection:ConnectionString").Value;
        options.Database
            = Configuration.GetSection("MongoConnection:Database").Value;
      });
      services.AddScoped<IAuthService, AuthService>(); // Configure DI
      services.AddScoped<IUsersService, UsersService>(); // Configure DI

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        loggerFactory.AddConsole(Configuration.GetSection("Logging"));
        loggerFactory.AddDebug();
      }
      else
      {
        app.UseHsts();
      }
      app.UseHttpsRedirection();
      app.UseCors(x => x
                     .AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader()
                     .AllowCredentials());

      app.UseAuthentication();
      // ! Sadece api endpointine atılan isteklerde DecodeToken middleware'i çalışacaktır.
      // ! Token alırken ya da Token gerektirmeyen endpointlerde çalışmasına gerek yok.
      app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"), appBuilder =>
      {
        appBuilder.UseMiddleware<DecodeToken>();
      });
      app.UseExceptionHandler(new ExceptionHandlerOptions { ExceptionHandler = new ExceptionHandler().Invoke });  // Handle Global Errors
      app.UseMvc();
    }
  }
}
