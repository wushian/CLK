using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CLK.ServiceModel
{     
    public abstract class ConnectionServiceBase<TConnection> : IConnectionService
        where TConnection : class
    {
        // Fields
        private readonly object _syncRoot = new object();
                
        private readonly ServiceHostBase _serviceHost = null;

        private readonly IContextChannel _channel = null;     

        private readonly ConnectionHost<TConnection> _connectionHost = null;

        private readonly TConnection _connection = null;             
               
        private bool _isConnected = true;


        // Constructors
        internal ConnectionServiceBase()
        {
            // Require
            if (OperationContext.Current == null) throw new InvalidOperationException();
            if (OperationContext.Current.Host == null) throw new InvalidOperationException();
            if (OperationContext.Current.Channel == null) throw new InvalidOperationException();
                        
            // ServiceHost
            _serviceHost = OperationContext.Current.Host;
                        
            // ConnectionHost
            _connectionHost = this.GetResource<ConnectionHost<TConnection>>();
            if (_connectionHost == null) throw new InvalidOperationException();

            // Connection
            _connection = this.CreateConnection();
            if (_connection == null) throw new InvalidOperationException();
            _connectionHost.Attach(_connection);

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
        internal abstract TConnection CreateConnection();
        
        protected TResource GetResource<TResource>() where TResource : class
        {
            // Resource
            return ConnectionServiceResource.Current.GetResource<TResource>(_serviceHost);
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

            // Connection
            _connectionHost.Detach(_connection);
        }


        // Events
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
