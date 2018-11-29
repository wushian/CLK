using Autofac;
using CLK.Autofac;
using CLK.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Logging.Hosting
{
    public class LoggerContextModule : PlatformModule
    {
        // Methods
        protected override void Load(ContainerBuilder autofacBuilder)
        {
            #region Contracts

            if (autofacBuilder == null) throw new ArgumentException();

            #endregion
            
            // LoggerContext
            autofacBuilder.Register(autofacContext =>
            {
                return new LoggerContext
                (
                    autofacContext.Resolve<IEnumerable<LoggerProvider>>()
                )
                {

                };
            })
            .As<LoggerContext>()
            .AutoActivate()
            .SingleInstance()
            .OnActivated(handler => { handler.Instance.Start();});

            // LoggerFactory
            autofacBuilder.Register(autofacContext =>
            {
                return autofacContext.Resolve<LoggerContext>()?.LoggerFactory;
            })
            .As<LoggerFactory>()
            .ExternallyOwned();

            // Logger
            autofacBuilder.RegisterGeneric
            (
                typeof(ResolveLogger<>)
            )
            .As(typeof(Logger<>))
            .ExternallyOwned();
        }
    }
}
