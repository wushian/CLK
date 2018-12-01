using System;
using System.IO;
using Autofac.Extensions.DependencyInjection;
using CLK.Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace CLK.AspNetCore
{
    internal class WebHostServiceProvider : IServiceProvider, ISupportRequiredService, IDisposable
    {
        // Fields
        private AutofacScope _serviceScope = null;

        private AutofacServiceProvider _serviceProvider = null;


        // Constructors
        public WebHostServiceProvider(AutofacScope autofacScope, IServiceCollection serviceCollection)
        {
            #region Contracts

            if (autofacScope == null) throw new ArgumentException();
            if (serviceCollection == null) throw new ArgumentException();

            #endregion

            // ServiceScope
            _serviceScope = autofacScope.BeginScope((autofacBuilder) =>
            {
                // Register
                autofacBuilder.RegisterServices(serviceCollection);
            });
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
                    // ServiceScope
                    _serviceScope.Start();

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
