using System;
using System.IO;
using Autofac.Extensions.DependencyInjection;
using CLK.Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace CLK.AspNetCore
{
    internal class WebHostBuilder : IStartup
    {
        // Fields
        private readonly WebHostOptions _options = null;


        // Constructors
        public WebHostBuilder(WebHostOptions options)
        {
            #region Contracts

            if (options == null) throw new ArgumentException();

            #endregion

            // Default
            _options = options;
        }


        // Methods
        public IWebHost Build()
        {
            // Create
            IWebHost webHost = null;
            {
                // Builder
                webHost = new Microsoft.AspNetCore.Hosting.WebHostBuilder()

                // Services
                .ConfigureServices((services) =>
                {
                    // Add
                    services.AddSingleton<IStartup>(this);
                })

                // Content
                .UseContentRoot(Directory.GetCurrentDirectory())

                // Listen
                .UseUrls(_options.ListenUrl)

                // Server
                .UseKestrel()

                // Build       
                .Build();
            }
            if (webHost == null) throw new InvalidOperationException("webHost=null");

            // Return
            return webHost;
        }

        public void Configure(IApplicationBuilder app)
        {
            #region Contracts

            if (app == null) throw new ArgumentException();

            #endregion

            // MiddlewareLauncher
            foreach (var middlewareLauncher in _options.MiddlewareLauncherList)
            {
                middlewareLauncher.ConfigureMiddleware(app);
            }
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            #region Contracts

            if (services == null) throw new ArgumentException();

            #endregion

            // MiddlewareLauncher
            foreach (var middlewareLauncher in _options.MiddlewareLauncherList)
            {
                middlewareLauncher.ConfigureServices(services);
            }

            // Return
            return new WebHostServiceProvider(_options.AutofacScope, services);
        }
    }

    

    

    

   
}
