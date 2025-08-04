using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using BussinesLogic.Models;
using BussinesLogic.Utils;
using AzureFunctionPB.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.Extensions.Http;
using Serilog;
using Serilog.Events;

public class Programq
{

    static async Task Main(string[] args)
    {
        FunctionsApplicationBuilder builder = FunctionsApplication.CreateBuilder(args);
        builder.ConfigureFunctionsWebApplication();

        builder.UseMiddleware<ExceptionHandlingMiddleware>();
        builder.UseMiddleware<AutheticationMiddleware>();

        builder.Services.AddOptions<AppSettings>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.Bind(settings);
            });
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHttpClient();

        builder.Services.AddScoped<ExcelProcessor>();


        //Authentication
        IConfigurationSection azureAdSection = builder.Configuration.GetSection("AzureAd");

        azureAdSection.GetSection("Instance").Value = builder.Configuration.GetValue<string>("Instance");
        azureAdSection.GetSection("Domain").Value = builder.Configuration.GetValue<string>("Domain");
        azureAdSection.GetSection("TenantId").Value = builder.Configuration.GetValue<string>("TenantId");
        azureAdSection.GetSection("ClientId").Value = builder.Configuration.GetValue<string>("ClientId");
        azureAdSection.GetSection("ClientSecret").Value = builder.Configuration.GetValue<string>("ClientSecret");
        azureAdSection.GetSection("AuthorityUrl").Value = builder.Configuration.GetValue<string>("AuthorityUrl");
        azureAdSection.GetSection("Audience").Value = builder.Configuration.GetValue<string>("Audience");

        builder.Services.Configure<JwtBearerOptions>(options =>
        {
            options.Authority = builder.Configuration.GetValue<string>("AuthorityUrl");
            options.Audience = builder.Configuration.GetValue<string>("Audience");

            builder.Configuration.Bind("AzureAd", options);
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = false
            };
            options.Events = new JwtBearerEvents();
        });

        builder.Services.AddSingleton(builder.Configuration);

        //services.AddAuthentication(sharedOptions =>
        //{
        //    sharedOptions.DefaultScheme = Constants.Bearer;
        //    sharedOptions.DefaultChallengeScheme = Constants.Bearer;
        //})
        //.AddMicrosoftIdentityWebApi(context.Configuration);

        builder.Build().Run();
        
    } 

}