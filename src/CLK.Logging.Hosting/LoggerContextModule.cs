using Autofac;
using CLK.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Logging.Hosting
{
    public class LoggerContextModule : ServiceModule
    {
        // Methods
        protected override void Load(ContainerBuilder autofacBuilder)
        {
            #region Contracts

            if (autofacBuilder == null) throw new ArgumentException();

            #endregion

            // Register
            autofacBuilder.RegisterGeneric(typeof(LoggerCollection<>)).As(typeof(Logger<>));
            
            // LoggerContext
            autofacBuilder.Register(autofacContext =>
            {                
                return new LoggerContext
                (
                    autofacContext.Resolve<IEnumerable<LoggerFactory>>()
                )
                {
                        
                };
            }).As<LoggerContext>()

            // Lifetime
            .OnActivated(handler =>
            {
                // Start
                handler.Instance.Start();
            })            
            .AutoActivate().SingleInstance();
        }
    }
}
