using System;
using System.IO;
using Autofac.Extensions.DependencyInjection;
using CLK.Autofac;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace CLK.AspNetCore
{
    public partial class AspNetCoreContext : IDisposable
    {
        // Fields
        private readonly IWebHost _webHost = null;


        // Constructors
        public AspNetCoreContext(string baseUrl, string controllerFilename, AutofacContext autofacContext)
        {
            #region Contracts

            if (string.IsNullOrEmpty(baseUrl) == true) throw new ArgumentException();
            if (string.IsNullOrEmpty(controllerFilename) == true) throw new ArgumentException();
            if (autofacContext == null) throw new ArgumentException();

            #endregion

            // WebHost
            _webHost = new WebHostBuilder(baseUrl, controllerFilename, autofacContext).Create();
            if (_webHost == null) throw new InvalidOperationException("_webHost=null");

            // Start
            _webHost.Start();
        }

        public void Dispose()
        {
            // Dispose
            _webHost.Dispose();
        }


        // Class
        private class WebHostBuilder : IStartup
        {
            // Fields
            private readonly string _baseUrl = null;

            private readonly string _controllerFilename = null;

            private readonly AutofacContext _autofacContext = null;


            // Constructors
            public WebHostBuilder(string baseUrl, string controllerFilename, AutofacContext autofacContext)
            {
                #region Contracts

                if (string.IsNullOrEmpty(baseUrl) == true) throw new ArgumentException();
                if (string.IsNullOrEmpty(controllerFilename) == true) throw new ArgumentException();
                if (autofacContext == null) throw new ArgumentException();

                #endregion

                // Default
                _baseUrl = baseUrl;
                _controllerFilename = controllerFilename;
                _autofacContext = autofacContext;
            }


            // Methods
            public IWebHost Create()
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

                    // Server
                    .UseKestrel()

                    // Listen
                    .UseUrls(_baseUrl)

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

                // Cors
                app.UseCors("Default");

                // Exception
                app.UseExceptionMiddleware();

                // FileServer
                app.UseFileServer();

                // Mvc   
                app.UseMvc();
            }

            public IServiceProvider ConfigureServices(IServiceCollection services)
            {
                #region Contracts

                if (services == null) throw new ArgumentException();

                #endregion

                // Cors
                services.AddCors(options => options.AddPolicy("Default", corsPolicyBuilder => corsPolicyBuilder
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
                .AddJsonFormatters()
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
}
