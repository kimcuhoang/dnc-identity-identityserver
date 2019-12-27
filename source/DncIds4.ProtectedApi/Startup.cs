using DncIds4.ProtectedApi.Config;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;

namespace DncIds4.ProtectedApi
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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(cfg =>
            {
                var guestPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireClaim("scope", this.IdentityServerConfig.ApiName)
                    .Build();
                cfg.Filters.Add(new AuthorizeFilter(guestPolicy));
            });

            services
                .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(opts =>
                {
                    opts.Authority = this.IdentityServerConfig.IdentityServerUrl;
                    opts.RequireHttpsMetadata = false;
                    opts.ApiName = this.IdentityServerConfig.ApiName;
                    opts.ApiSecret = this.IdentityServerConfig.ClientSecret;
                });

            services.AddAuthorization(opts =>
            {
                opts.AddPolicy("For_Admin", policy => { policy.RequireClaim("role", "api::admin"); });

                opts.AddPolicy("For_User", policy => { policy.RequireClaim("role", "api::user"); });
            });

            services.AddSwaggerGen(opts =>
            {
                opts.SwaggerDoc("V1", new OpenApiInfo
                {
                    Title = "Example of IdentityServer Integration",
                    Version = "V1"
                });

                opts.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(this.IdentityServerConfig.AuthorizeUrl, UriKind.Absolute),
                            TokenUrl = new Uri(this.IdentityServerConfig.TokenUrl, UriKind.Absolute),
                        }
                    },
                });

                opts.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "oauth2"}
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(cfg =>
            {
                cfg.SwaggerEndpoint("/swagger/V1/swagger.json", "Example of Identity Client");
                cfg.OAuthClientId(this.IdentityServerConfig.ClientId);
                cfg.OAuthClientSecret(this.IdentityServerConfig.ClientSecret);
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
