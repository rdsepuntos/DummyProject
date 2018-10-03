using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Allegro.Framework.Swagger
{
    public sealed class SwaggerRegistration
    {
        #region private variables
        private SwaggerEndpoint _swaggerEndpoint;
        private SwaggerAuthentication _swaggerAuthentication;
        private Info _info;
        private string _version = "v1";
        #endregion

        #region setting default value
        public SwaggerEndpoint SwaggerEndpoint
        {
            get
            {
                return _swaggerEndpoint ?? new SwaggerEndpoint
                {
                    Name = $"{Info.Title} {Version}",
                    Route = "swagger",
                    Url = "/swagger/v1/swagger.json"
                };
            }
            set
            {
                _swaggerEndpoint = value;
            }
        }

        public SwaggerAuthentication SwaggerAuthentication
        {
            get
            {
                return _swaggerAuthentication ?? new SwaggerAuthentication
                {
                    Name = "Bearer",
                    Requirement = new Dictionary<string, IEnumerable<string>>
                    {
                       { "Bearer", Enumerable.Empty<string>() }
                    },
                    ApiKeyScheme = new ApiKeyScheme
                    {
                        In = "header",
                        Description = "Please enter JWT with Bearer into field",
                        Name = "Authorization",
                        Type = "apiKey"
                    }
                };
            }
            set
            {
                _swaggerAuthentication = value;
            }
        }

        public Info Info
        {
            get
            {
                return _info ?? new Info
                {
                    Title = "Service"
                };
            }
            set
            {
                _info = value;
            }
        }

        public string Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
            }
        }

        internal bool EnableAuthentication { get; set; }
        #endregion

        #region methods for swagger
        public void StartSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(Version, Info);

                if (EnableAuthentication)
                {
                    c.AddSecurityDefinition(SwaggerAuthentication.Name, SwaggerAuthentication.ApiKeyScheme);
                    c.AddSecurityRequirement(SwaggerAuthentication.Requirement);
                }
            });
        }

        public void BuildSwagger(IApplicationBuilder app)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(SwaggerEndpoint.Url, SwaggerEndpoint.Name);
                c.RoutePrefix = SwaggerEndpoint.Route;
            });
        }
        #endregion
    }
}
