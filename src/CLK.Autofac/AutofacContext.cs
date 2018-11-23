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
    public class AutofacContext : IDisposable
    {
        // Fields
        private readonly List<Action<ContainerBuilder>> _registerDelegateList = new List<Action<ContainerBuilder>>();

        private IContainer _container = null;


        // Constructors
        public AutofacContext()
        {

        }

        public void Start()
        {
            // Start
            var container = this.Container;
            if (container == null) throw new InvalidOperationException("container=null");
        }

        public void Dispose()
        {
            // Dispose
            _container?.Dispose();
        }


        // Properties
        public IContainer Container
        {
            get
            {
                // Create
                if (_container == null)
                {
                    // AutofacBuilder    
                    var autofacBuilder = new ContainerBuilder();
                    foreach (var registerDelegate in _registerDelegateList)
                    {
                        // Register
                        registerDelegate(autofacBuilder);
                    }

                    // AutofacContainer
                    var autofacContainer = autofacBuilder.Build();
                    if (autofacContainer == null) throw new InvalidOperationException();

                    // Setting
                    _container = autofacContainer;
                }

                // Return
                return _container;
            }
        }


        // Methods
        public TComponent Resolve<TComponent>() where TComponent : class
        {
            // Require
            if (this.Container.IsRegistered<TComponent>() == false) return null;

            // Return
            return this.Container.Resolve<TComponent>();
        }


        public void RegisterConfig(string configFilename)
        {
            #region Contracts

            if (string.IsNullOrEmpty(configFilename) == true) throw new ArgumentException();

            #endregion

            // Require
            if (_container != null) throw new InvalidOperationException();

            // Attach
            _registerDelegateList.Add((autofacBuilder) =>
            {
                // Register
                autofacBuilder.RegisterConfig(configFilename);
            });
        }

        public void RegisterAssemblyTypes(Type type, string assemblyFilename)
        {
            #region Contracts

            if (type == null) throw new ArgumentException();
            if (string.IsNullOrEmpty(assemblyFilename) == true) throw new ArgumentException();

            #endregion

            // Require
            if (_container != null) throw new InvalidOperationException();

            // Attach
            _registerDelegateList.Add((autofacBuilder) =>
            {
                // Register
                autofacBuilder.RegisterAssemblyTypes(type, assemblyFilename);
            });
        }

        public void RegisterAssemblyModules(Type type, string assemblyFilename)
        {
            #region Contracts

            if (type == null) throw new ArgumentException();
            if (string.IsNullOrEmpty(assemblyFilename) == true) throw new ArgumentException();

            #endregion

            // Require
            if (_container != null) throw new InvalidOperationException();

            // Attach
            _registerDelegateList.Add((autofacBuilder) =>
            {
                // Register
                autofacBuilder.RegisterAssemblyModules(type, assemblyFilename);
            });
        }


        public void RegisterType(Type type)
        {
            #region Contracts

            if (type == null) throw new ArgumentException();

            #endregion

            // Require
            if (_container != null) throw new InvalidOperationException();

            // Attach
            _registerDelegateList.Add((autofacBuilder) =>
            {
                // Register
                autofacBuilder.RegisterType(type);
            });
        }

        public void RegisterInstance(Type type, object instance)
        {
            #region Contracts

            if (type == null) throw new ArgumentException();
            if (instance == null) throw new ArgumentException();

            #endregion

            // Require
            if (_container != null) throw new InvalidOperationException();

            // Attach
            _registerDelegateList.Add((autofacBuilder) =>
            {
                // Register
                autofacBuilder.RegisterInstance(type, instance);
            });
        }

        public void RegisterGeneric(Type type, Type implementer)
        {
            #region Contracts

            if (type == null) throw new ArgumentException();
            if (implementer == null) throw new ArgumentException();

            #endregion

            // Require
            if (_container != null) throw new InvalidOperationException();

            // Attach
            _registerDelegateList.Add((autofacBuilder) =>
            {
                // Register
                autofacBuilder.RegisterInstance(type, implementer);
            });
        }


        public void RegisterServices(IServiceCollection serviceList)
        {
            #region Contracts

            if (serviceList == null) throw new ArgumentException();

            #endregion

            // Require
            if (_container != null) throw new InvalidOperationException();

            // Attach
            _registerDelegateList.Add((autofacBuilder) =>
            {
                // Register
                autofacBuilder.RegisterServices(serviceList);
            });
        }
    }
}
