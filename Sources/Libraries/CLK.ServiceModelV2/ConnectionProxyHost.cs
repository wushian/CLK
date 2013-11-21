using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CLK.ServiceModel
{    
    public class ConnectionProxyHost<TService> : ConnectionProxyHostBase<ConnectionProxy<TService>, Connection<TService>>
        where TService : class, IConnectionService
    {
        // Constructors        
        public ConnectionProxyHost(IEnumerable<ConnectionProxy<TService>> connectionProxyCollection, Func<IEnumerable<ConnectionProxy>, bool> connectedPredicate) : base(connectionProxyCollection, connectedPredicate) { }
        

        // Methods
        internal override Connection<TService> CreateConnection(ConnectionProxy<TService> connectionProxy)
        {
            #region Contracts

            if (connectionProxy == null) throw new ArgumentNullException();

            #endregion

            // Return
            return new ConnectionAdapter(connectionProxy);
        }


        // Class
        private sealed class ConnectionAdapter : Connection<TService>
        {
            // Fields
            private readonly ConnectionProxy<TService> _connectionProxy = null;


            // Constructors        
            public ConnectionAdapter(ConnectionProxy<TService> connectionProxy)
            {
                #region Contracts

                if (connectionProxy == null) throw new ArgumentNullException();

                #endregion

                // Arguments
                _connectionProxy = connectionProxy;
            }


            // Properties
            public override TService Service { get { return _connectionProxy.Service; } }
        }
    }

    public class ConnectionProxyHost<TService, TCallback> : ConnectionProxyHostBase<ConnectionProxy<TService, TCallback>, Connection<TService, TCallback>>
        where TService : class, IConnectionService
        where TCallback : class
    {
        // Constructors        
        public ConnectionProxyHost(IEnumerable<ConnectionProxy<TService, TCallback>> connectionProxyCollection, Func<IEnumerable<ConnectionProxy>, bool> connectedPredicate) : base(connectionProxyCollection, connectedPredicate) { }


        // Methods
        internal override Connection<TService, TCallback> CreateConnection(ConnectionProxy<TService, TCallback> connectionProxy)
        {
            #region Contracts

            if (connectionProxy == null) throw new ArgumentNullException();

            #endregion

            // Return
            return new ConnectionAdapter(connectionProxy);
        }


        // Class
        private sealed class ConnectionAdapter : Connection<TService, TCallback>
        {
            // Fields
            private readonly ConnectionProxy<TService, TCallback> _connectionProxy = null;


            // Constructors        
            public ConnectionAdapter(ConnectionProxy<TService, TCallback> connectionProxy)
            {
                #region Contracts

                if (connectionProxy == null) throw new ArgumentNullException();

                #endregion

                // Arguments
                _connectionProxy = connectionProxy;
            }


            // Properties
            public override TService Service { get { return _connectionProxy.Service; } }

            public override TCallback Callback { get { return _connectionProxy.Callback; } }
        }
    }

    public static class ConnectionProxyHost
    {
        // ConnectedPredicate
        public static Func<IEnumerable<ConnectionProxy>, bool> OneConnectedPredicate
        {
            get
            {
                return delegate(IEnumerable<ConnectionProxy> connectionProxyCollection)
                {
                    foreach (ConnectionProxy connectionProxy in connectionProxyCollection)
                    {
                        if (connectionProxy.IsConnected == true)
                        {
                            return true;
                        }
                    }
                    return false;
                };
            }
        }

        public static Func<IEnumerable<ConnectionProxy>, bool> AllConnectedPredicate
        {
            get
            {
                return delegate(IEnumerable<ConnectionProxy> connectionProxyCollection)
                {
                    foreach (ConnectionProxy connectionProxy in connectionProxyCollection)
                    {
                        if (connectionProxy.IsConnected == false)
                        {
                            return false;
                        }
                    }
                    return true;
                };
            }
        }
    }
}