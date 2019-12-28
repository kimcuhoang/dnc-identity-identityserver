using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DncIds4.IdentityServer.Config;
using DncIds4.IdentityServer.Data;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace DncIds4.IdentityServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            this.ConnectionString = this.Configuration.GetConnectionString("Default");
            this.MigrationAssembly = this.GetType().Assembly.GetName().Name;
            this.IdentityServerConfig = this.Configuration.GetSection("IdentityServerConfig").Get<IdentityServerConfig>();
        }

        private string ConnectionString { get; }
        private string MigrationAssembly { get; }
        private IdentityServerConfig IdentityServerConfig { get; }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(cfg =>
            {
                var guestPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(IdentityServerAuthenticationDefaults.AuthenticationScheme)
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
                opts.AddPolicy("For_Admin", policy =>
                {
                    policy.AddAuthenticationSchemes(IdentityServerAuthenticationDefaults.AuthenticationScheme);
                    policy.RequireClaim("scope", this.IdentityServerConfig.ApiName);
                    policy.RequireClaim("role", "api::admin");
                });

                opts.AddPolicy("For_User", policy =>
                {
                    policy.AddAuthenticationSchemes(IdentityServerAuthenticationDefaults.AuthenticationScheme);
                    policy.RequireClaim("role", "api::user");
                });
            });

            services.AddDbContext<ApplicationDbContext>(opts =>
                {
                    opts.UseSqlServer(this.ConnectionString,
                        builder => { builder.MigrationsAssembly(this.MigrationAssembly); });
                });

            services
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services
                .AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(Database.IdentityResources)
                .AddInMemoryApiResources(Database.ApiResources)
                .AddInMemoryClients(Database.Clients)
                //.AddAspNetIdentity<IdentityUser>();
                .AddTestUsers(Database.TestUsers);

            services.AddSwaggerGen(opts =>
            {
                opts.SwaggerDoc("V1", new OpenApiInfo
                {
                    Title = "IdentityServer4",
                    Version = "V1"
                });

                //Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                opts.IncludeXmlComments(xmlPath);

                opts.AddSecurityDefinition("oauth2ClientCredential", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        ClientCredentials = new OpenApiOAuthFlow
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
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "oauth2ClientCredential"}
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using var serviceScope = serviceScopeFactory.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();

            app.UseSwagger();
            app.UseSwaggerUI(cfg =>
            {
                cfg.SwaggerEndpoint("/swagger/V1/swagger.json", "IdentityServer4");
                cfg.OAuthClientId(this.IdentityServerConfig.ClientId);
                cfg.OAuthClientSecret(this.IdentityServerConfig.ClientSecret);
            });

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
