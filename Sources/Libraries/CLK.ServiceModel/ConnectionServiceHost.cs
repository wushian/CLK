using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceModel
{
    public abstract class ConnectionServiceHost
    {
        // Fields   
        private readonly ServiceHost _serviceHost = null;
               

        // Constructors
        public ConnectionServiceHost(ServiceHost serviceHost)
        {
            #region Contracts

            if (serviceHost == null) throw new ArgumentNullException();

            #endregion

            // ServiceHost
            _serviceHost = serviceHost;
        }
        

        // Methods
        public virtual void Open()
        {                      
            // ServiceHost
            _serviceHost.Open();
        }

        public virtual void Close()
        {
            // ServiceHost
            _serviceHost.Abort();
        }


        protected void AttachServiceProvider(ConnectionServiceProvider serviceProvider)
        {
            #region Contracts

            if (serviceProvider == null) throw new ArgumentNullException();

            #endregion

            // Locator
            ConnectionServiceProviderLocator.Current.AttachServiceProvider(_serviceHost, serviceProvider);
        }

        protected void DetachServiceProvider(ConnectionServiceProvider serviceProvider)
        {
            #region Contracts

            if (serviceProvider == null) throw new ArgumentNullException();

            #endregion

            // Locator
            ConnectionServiceProviderLocator.Current.DetachServiceProvider(_serviceHost, serviceProvider);
        }
    }
}
