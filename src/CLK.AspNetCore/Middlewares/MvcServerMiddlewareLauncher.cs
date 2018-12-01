using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CLK.Autofac;
using CLK.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CLK.AspNetCore
{
    public class MvcServerMiddlewareLauncher : MiddlewareLauncher
    {
        // Fields
        private readonly LoggerFactory _loggerFactory = null;

        private readonly string _controllerFileName = null;


        // Constructors
        public MvcServerMiddlewareLauncher(LoggerFactory loggerFactory, string controllerFileName = @"*.Services.dll")
        {
            #region Contracts

            if (loggerFactory == null) throw new ArgumentException();
            if (string.IsNullOrEmpty(controllerFileName) == true) throw new ArgumentException();

            #endregion

            // Default
            _loggerFactory = loggerFactory;
            _controllerFileName = controllerFileName;
        }


        // Methods
        public void ConfigureMiddleware(IApplicationBuilder app)
        {
            #region Contracts

            if (app == null) throw new ArgumentException();

            #endregion

            // Mvc   
            app.UseMvc();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            #region Contracts

            if (services == null) throw new ArgumentException();

            #endregion

            // MvcCore
            var builder = services.AddMvcCore(options =>
            {
                // Conventions
                options.Conventions.Clear();
                options.Conventions.Add(new NamespaceRouteConvention());

                // Filters
                options.Filters.Add(new ActionLogFilter(_loggerFactory));
            })
            .AddJsonFormatters();

            // AssemblyControllers
            this.RegisterAssemblyControllers(builder);
        }


        private void RegisterAssemblyControllers(IMvcCoreBuilder builder)
        {
            #region Contracts

            if (builder == null) throw new ArgumentException();

            #endregion

            // AssemblyFileList
            var assemblyFileList = FileHelper.GetAllFile(_controllerFileName);
            if (assemblyFileList == null) throw new InvalidOperationException();

            // AddAssemblyController
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
                builder.AddApplicationPart(assembly);
            }
        }
    }
}
