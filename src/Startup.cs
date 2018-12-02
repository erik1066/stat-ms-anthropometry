#pragma warning disable 1591 // disables the warnings about missing Xml code comments
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Foundation.Sdk;
using Foundation.Sdk.Data;
using Foundation.Sdk.Security;
using Foundation.AnthStat.WebUI.Security;
using Polly;
using Polly.Extensions;
using Polly.Extensions.Http;

namespace Foundation.AnthStat.WebUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Services to add</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddResponseCaching();

            /* If you set this microservice up to use an OAuth2 provider, such as Auth0, then 'useAuthorization' will be true and there is extra
             * work involved to configure your scope-based authorization model. That's all been deactivated for the sake of simplicity but this
             * at least shows you how it can be done.*/
            string authorizationDomain = Common.GetConfigurationVariable(Configuration, "OAUTH2_ACCESS_TOKEN_URI", "Auth:Domain", string.Empty);
            bool useAuthorization = !string.IsNullOrEmpty(authorizationDomain);

            string applicationName = Common.GetConfigurationVariable(Configuration, "APP_NAME", "AppName", "stat-nutritional-anthropometry");

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Nutritional Anthropometry Statistical Microservice",
                    Version = "v1",
                    Description = "A microservice for computing z-scores for children and adolescents using the WHO 2006 Child Growth Standards, the WHO 2007 Growth Reference, and the CDC 2000 Growth Charts. A z-score-to-percentile converter is also provided.",
                    Contact = new Contact
                    {
                        Name = "Erik Knudsen",
                        Email = string.Empty,
                        Url = "https://github.com/erik1066"
                    },
                    License = new License
                    {
                        Name = "Apache 2.0",
                        Url = "https://www.apache.org/licenses/LICENSE-2.0"
                    }
                });

                /* If you set this microservice up to use an OAuth2 provider, such as Auth0, and you have passed in the right config vars,
                 * then Swagger is automatically set up to include a login prompt via the code below. */
                if (useAuthorization)
                {
                    c.AddSecurityDefinition("Bearer", new ApiKeyScheme { In = "header", Description = "Please enter JWT with Bearer into field", Name = "Authorization", Type = "apiKey" });
                    c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                        { "Bearer", Enumerable.Empty<string>() },
                    });
                }

                // These two lines are necessary for Swagger to pick up the C# XML comments and show them in the Swagger UI. See https://github.com/domaindrivendev/Swashbuckle.AspNetCore for more details.
                var filePath = System.IO.Path.Combine(System.AppContext.BaseDirectory, "api.xml");
                c.IncludeXmlComments(filePath);

                c.EnableAnnotations();
            });

            services.AddMvc(options =>
            {
               options.InputFormatters.Add(new TextPlainInputFormatter());
            })
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            #region Health checks
            services.AddHealthChecks(checks =>
            {
                checks
                    .AddHealthCheckGroup(
                        "memory",
                        group => group.AddPrivateMemorySizeCheck(1)
                                    .AddVirtualMemorySizeCheck(128_000_000_000)
                                    .AddWorkingSetCheck(140_000_000),
                            CheckStatus.Unhealthy
                    );
            });
            #endregion // Health checks

            #region OAuth2 configuration
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = authorizationDomain;
                options.Audience = Common.GetConfigurationVariable(Configuration, "OAUTH2_CLIENT_ID", "Auth:ApiIdentifier", string.Empty);
            });

            /* These policy names match the names in the [Authorize] attribute(s) in the Controller classes.
             * The HasScopeHandler class is used (see below) to pass/fail the authorization check if authorization
             * has been enabled via the microservice's configuration.
             */
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Common.READ_AUTHORIZATION_NAME, policy => policy.Requirements.Add(new HasScopeRequirement(Common.READ_AUTHORIZATION_NAME, authorizationDomain)));
                options.AddPolicy(Common.INSERT_AUTHORIZATION_NAME, policy => policy.Requirements.Add(new HasScopeRequirement(Common.INSERT_AUTHORIZATION_NAME, authorizationDomain)));
                options.AddPolicy(Common.UPDATE_AUTHORIZATION_NAME, policy => policy.Requirements.Add(new HasScopeRequirement(Common.UPDATE_AUTHORIZATION_NAME, authorizationDomain)));
                options.AddPolicy(Common.DELETE_AUTHORIZATION_NAME, policy => policy.Requirements.Add(new HasScopeRequirement(Common.DELETE_AUTHORIZATION_NAME, authorizationDomain)));
            });

            // If the developer has not configured OAuth2, then disable authentication and authorization
            if (useAuthorization)
            {
                services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
            }
            else
            {
                services.AddSingleton<IAuthorizationHandler, AlwaysAllowHandler>();
            }
            #endregion // OAuth2 configuration
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCors("CorsPolicy");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // app.UseHttpsRedirection(); // use for production but for R&D and testing, can be a problem
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Nutritional Anthropometry Statistical API V1");
            });

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
#pragma warning restore 1591
