using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceModel
{
    public abstract class ConnectionProxyMediator<TService, TCallback>
        where TService : class, IConnectionService
        where TCallback : class
    {
        // Properties  
        protected TCallback Callback { get; private set; }


        // Methods  
        internal protected virtual void Attach(TCallback callback)
        {
            #region Contracts

            if (callback == null) throw new ArgumentNullException();

            #endregion

            // Callback
            this.Callback = callback;
        }

        internal protected virtual void Detach(TCallback callback)
        {
            #region Contracts

            if (callback == null) throw new ArgumentNullException();

            #endregion

            // CallbackDetach
            this.Callback = null;
        }
    }
}
