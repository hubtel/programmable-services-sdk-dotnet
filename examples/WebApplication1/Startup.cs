using System.Text.Json;
using Hubtel.ProgrammableServices.Sdk.Core;
using Hubtel.ProgrammableServices.Sdk.Extensions;
using Hubtel.ProgrammableServices.Sdk.Storage;
using JustEat.StatsD;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProgrammableServicesSampleApp.ProgrammableServiceControllers;

namespace ProgrammableServicesSampleApp
{
    public class Startup
    {

        private readonly IWebHostEnvironment _environment;
        private IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _environment = environment;

            var builder = new ConfigurationBuilder().SetBasePath(_environment.ContentRootPath).AddJsonFile("appsettings.json",
                true,
                true).AddJsonFile($"appsettings.{_environment.EnvironmentName}.json",
                true).AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"))
                    .ClearProviders().SetMinimumLevel(LogLevel.Debug).AddConsole());


            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
           
            
            //add programmable services 
            services.AddHubtelProgrammableServices((p) => new ProgrammableServiceConfiguration
            {
                Storage = new DefaultProgrammableServiceStorage(),
                HubtelFulfilmentApiEndpoint = "http://ps.hubtel.com/callback", //put Hubtel's fulfilment endpoint here; this is a dummy
                EnableResumeSession = true
            });

        

            services.AddStatsD(
                (provider) =>
                {
                    var logger = provider.GetService<ILogger<Program>>();
                    return new StatsDConfiguration()
                    {
                        Host = "localhost",
                        Port = 2213,
                        Prefix = "test",
                        OnError = (ex) =>
                        {
                            logger?.LogError(ex, ex.Message);
                            return true;
                        }
                    };
                });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //alternative to creating actual controllers
            //uncomment if you want to use this approach
           app.UseHubtelProgrammableService("/api/paginate", nameof(PaginationTestController));

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

           
        }
    }
}
