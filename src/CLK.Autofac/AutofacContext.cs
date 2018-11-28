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
    public class AutofacContext : AutofacScope
    {
        // Fields
        private List<Action<ContainerBuilder>> _configurationActionList = new List<Action<ContainerBuilder>>();


        // Constructors
        public AutofacContext() : this(new List<Action<ContainerBuilder>>())
        {

        }

        private AutofacContext(List<Action<ContainerBuilder>> configurationActionList) : base(() => { return Build(configurationActionList); })
        {
            #region Contracts

            if (configurationActionList == null) throw new ArgumentException();

            #endregion

            // Default
            _configurationActionList = configurationActionList;
        }


        // Methods
        private static ILifetimeScope Build(List<Action<ContainerBuilder>> configurationActionList)
        {
            #region Contracts

            if (configurationActionList == null) throw new ArgumentException();

            #endregion

            // AutofacBuilder    
            var autofacBuilder = new ContainerBuilder();
            foreach (var configurationAction in configurationActionList)
            {
                // Configure
                configurationAction(autofacBuilder);
            }

            // AutofacContainer
            var autofacContainer = autofacBuilder.Build();
            if (autofacContainer == null) throw new InvalidOperationException();

            // Return
            return autofacContainer;
        }


        public void RegisterConfig(string configFilename)
        {
            #region Contracts

            if (string.IsNullOrEmpty(configFilename) == true) throw new ArgumentException();

            #endregion

            // Require
            if (this.Container != null) throw new InvalidOperationException("this.Container!=null");

            // Attach
            _configurationActionList.Add((autofacBuilder) =>
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
            if (this.Container != null) throw new InvalidOperationException("this.Container!=null");

            // Attach
            _configurationActionList.Add((autofacBuilder) =>
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
            if (this.Container != null) throw new InvalidOperationException("this.Container!=null");

            // Attach
            _configurationActionList.Add((autofacBuilder) =>
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
            if (this.Container != null) throw new InvalidOperationException("this.Container!=null");

            // Attach
            _configurationActionList.Add((autofacBuilder) =>
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
            if (this.Container != null) throw new InvalidOperationException("this.Container!=null");

            // Attach
            _configurationActionList.Add((autofacBuilder) =>
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
            if (this.Container != null) throw new InvalidOperationException("this.Container!=null");

            // Attach
            _configurationActionList.Add((autofacBuilder) =>
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
            if (this.Container != null) throw new InvalidOperationException("this.Container!=null");

            // Attach
            _configurationActionList.Add((autofacBuilder) =>
            {
                // Register
                autofacBuilder.RegisterServices(serviceList);
            });
        }
    }
}
