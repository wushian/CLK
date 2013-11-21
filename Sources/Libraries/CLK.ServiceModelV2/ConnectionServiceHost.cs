using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceModel
{
    public class ConnectionServiceHost<TService> : ConnectionServiceHostBase<Connection<TService>>
        where TService : class, IConnectionService
    {
        // Constructors
        public ConnectionServiceHost(ServiceHost serviceHost) : base(serviceHost) { }


        // Properties
        internal override ConnectionHost<Connection<TService>> ConnectionHost
        {
            get
            {
                return this;
            }
        }
    }

    public class ConnectionServiceHost<TService, TCallback> : ConnectionServiceHostBase<Connection<TService, TCallback>>
        where TService : class, IConnectionService
        where TCallback : class
    {
        // Constructors
        public ConnectionServiceHost(ServiceHost serviceHost) : base(serviceHost) { }


        // Properties
        internal override ConnectionHost<Connection<TService, TCallback>> ConnectionHost
        {
            get
            {
                return this;
            }
        }
    }
}
