using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DncIds4.ApiGatewayOcelot.Config;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace DncIds4.ApiGatewayOcelot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            this.IdentityServerConfig = this.Configuration.GetSection("IdentityServerConfig").Get<IdentityServerConfig>();
        }

        public IConfiguration Configuration { get; }
        private IdentityServerConfig IdentityServerConfig { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services
                .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication("TestKey", opts =>
                {
                    opts.Authority = this.IdentityServerConfig.IdentityServerUrl;
                    opts.RequireHttpsMetadata = false;
                    opts.ApiName = this.IdentityServerConfig.ApiName;
                    opts.ApiSecret = this.IdentityServerConfig.ClientSecret;
                    opts.SupportedTokens = SupportedTokens.Both;
                });
            services.AddOcelot(this.Configuration);
            services.AddCors(opts =>
            {
                opts.AddDefaultPolicy(cfg =>
                {
                    cfg.AllowAnyHeader();
                    cfg.AllowAnyOrigin();
                    cfg.AllowAnyMethod();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseOcelot().Wait();
        }
    }
}
