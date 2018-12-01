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
    public static class AutofacBuilderExtensions
    {
        // Methods
        public static void RegisterConfig(this ContainerBuilder autofacBuilder, string configFileName)
        {
            #region Contracts

            if (autofacBuilder == null) throw new ArgumentException();
            if (string.IsNullOrEmpty(configFileName) == true) throw new ArgumentException();

            #endregion

            // ConfigFileList
            var configFileList = FileHelper.GetAllFile(configFileName);
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

        public static void RegisterAssemblyTypes(this ContainerBuilder autofacBuilder, Type type, string assemblyFileName)
        {
            #region Contracts

            if (autofacBuilder == null) throw new ArgumentException();
            if (type == null) throw new ArgumentException();
            if (string.IsNullOrEmpty(assemblyFileName) == true) throw new ArgumentException();

            #endregion

            // AssemblyFileList
            var assemblyFileList = FileHelper.GetAllFile(assemblyFileName);
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

        public static void RegisterAssemblyModules(this ContainerBuilder autofacBuilder, Type type, string assemblyFileName)
        {
            #region Contracts

            if (autofacBuilder == null) throw new ArgumentException();
            if (type == null) throw new ArgumentException();
            if (string.IsNullOrEmpty(assemblyFileName) == true) throw new ArgumentException();

            #endregion
            
            // AssemblyFileList
            var assemblyFileList = FileHelper.GetAllFile(assemblyFileName);
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


        public static void RegisterInstance(this ContainerBuilder autofacBuilder, Type type, object instance)
        {
            #region Contracts

            if (autofacBuilder == null) throw new ArgumentException();
            if (type == null) throw new ArgumentException();
            if (instance == null) throw new ArgumentException();

            #endregion

            // Register
            autofacBuilder.RegisterInstance(instance).As(type);
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
