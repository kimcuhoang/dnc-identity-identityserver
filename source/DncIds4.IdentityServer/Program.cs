using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace DncIds4.IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureAppConfiguration((webHostBuilderContext, configurationBuilder) =>
                    {
                        //configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
                        //https://andrewlock.net/sharing-appsettings-json-configuration-files-between-projects-in-asp-net-core/
                        if (webHostBuilderContext.HostingEnvironment.IsDevelopment())
                        {
                            var shareSettingsPath = Path.Combine(webHostBuilderContext.HostingEnvironment.ContentRootPath,
                                                                    "..", "..", 
                                                                    "sharedSettings.json");
                            configurationBuilder.AddJsonFile(shareSettingsPath, optional: true);
                        }
                        
                        configurationBuilder.AddJsonFile("sharedSettings.json", optional:true); // When app is published
                        configurationBuilder.AddJsonFile("appsettings.json");
                        configurationBuilder.AddJsonFile($"appsettings.{webHostBuilderContext.HostingEnvironment.EnvironmentName}.json", true);
                        configurationBuilder.AddEnvironmentVariables();
                    });
                    webBuilder.CaptureStartupErrors(true);
                })// Add a new service provider configuration
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                    options.ValidateOnBuild = true;
                });
    }
}
