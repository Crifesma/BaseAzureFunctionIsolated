using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using BussinesLogic.Models;
using BussinesLogic.Utils;
using AzureFunctionPB.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(builder =>
    {
        builder.UseMiddleware<ExceptionHandlingMiddleware>();
        builder.UseMiddleware<AutheticationMiddleware>();
    })
    .ConfigureFunctionsWebApplication((IFunctionsWorkerApplicationBuilder builder) =>
    {
        
    })
    .ConfigureServices(
        (context, services) =>
        {
            //Logs, cuidado con los costos
            //services.AddApplicationInsightsTelemetryWorkerService();
            //services.ConfigureFunctionsApplicationInsights();

            // Configurar AppSettings
            services.AddOptions<AppSettings>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.Bind(settings);
                });

            // Registrar HttpClient
            services.AddHttpClient();

            // Registrar servicios
            services.AddScoped<ExcelProcessor>();

            //Authentication
            IConfigurationSection azureAdSection = context.Configuration.GetSection("AzureAd");

            azureAdSection.GetSection("Instance").Value = context.Configuration.GetValue<string>("Instance");
            azureAdSection.GetSection("Domain").Value = context.Configuration.GetValue<string>("Domain");
            azureAdSection.GetSection("TenantId").Value = context.Configuration.GetValue<string>("TenantId");
            azureAdSection.GetSection("ClientId").Value = context.Configuration.GetValue<string>("ClientId");
            azureAdSection.GetSection("ClientSecret").Value = context.Configuration.GetValue<string>("ClientSecret");
            azureAdSection.GetSection("AuthorityUrl").Value = context.Configuration.GetValue<string>("AuthorityUrl");
            azureAdSection.GetSection("Audience").Value = context.Configuration.GetValue<string>("Audience");

            services.Configure<JwtBearerOptions>(options =>
            {
                options.Authority = context.Configuration.GetValue<string>("AuthorityUrl");
                options.Audience = context.Configuration.GetValue<string>("Audience");

                context.Configuration.Bind("AzureAd", options);
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = false
                };
                options.Events = new JwtBearerEvents();
            });

            services.AddSingleton(context.Configuration);

            //services.AddAuthentication(sharedOptions =>
            //{
            //    sharedOptions.DefaultScheme = Constants.Bearer;
            //    sharedOptions.DefaultChallengeScheme = Constants.Bearer;
            //})
            //.AddMicrosoftIdentityWebApi(context.Configuration);

        }
    )
    .Build();

host.Run();
