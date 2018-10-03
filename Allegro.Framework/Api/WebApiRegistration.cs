using Allegro.Framework.Authentication;
using Allegro.Framework.Swagger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using System.Reflection;

namespace Allegro.Framework.Api
{
    public abstract class WebApiRegistration
    {
        private readonly IConfiguration _configuration;
        private readonly SwaggerRegistration _swaggerRegistration;
        private readonly AuthenticationManager _authentication;

        /// <summary>
        /// Compliance on the requirement of allegro using authorization, logging and Swagger.
        /// </summary>
        /// <param name="configuration">parameter provided in the startup</param>
        public WebApiRegistration(IConfiguration configuration)
        {
            _configuration = configuration;
            _swaggerRegistration = SwaggerRegistration();
            _authentication = new AuthenticationManager();
        }

        /// <summary>
        /// runtime method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            var settings = new WebApiSettings(GetAssemblies());

            DependencyContainer(services, _configuration);

            _swaggerRegistration.Info = new Info { Title = settings.WebApiName };
            _swaggerRegistration.StartSwagger(services);

            if (EnableAuthoriztion())
            {
                _authentication.StartAuthentication(services);

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Member",
                        policy => policy.RequireClaim("MembershipId"));
                });
            }

            AddServices(services);

            services.AddMvc().AddApplicationPart(Assembly.Load(settings.ControllerNamespace));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        /// <summary>
        /// Need to add services on startup
        /// </summary>
        /// <param name="services"></param>
        public virtual void AddServices(IServiceCollection services) { }

        /// <summary>
        /// runtime method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();

            _swaggerRegistration.BuildSwagger(app);
            AddConfiguration(app, env);
        }

        /// <summary>
        /// need to add configuration on startup
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public virtual void AddConfiguration(IApplicationBuilder app, IHostingEnvironment env) { }

        /// <summary>
        /// Use to configure swagger.
        /// </summary>
        /// <returns></returns>
        public virtual SwaggerRegistration SwaggerRegistration()
        {
            return new SwaggerRegistration() { EnableAuthentication = EnableAuthoriztion() };
        }

        /// <summary>
        /// Contains all the DI of the solution
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public abstract void DependencyContainer(IServiceCollection services, IConfiguration configuration);

        /// <summary>
        /// Use to register assemblies
        /// </summary>
        /// <returns></returns>
        public abstract Dictionary<AssemblyType, Assembly> GetAssemblies();

        /// <summary>
        /// Enable/Disable the authorization of the apis.
        /// </summary>
        /// <returns></returns>
        public virtual bool EnableAuthoriztion()
        {
            return true;
        }
    }
}

