using Autofac;
using Autofac.Configuration;
using Autofac.Extensions.DependencyInjection;
using CLK.Autofac;
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
    public class PlatformContext : IDisposable
    {
        // Fields
        private readonly AutofacContext _autofacContext = null;

        private List<PlatformHoster> _platformHosterList = null;


        // Constructors
        public PlatformContext(string moduleFileName = @"*.Hosting.dll", string configFileName = @"*.Hosting.json")
        {
            // AutofacContext
            _autofacContext = new AutofacContext();
            {
                // RegisterModule
                if (string.IsNullOrEmpty(moduleFileName) == false)
                {
                    _autofacContext.RegisterAssemblyModules(typeof(PlatformModule), moduleFileName);
                }
                _autofacContext.RegisterAssemblyModules(typeof(PlatformModule), Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location));

                // RegisterConfig
                if (string.IsNullOrEmpty(configFileName) == false)
                {
                    _autofacContext.RegisterConfig(configFileName);
                }
                _autofacContext.RegisterConfig(Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location) + Path.GetExtension(configFileName));
            }
            _autofacContext.RegisterInstance(typeof(AutofacContext), _autofacContext);
        }

        public void Start()
        {
            // AutofacContext
            _autofacContext.Start();

            // PlatformHosterList
            if (_autofacContext.IsRegistered<List<PlatformHoster>>() == true)
            {
                // Create
                _platformHosterList = _autofacContext.Resolve<List<PlatformHoster>>();
                if (_platformHosterList == null) throw new InvalidOperationException("_platformHosterList=null");

                // Start
                foreach (var platformHoster in _platformHosterList)
                {
                    platformHoster.Start();
                }
            }
        }

        public void Dispose()
        {
            // PlatformHosterList
            if (_platformHosterList != null)
            {
                // Dispose
                foreach (var platformHoster in _platformHosterList.Reverse<PlatformHoster>())
                {
                    platformHoster.Dispose();
                }
            }

            // AutofacContext
            _autofacContext?.Dispose();
        }


        // Methods
        public TService Resolve<TService>() where TService : class
        {
            // Return
            return _autofacContext.Resolve<TService>();
        }
    }
}
