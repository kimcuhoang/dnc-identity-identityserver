using DncIds4.ApiGatewayOcelot.Config;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            services
                .AddAuthentication()
                .AddIdentityServerAuthentication("ResourceApi", opts =>
                {
                    opts.Authority = this.IdentityServerConfig.IdentityServerUrl;
                    opts.RequireHttpsMetadata = false;
                    opts.ApiName = "ResourceApi";
                    opts.ApiSecret = this.IdentityServerConfig.ClientSecret;
                    opts.SupportedTokens = SupportedTokens.Both;
                });

            services.AddOcelot();
                

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
            app.UseAuthentication();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
            app.UseOcelot().Wait();
        }
    }
}
