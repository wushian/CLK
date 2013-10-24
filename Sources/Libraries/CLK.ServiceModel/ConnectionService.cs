using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CLK.ServiceModel
{
    [ServiceBehavior(UseSynchronizationContext = false, IncludeExceptionDetailInFaults = true)]
    public abstract class ConnectionService : IConnectionService
    {
        // Fields
        private readonly object _syncRoot = new object();

        private readonly ServiceHostBase _serviceHost = null;

        private readonly IContextChannel _channel = null;

        private bool _isConnected = false;


        // Constructors
        public ConnectionService()
        {
            // Require
            if (OperationContext.Current == null) throw new InvalidOperationException();
            if (OperationContext.Current.Host == null) throw new InvalidOperationException();
            if (OperationContext.Current.Channel == null) throw new InvalidOperationException();            

            // IsConnected
            _isConnected = true;

            // ServiceHost
            _serviceHost = OperationContext.Current.Host;

            // Channel
            _channel = OperationContext.Current.Channel;
            _channel.Closed += this.Channel_Closed;   
        }


        // Propertie
        public bool IsConnected
        {
            get
            {
                lock (_syncRoot)
                {
                    // Return
                    return _isConnected;
                }
            }
        }      


        // Methods               
        protected ConnectionServiceProvider GetServiceProvider()
        {
            // Locator
            ConnectionServiceProvider serviceProvider = this.GetServiceProvider<ConnectionServiceProvider>();
            if (serviceProvider == null) serviceProvider = new ConnectionServiceProvider();

            // Return
            return serviceProvider;
        }

        protected TServiceProvider GetServiceProvider<TServiceProvider>()
           where TServiceProvider : ConnectionServiceProvider
        {
            // Locator
            return ConnectionServiceProviderLocator.Current.GetServiceProvider<TServiceProvider>(_serviceHost);
        }
        

        void IConnectionService.Heartbeat()
        {
            // Notify
            this.OnHeartbeating(this, EventArgs.Empty);
        }
    

        // Handlers
        private void Channel_Closed(object sender, EventArgs e)
        {
            #region Contracts

            if (sender == null) throw new ArgumentNullException();
            if (e == null) throw new ArgumentNullException();

            #endregion

            // _isConnected
            lock (_syncRoot)
            {
                if (_isConnected == false) return;
                _isConnected = false;
            }

            // Notify
            this.OnDisconnected(sender, e);
        }


        // Events
        public event EventHandler Disconnected;
        private void OnDisconnected(object sender, EventArgs e)
        {
            #region Contracts

            if (sender == null) throw new ArgumentNullException();
            if (e == null) throw new ArgumentNullException();

            #endregion

            var handler = this.Disconnected;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler Heartbeating;
        private void OnHeartbeating(object sender, EventArgs e)
        {
            #region Contracts

            if (sender == null) throw new ArgumentNullException();
            if (e == null) throw new ArgumentNullException();

            #endregion

            var handler = this.Heartbeating;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
