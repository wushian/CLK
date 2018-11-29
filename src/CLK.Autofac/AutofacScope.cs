using Autofac;
using Autofac.Configuration;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Autofac
{
    public class AutofacScope : IDisposable
    {
        // Fields
        private readonly Func<ILifetimeScope> _buildAction = null;


        // Constructors
        public AutofacScope(Func<ILifetimeScope> buildAction)
        {
            #region Contracts

            if (buildAction == null) throw new ArgumentException();

            #endregion

            // Default
            _buildAction = buildAction;
        }

        public void Start()
        {
            // Container
            if (this.Container == null)
            {
                this.Container = _buildAction();
            }
            if (this.Container == null) throw new InvalidOperationException("this.Container=null");
        }

        public void Dispose()
        {
            // Dispose
            this.Container?.Dispose();
        }


        // Properties
        public ILifetimeScope Container { get; private set; }


        // Methods
        public AutofacScope BeginScope(Action<ContainerBuilder> configurationAction)
        {
            #region Contracts

            if (configurationAction == null) throw new ArgumentException();

            #endregion

            // Require
            if (this.Container == null) throw new InvalidOperationException("this.Container=null");

            // BuildAction
            Func<ILifetimeScope> buildAction = () =>
            {
                return this.Container.BeginLifetimeScope(configurationAction);
            };

            // Return
            return new AutofacScope(buildAction);
        }
        
        
        public bool IsRegistered<TComponent>() where TComponent : class
        {
            // Require
            if (this.Container == null) throw new InvalidOperationException("this.Container=null");

            // Return
            return this.Container.IsRegistered<TComponent>();
        }

        public TComponent Resolve<TComponent>() where TComponent : class
        {
            // Require
            if (this.Container == null) throw new InvalidOperationException("this.Container=null");
            if (this.Container.IsRegistered<TComponent>() == false) return null;

            // Return
            return this.Container.Resolve<TComponent>();
        }
    }
}
