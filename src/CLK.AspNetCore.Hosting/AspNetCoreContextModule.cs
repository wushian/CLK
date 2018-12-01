using Autofac;
using CLK.Autofac;
using CLK.Logging;
using CLK.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.AspNetCore.Hosting
{
    public class AspNetCoreContextModule : PlatformModule
    {
        // Methods
        protected override void Load(ContainerBuilder autofacBuilder)
        {
            #region Contracts

            if (autofacBuilder == null) throw new ArgumentException();

            #endregion

            // AspNetCoreContext
            autofacBuilder.Register(autofacContext =>
            {
                // Create
                return new AspNetCoreContext
                (
                    this.ResolveWebHostOptions(autofacContext)
                )
                {

                };
            })
            .As<AspNetCoreContext>()
            .ExternallyOwned()
            .SingleInstance();

            // AspNetCoreContextHoster
            autofacBuilder.RegisterType
            (
                typeof(AspNetCoreContextHoster)
            )
            .As<PlatformHoster>()
            .ExternallyOwned();
        }

        private WebHostOptions ResolveWebHostOptions(IComponentContext autofacContext)
        {
            #region Contracts

            if (autofacContext == null) throw new ArgumentException();

            #endregion

            // WebHostOptions
            WebHostOptions options = null;
            if (autofacContext.IsRegistered<WebHostOptions>() == true)
            {
                options = autofacContext.Resolve<WebHostOptions>();
            }
            if (options == null) options = new WebHostOptions(autofacContext.Resolve<AutofacScope>());

            // DefaultMiddleware
            if (options.MiddlewareLauncherList.Count <= 0)
            {
                // Cors
                options.MiddlewareLauncherList.Add(new CorsMiddlewareLauncher());

                // UnhandledException
                options.MiddlewareLauncherList.Add(new UnhandledExceptionMiddlewareLauncher());

                // FileServer
                options.MiddlewareLauncherList.Add(new FileServerMiddlewareLauncher());

                // MvcServer
                options.MiddlewareLauncherList.Add(new MvcServerMiddlewareLauncher
                (
                    autofacContext.Resolve<LoggerFactory>()
                ));
            }

            // Return
            return options;
        }
    }
}
