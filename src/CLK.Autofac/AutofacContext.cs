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
using AutofacCore = Autofac;

namespace CLK.Autofac
{
    public class AutofacContext : IDisposable
    {
        // Fields
        private readonly List<Action<ContainerBuilder>> _registerDelegateList = new List<Action<ContainerBuilder>>();

        private readonly List<string> _configFileList = new List<string>();

        private IContainer _container = null;


        // Constructors
        public AutofacContext(string configFilename = null)
        {
            // ConfigFilename
            if (string.IsNullOrEmpty(configFilename) == false)
            {
                // RegisterConfig
                this.RegisterConfig(configFilename);
            }

            // EntryConfigFilename
            var entryConfigFilename = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location) + ".json";
            if (string.IsNullOrEmpty(entryConfigFilename) == false)
            {
                // RegisterConfig
                this.RegisterConfig(entryConfigFilename);
            }            
        }

        public void Dispose()
        {
            // Dispose
            if (_container != null)
            {
                _container.Dispose();
                _container = null;
            }
        }


        // Properties
        public IContainer Container
        {
            get
            {
                // Create
                if (_container == null)
                {
                    _container = this.CreateContainer();
                    if (_container == null) throw new InvalidOperationException();
                }

                // Return
                return _container;
            }
        }


        // Methods
        private IContainer CreateContainer()
        {
            // AutofacBuilder    
            var autofacBuilder = new ContainerBuilder();
            foreach (var registerDelegate in _registerDelegateList)
            {
                // Fegister
                registerDelegate(autofacBuilder);
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
            if (_container != null) throw new InvalidOperationException();

            // Delegate
            _registerDelegateList.Add((autofacBuilder) =>
            {
                this.RegisterConfig(configFilename, autofacBuilder);
            });
        }

        private void RegisterConfig(string configFilename, ContainerBuilder autofacBuilder)
        {
            #region Contracts

            if (string.IsNullOrEmpty(configFilename) == true) throw new ArgumentException();
            if (autofacBuilder == null) throw new ArgumentException();

            #endregion

            // ConfigFileList
            var configFileList = this.GetAllFile(configFilename);
            if (configFileList == null) throw new InvalidOperationException();

            // RegisterConfig
            foreach (var configFile in configFileList)
            {
                // Require
                if (_configFileList.Contains(configFile.Name.ToLower()) == true)
                {
                    continue;
                }
                _configFileList.Add(configFile.Name.ToLower());

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


        public void RegisterAssemblyTypes(string assemblyFilename, Type type)
        {
            #region Contracts

            if (string.IsNullOrEmpty(assemblyFilename) == true) throw new ArgumentException();
            if (type == null) throw new ArgumentException();

            #endregion

            // Require
            if (_container != null) throw new InvalidOperationException();

            // Delegate
            _registerDelegateList.Add((autofacBuilder) =>
            {
                this.RegisterAssemblyTypes(assemblyFilename, type, autofacBuilder);
            });
        }

        private void RegisterAssemblyTypes(string assemblyFilename, Type type, ContainerBuilder autofacBuilder)
        {
            #region Contracts

            if (string.IsNullOrEmpty(assemblyFilename) == true) throw new ArgumentException();
            if (type == null) throw new ArgumentException();
            if (autofacBuilder == null) throw new ArgumentException();

            #endregion

            // AssemblyFileList
            var assemblyFileList = this.GetAllFile(assemblyFilename);
            if (assemblyFileList == null) throw new InvalidOperationException();

            // AssemblyList
            var assemblyList = new List<Assembly>();
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
                assemblyList.Add(assembly);
            }

            // Register
            foreach (var assembly in assemblyList)
            {
                autofacBuilder
                    .RegisterAssemblyTypes(assembly)
                    .Where(assemblyType => assemblyType.IsSubclassOf(type));
            }
        }


        public void RegisterType(Type type)
        {
            #region Contracts

            if (type == null) throw new ArgumentException();

            #endregion

            // Require
            if (_container != null) throw new InvalidOperationException();

            // Delegate
            _registerDelegateList.Add((autofacBuilder) =>
            {
                this.RegisterType(type, autofacBuilder);
            });
        }

        private void RegisterType(Type type, ContainerBuilder autofacBuilder)
        {
            #region Contracts

            if (type == null) throw new ArgumentException();
            if (autofacBuilder == null) throw new ArgumentException();

            #endregion

            // Register
            autofacBuilder.RegisterType(type);
        }


        public void RegisterInstance(object instance, Type type)
        {
            #region Contracts

            if (instance == null) throw new ArgumentException();
            if (type == null) throw new ArgumentException();

            #endregion

            // Require
            if (_container != null) throw new InvalidOperationException();

            // Delegate
            _registerDelegateList.Add((autofacBuilder) =>
            {
                this.RegisterInstance(instance, type, autofacBuilder);
            });
        }

        private void RegisterInstance(object instance, Type type, ContainerBuilder autofacBuilder)
        {
            #region Contracts

            if (instance == null) throw new ArgumentException();
            if (type == null) throw new ArgumentException();
            if (autofacBuilder == null) throw new ArgumentException();

            #endregion

            // Register
            autofacBuilder.RegisterInstance(instance).AsImplementedInterfaces();
        }


        public void RegisterModule(AutofacCore.Module module)
        {
            #region Contracts

            if (module == null) throw new ArgumentException();

            #endregion

            // Require
            if (_container != null) throw new InvalidOperationException();

            // Delegate
            _registerDelegateList.Add((autofacBuilder) =>
            {
                this.RegisterModule(module, autofacBuilder);
            });
        }

        private void RegisterModule(AutofacCore.Module module, ContainerBuilder autofacBuilder)
        {
            #region Contracts

            if (module == null) throw new ArgumentException();
            if (autofacBuilder == null) throw new ArgumentException();

            #endregion

            // Register
            autofacBuilder.RegisterModule(module);
        }


        public void RegisterServices(IServiceCollection serviceList)
        {
            #region Contracts

            if (serviceList == null) throw new ArgumentException();

            #endregion

            // Require
            if (_container != null) throw new InvalidOperationException();

            // Delegate
            _registerDelegateList.Add((autofacBuilder) =>
            {
                this.RegisterServices(serviceList, autofacBuilder);
            });
        }

        private void RegisterServices(IServiceCollection serviceList, ContainerBuilder autofacBuilder)
        {
            #region Contracts

            if (serviceList == null) throw new ArgumentException();
            if (autofacBuilder == null) throw new ArgumentException();

            #endregion

            // Register
            autofacBuilder.Populate(serviceList);
        }


        private List<FileInfo> GetAllFile(string filename)
        {
            #region Contracts

            if (string.IsNullOrEmpty(filename) == true) throw new ArgumentException();

            #endregion

            // Result
            var fileList = new List<FileInfo>();

            // SearchPatternList
            var searchPatternList = filename.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (searchPatternList == null) throw new InvalidOperationException();

            // EntryDirectory           
            foreach (var searchPattern in searchPatternList)
            {
                // SearchFile 
                var searchFileDirectory = new DirectoryInfo(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location));
                var searchFileList = searchFileDirectory?.GetFiles(searchPattern, SearchOption.AllDirectories);
                if (searchFileList == null) throw new InvalidOperationException();

                // Add
                fileList.AddRange(searchFileList);
            }

            // CurrentDirectory         
            foreach (var searchPattern in searchPatternList)
            {
                // SearchFile 
                var searchFileDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
                var searchFileList = searchFileDirectory?.GetFiles(searchPattern, SearchOption.AllDirectories);
                if (searchFileList == null) throw new InvalidOperationException();

                // Add
                fileList.AddRange(searchFileList);
            }

            // Return
            return fileList;
        }
    }
}
