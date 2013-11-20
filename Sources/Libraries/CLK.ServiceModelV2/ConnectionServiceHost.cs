using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceModel
{
    public abstract class ConnectionServiceHost<TService> : ConnectionServiceHostBase<Connection<TService>>
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

    public abstract class ConnectionServiceHost<TService, TCallback> : ConnectionServiceHostBase<Connection<TService, TCallback>>
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

    public abstract class ConnectionServiceHostBase<TConnection> : ConnectionHost<TConnection>
        where TConnection : class
    {
        // Fields   
        private readonly ServiceHost _serviceHost = null;
               

        // Constructors
        internal ConnectionServiceHostBase(ServiceHost serviceHost)
        {
            #region Contracts

            if (serviceHost == null) throw new ArgumentNullException();

            #endregion

            // ServiceHost
            _serviceHost = serviceHost;      
        }


        // Properties
        internal abstract ConnectionHost<TConnection> ConnectionHost { get; }
        

        // Methods
        public void Open()
        {
            // ConnectionHost
            ConnectionHost<TConnection> connectionHost = this.ConnectionHost;
            if (connectionHost == null) throw new InvalidOperationException();
            this.AttachResource(connectionHost);

            // ServiceHost
            _serviceHost.Open();
        }

        public void Close()
        {
            // ServiceHost
            _serviceHost.Abort();

            // ConnectionHost
            ConnectionHost<TConnection> connectionHost = this.ConnectionHost;
            if (connectionHost == null) throw new InvalidOperationException();
            this.DetachResource(connectionHost);
        }
        
        protected void AttachResource(object resource)
        {
            #region Contracts

            if (resource == null) throw new ArgumentNullException();

            #endregion

            // Resource
            ConnectionServiceResource.Current.AttachResource(_serviceHost, resource);
        }

        protected void DetachResource(object resource)
        {
            #region Contracts

            if (resource == null) throw new ArgumentNullException();

            #endregion

            // Resource
            ConnectionServiceResource.Current.DetachResource(_serviceHost, resource);
        }
    }
}
