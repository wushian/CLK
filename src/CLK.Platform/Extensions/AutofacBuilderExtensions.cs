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

namespace CLK.Platform
{
    public static class AutofacBuilderExtensions
    {
        // Methods
        public static void RegisterConfig(this ContainerBuilder autofacBuilder, string configFilename)
        {
            #region Contracts

            if (autofacBuilder == null) throw new ArgumentException();
            if (string.IsNullOrEmpty(configFilename) == true) throw new ArgumentException();

            #endregion
            
            // ConfigFileList
            var configFileList = FileHelper.GetAllFile(configFilename);
            if (configFileList == null) throw new InvalidOperationException();

            // Execute
            foreach (var configFile in configFileList)
            {
                // ConfigBuilder    
                var configBuilder = new ConfigurationBuilder();
                configBuilder.AddJsonFile(configFile.FullName);

                // AutofacConfig
                var autofacConfig = configBuilder.Build();
                if (autofacConfig == null) throw new InvalidOperationException();

                // Register
                autofacBuilder.RegisterModule(new ConfigurationModule(autofacConfig));
            }
        }

        public static void RegisterAssemblyTypes(this ContainerBuilder autofacBuilder, Type type, string assemblyFilename)
        {
            #region Contracts

            if (autofacBuilder == null) throw new ArgumentException();
            if (type == null) throw new ArgumentException();
            if (string.IsNullOrEmpty(assemblyFilename) == true) throw new ArgumentException();

            #endregion

            // AssemblyFileList
            var assemblyFileList = FileHelper.GetAllFile(assemblyFilename);
            if (assemblyFileList == null) throw new InvalidOperationException();

            // Execute
            foreach (var assemblyFile in assemblyFileList)
            {
                // Require
                if (File.Exists(assemblyFile.FullName) == false) continue;

                // Assembly
                Assembly assembly = null;
                try
                {
                    assembly = Assembly.LoadFile(assemblyFile.FullName);
                    if (assembly == null) throw new InvalidOperationException();
                }
                catch
                {
                    continue;
                }

                // Register
                autofacBuilder.RegisterAssemblyTypes(assembly).As(type);
            }
        }

        public static void RegisterAssemblyModules(this ContainerBuilder autofacBuilder, Type type, string assemblyFilename)
        {
            #region Contracts

            if (autofacBuilder == null) throw new ArgumentException();
            if (type == null) throw new ArgumentException();
            if (string.IsNullOrEmpty(assemblyFilename) == true) throw new ArgumentException();

            #endregion
            
            // AssemblyFileList
            var assemblyFileList = FileHelper.GetAllFile(assemblyFilename);
            if (assemblyFileList == null) throw new InvalidOperationException();

            // Execute
            foreach (var assemblyFile in assemblyFileList)
            {
                // Assembly
                Assembly assembly = null;
                try
                {
                    assembly = Assembly.LoadFile(assemblyFile.FullName);
                    if (assembly == null) throw new InvalidOperationException();
                }
                catch
                {
                    continue;
                }

                // Register
                autofacBuilder.RegisterAssemblyModules(type, assembly);
            }
        }


        public static void RegisterType(this ContainerBuilder autofacBuilder, Type type)
        {
            #region Contracts

            if (autofacBuilder == null) throw new ArgumentException();
            if (type == null) throw new ArgumentException();

            #endregion

            // Register
            autofacBuilder.RegisterType(type);
        }

        public static void RegisterInstance(this ContainerBuilder autofacBuilder, Type type, object instance)
        {
            #region Contracts

            if (autofacBuilder == null) throw new ArgumentException();
            if (type == null) throw new ArgumentException();
            if (instance == null) throw new ArgumentException();

            #endregion

            // Register
            autofacBuilder.RegisterInstance(instance).AsImplementedInterfaces();
        }

        public static void RegisterGeneric(this ContainerBuilder autofacBuilder, Type type, Type implementer)
        {
            #region Contracts

            if (autofacBuilder == null) throw new ArgumentException();
            if (type == null) throw new ArgumentException();
            if (implementer == null) throw new ArgumentException();

            #endregion

            // Register
            autofacBuilder.RegisterGeneric(implementer).As(type);
        }


        public static void RegisterServices(this ContainerBuilder autofacBuilder, IServiceCollection serviceList)
        {
            #region Contracts

            if (autofacBuilder == null) throw new ArgumentException();
            if (serviceList == null) throw new ArgumentException();

            #endregion

            // Register
            autofacBuilder.Populate(serviceList);
        }
    }
}
