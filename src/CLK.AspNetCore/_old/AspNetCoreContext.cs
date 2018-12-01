using System;
using System.IO;
using Autofac.Extensions.DependencyInjection;
using CLK.Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace CLK.AspNetCore
{
    public class AspNetCoreContext : IDisposable
    {
        // Fields
        private WebHostBuilder _webHostBuilder = null;

        private IWebHost _webHost = null;


        // Constructors
        public AspNetCoreContext(AutofacContext autofacContext, AspNetCoreOptions aspNetCoreOptions)
        {
            #region Contracts

            if (autofacContext == null) throw new ArgumentException();
            if (aspNetCoreOptions == null) throw new ArgumentException();

            #endregion

            // WebHostBuilder
            _webHostBuilder = new WebHostBuilder(autofacContext, aspNetCoreOptions);
        }

        public void Start()
        {
            // Require
            if (_webHost != null) return;

            // WebHost
            _webHost = _webHostBuilder.Build();
            if (_webHost == null) throw new InvalidOperationException("_webHost=null");

            // Start
            _webHost.Start();
        }

        public void Dispose()
        {
            // Require
            if (_webHost == null) return;

            // Dispose
            _webHost.Dispose();
        }


        // Class
        private class WebHostBuilder : IStartup
        {
            // Fields
            private readonly AutofacContext _autofacContext = null;

            private readonly AspNetCoreOptions _aspNetCoreOptions = null;


            // Constructors
            public WebHostBuilder(AutofacContext autofacContext, AspNetCoreOptions aspNetCoreOptions)
            {
                #region Contracts

                if (autofacContext == null) throw new ArgumentException();
                if (aspNetCoreOptions == null) throw new ArgumentException();

                #endregion

                // Default
                _autofacContext = autofacContext;
                _aspNetCoreOptions = aspNetCoreOptions;
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

                    // Server
                    .UseKestrel()

                    // Listen
                    .UseUrls(_aspNetCoreOptions.ListenUrl)

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
                    options.Filters.AddActionLog();
                })
                .AddJsonFormatters()
                .AddAssemblyControllers(_aspNetCoreOptions.ControllerFileName);
                
                // Return
                return new WebHostServiceProvider(_autofacContext, services);
            }
        }

        private class WebHostServiceProvider : IServiceProvider, ISupportRequiredService, IDisposable
        {
            // Fields
            private AutofacScope _serviceScope = null;

            private AutofacServiceProvider _serviceProvider = null;


            // Constructors
            public WebHostServiceProvider(AutofacContext autofacContext, IServiceCollection serviceCollection)
            {
                #region Contracts

                if (autofacContext == null) throw new ArgumentException();
                if (serviceCollection == null) throw new ArgumentException();

                #endregion

                // ServiceScope
                _serviceScope = autofacContext.BeginScope((autofacBuilder) =>
                {
                    // Register
                    autofacBuilder.RegisterServices(serviceCollection);
                });
                _serviceScope.Start();
            }

            public void Dispose()
            {
                // Dispose
                _serviceProvider?.Dispose();
                _serviceScope?.Dispose();
            }


            // Properties
            private AutofacServiceProvider ServiceProvider
            {
                get
                {
                    // Create
                    if (_serviceProvider == null)
                    {
                        // ServiceProvider
                        _serviceProvider = new AutofacServiceProvider(_serviceScope.Container);
                    }
                    if (_serviceProvider == null) throw new InvalidOperationException("_serviceProvider=null");

                    // Return
                    return _serviceProvider;
                }
            }


            // Methods
            public object GetService(Type serviceType)
            {
                #region Contracts

                if (serviceType == null) throw new ArgumentException();

                #endregion

                // Return
                return this.ServiceProvider.GetService(serviceType);
            }

            public object GetRequiredService(Type serviceType)
            {
                #region Contracts

                if (serviceType == null) throw new ArgumentException();

                #endregion

                // Return
                return this.ServiceProvider.GetRequiredService(serviceType);
            }
        }
    }
}
