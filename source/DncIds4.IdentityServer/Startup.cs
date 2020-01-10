using DncIds4.IdentityServer.Config;
using DncIds4.IdentityServer.Data;
using DncIds4.IdentityServer.Services;
using DncIds4.IdentityServer.StartupTasks;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using DncIds4.Common.Consul;
using DncIds4.IdentityServer.Securities.Admin;

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
            services.AddConsul(this.Configuration);
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
                    policy.RequireClaim("iss", this.IdentityServerConfig.IdentityServerUrl);
                    policy.AddRequirements(
                        new IsAdminRequirement(ApiRoleDefinition.ApiRoles[ApiRoleDefinition.Roles.Admin]));
                });

                opts.AddPolicy("For_User", policy =>
                {
                    policy.AddAuthenticationSchemes(IdentityServerAuthenticationDefaults.AuthenticationScheme);
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(x => (x.Type == Constants.IdentityResource.UserRoles || x.Type == $"client_{Constants.IdentityResource.UserRoles}")
                                                   && x.Value == ApiRoleDefinition.ApiRoles[ApiRoleDefinition.Roles.User]));
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
                .AddAspNetIdentity<IdentityUser>();

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

            services.AddHostedService<IdentityDbMigratorHostedService>();
            services.AddScoped<AccountService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddSingleton<IAuthorizationHandler, IsClientAdminClaimAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, IsUserAdminClaimAuthorizationHandler>();
            services.AddCors(opts =>
            {
                opts.AddPolicy("Cors_Policy", cfg =>
                {
                    cfg.AllowAnyHeader();
                    cfg.AllowAnyOrigin();
                    cfg.AllowAnyMethod();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(cfg =>
            {
                cfg.SwaggerEndpoint("/swagger/V1/swagger.json", "IdentityServer4");
                cfg.OAuthClientId(this.IdentityServerConfig.ClientId);
                cfg.OAuthClientSecret(this.IdentityServerConfig.ClientSecret);
            });
            app.UseCors("Cors_Policy");
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
