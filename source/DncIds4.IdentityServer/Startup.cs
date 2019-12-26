using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DncIds4.IdentityServer.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DncIds4.IdentityServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services
                .AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(Database.IdentityResources)
                .AddInMemoryApiResources(Database.ApiResources)
                .AddInMemoryClients(Database.Clients)
                .AddTestUsers(Database.TestUsers);

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseIdentityServer();
        }
    }
}
