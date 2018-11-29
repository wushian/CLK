using Autofac;
using CLK.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Transactions.Hosting
{
    public class TransactionContextModule : PlatformModule
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
                    autofacContext.Resolve<IEnumerable<TransactionProvider>>()
                )
                {

                };
            })
            .As<TransactionContext>()
            .AutoActivate()
            .SingleInstance()
            .OnActivated(handler => { handler.Instance.Start(); });

            // TransactionFactory
            autofacBuilder.Register(autofacContext =>
            {
                return autofacContext.Resolve<TransactionContext>()?.TransactionFactory;
            })
            .As<TransactionFactory>()
            .ExternallyOwned();

            // Transaction
            autofacBuilder.Register(autofacContext =>
            {
                return autofacContext.Resolve<TransactionContext>()?.BeginTransaction();
            })
            .As<Transaction>()
            .ExternallyOwned();
        }
    }
}
