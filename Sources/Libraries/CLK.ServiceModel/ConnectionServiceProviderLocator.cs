using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceModel
{
    internal sealed partial class ConnectionServiceProviderLocator
    {
        // Singleton
        private static ConnectionServiceProviderLocator _current = null;

        public static ConnectionServiceProviderLocator Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new ConnectionServiceProviderLocator();
                }
                return _current;
            }
        }
    }

    internal sealed partial class ConnectionServiceProviderLocator
    {
        // Fields  
        private readonly object _syncRoot = new object();

        private readonly Dictionary<ServiceHostBase, List<ConnectionServiceProvider>> _serviceProviderDictionary = new Dictionary<ServiceHostBase,List<ConnectionServiceProvider>>();


        // Methods
        public void AttachServiceProvider(ServiceHostBase serviceHost, ConnectionServiceProvider serviceProvider)
        {
            #region Contracts

            if (serviceHost == null) throw new ArgumentNullException();
            if (serviceProvider == null) throw new ArgumentNullException();

            #endregion

            lock (_syncRoot)
            {
                // ServiceHostBase
                if (_serviceProviderDictionary.ContainsKey(serviceHost) == false)
                {
                    _serviceProviderDictionary.Add(serviceHost, new List<ConnectionServiceProvider>());
                }

                // ServiceProvider
                List<ConnectionServiceProvider> serviceProviderList = _serviceProviderDictionary[serviceHost];
                if (serviceProviderList.Contains(serviceProvider) == false)
                {
                    serviceProviderList.Add(serviceProvider);
                }
            }
        }

        public void DetachServiceProvider(ServiceHostBase serviceHost, ConnectionServiceProvider serviceProvider)
        {
            #region Contracts

            if (serviceHost == null) throw new ArgumentNullException();
            if (serviceProvider == null) throw new ArgumentNullException();

            #endregion

            lock (_syncRoot)
            {
                // ServiceHostBase
                if (_serviceProviderDictionary.ContainsKey(serviceHost) == false)
                {
                    return;
                }

                // ServiceProvider
                List<ConnectionServiceProvider> serviceProviderList = _serviceProviderDictionary[serviceHost];
                if (serviceProviderList.Contains(serviceProvider) == false)
                {
                    serviceProviderList.Remove(serviceProvider);
                }

                if (serviceProviderList.Count == 0)
                {
                    _serviceProviderDictionary.Remove(serviceHost);
                }
            }
        }

        public TServiceProvider GetServiceProvider<TServiceProvider>(ServiceHostBase serviceHost)
            where TServiceProvider : ConnectionServiceProvider
        {
            #region Contracts

            if (serviceHost == null) throw new ArgumentNullException();

            #endregion

            lock (_syncRoot)
            {
                // Result
                TServiceProvider serviceProvider = null;

                // Search 
                if (_serviceProviderDictionary.ContainsKey(serviceHost) == true)
                {
                    foreach (ConnectionServiceProvider connectionServiceProvider in _serviceProviderDictionary[serviceHost])
                    {
                        serviceProvider = connectionServiceProvider as TServiceProvider;
                        if (serviceProvider != null) break;
                    }
                }

                // Return
                return serviceProvider;
            }
        }
    }
}
