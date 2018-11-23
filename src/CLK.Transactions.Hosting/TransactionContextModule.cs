using Autofac;
using CLK.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Transactions.Hosting
{
    public class TransactionContextModule : ServiceModule
    {
        // Methods
        protected override void Load(ContainerBuilder autofacBuilder)
        {
            #region Contracts

            if (autofacBuilder == null) throw new ArgumentException();

            #endregion

            // TransactionContext
            autofacBuilder.Register(autofacContext =>
            {                
                return new TransactionContext
                (
                    autofacContext.Resolve<TransactionScopeFactory>()
                )
                {
                        
                };
            }).As<TransactionContext>()

            // Lifetime
            .OnActivated(handler =>
            {
                // Start
                TransactionContext.Initialize(handler.Instance).Start();
            })            
            .AutoActivate().SingleInstance();
        }
    }
}
