using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceModel
{
    public abstract class ConnectionProxyCallbackAgent<TCallback>
        where TCallback : class
    {
        // Properties  
        protected TCallback Callback { get; set; }


        // Methods  
        internal void Initialize(TCallback callback)
        {
            #region Contracts

            if (callback == null) throw new ArgumentNullException();

            #endregion

            // Callback
            this.Callback = callback;
        }
    }
}
