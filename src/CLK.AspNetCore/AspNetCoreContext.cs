using System;
using Autofac.Extensions.DependencyInjection;
using CLK.Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace CLK.AspNetCore
{
    public partial class AspNetCoreContext
    {
        // Fields
        private readonly AutofacContext _autofacContext = null;

        private readonly string _controllerFilename = null;


        // Constructors
        public AspNetCoreContext(AutofacContext autofacContext, string controllerFilename)
        {
            #region Contracts

            if (autofacContext == null) throw new ArgumentException();
            if (string.IsNullOrEmpty(controllerFilename) == true) throw new ArgumentException();

            #endregion

            // Default
            _autofacContext = autofacContext;
            _controllerFilename = controllerFilename;
        }

        // Methods

    }

    public partial class AspNetCoreContext : IStartup
    {
        // Methods
        void IStartup.Configure(IApplicationBuilder app)
        {
            #region Contracts

            if (app == null) throw new ArgumentException();

            #endregion

            // Cors
            app.UseCors("Default");

            // Exception
            app.UseExceptionMiddleware();            

            // FileServer
            app.UseFileServer();

            // Mvc   
            app.UseMvc();
        }

        IServiceProvider IStartup.ConfigureServices(IServiceCollection services)
        {
            #region Contracts

            if (services == null) throw new ArgumentException();

            #endregion

            // Cors
            services.AddCors(corsOptions => corsOptions.AddPolicy("Default", corsPolicyBuilder => corsPolicyBuilder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
            ));

            // Mvc
            services.AddMvcCore(options =>
            {
                // Conventions
                options.Conventions.Clear();
                options.Conventions.AddNamespaceRoute();

                // Filters
                options.Filters.AddActionLogger();
            })
            .AddAssemblyController(_controllerFilename);

            // ServiceProvider
            IServiceProvider serviceProvider = null;
            {
                // Register
                _autofacContext.RegisterServices(services);

                // Create
                serviceProvider = new AutofacServiceProvider(_autofacContext.Container);
            }

            // Return
            return serviceProvider;
        }
    }
}
