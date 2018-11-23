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
        private readonly AutofacContext _platformAutofacContext = null;
        
        private readonly AutofacContext _serviceAutofacContext = null;

        private List<PlatformHoster> _platformHosterList = null;


        // Constructors
        public PlatformContext(string moduleFileName = "*.Hosting.dll", string configFilename = "*.Hosting.json")
        {
            // PlatformAutofacContext
            _platformAutofacContext = new AutofacContext();
            {
                // RegisterModule
                if (string.IsNullOrEmpty(moduleFileName) == false)
                {
                    _platformAutofacContext.RegisterAssemblyModules(typeof(PlatformModule), moduleFileName);
                }
                _platformAutofacContext.RegisterAssemblyModules(typeof(PlatformModule), Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location));
            }

            // ServiceAutofacContext
            _serviceAutofacContext = new AutofacContext();
            {
                // RegisterModule
                if (string.IsNullOrEmpty(moduleFileName) == false)
                {
                    _serviceAutofacContext.RegisterAssemblyModules(typeof(ServiceModule), moduleFileName);
                }
                _serviceAutofacContext.RegisterAssemblyModules(typeof(ServiceModule), Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location));

                // RegisterConfig
                if (string.IsNullOrEmpty(configFilename) == false)
                {
                    _serviceAutofacContext.RegisterConfig(configFilename);
                }
                _serviceAutofacContext.RegisterConfig(Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location) + Path.GetExtension(configFilename));
            }

            // Attach
            _platformAutofacContext.RegisterInstance(typeof(AutofacContext), _serviceAutofacContext);
        }

        public void Start()
        {
            // AutofacContext
            _platformAutofacContext.Start();
            _serviceAutofacContext.Start();

            // PlatformHosterList
            _platformHosterList = _platformAutofacContext.Resolve<List<PlatformHoster>>();
            if (_platformHosterList != null)
            {
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
                foreach (var platformHoster in _platformHosterList.Reverse<PlatformHoster>())
                {
                    platformHoster.Dispose();
                }
            }

            // AutofacContext
            _serviceAutofacContext?.Dispose();
            _platformAutofacContext?.Dispose();
        }


        // Methods
        public TService Resolve<TService>() where TService : class
        {
            // Return
            return _serviceAutofacContext.Resolve<TService>();
        }
    }
}
