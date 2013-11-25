using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CLK.ServiceModel
{     
    public abstract class ConnectionService : Connection, IDisposable
    {
        // Fields
        private readonly object _syncRoot = new object();        

        private readonly ServiceHostBase _serviceHost = null;

        private readonly IContextChannel _channel = null;

        private readonly ConnectionServiceMediator _serviceMediator = null;

        private bool _isConnected = true;


        // Constructors
        internal ConnectionService()
        {
            // Require
            if (OperationContext.Current == null) throw new InvalidOperationException();
            if (OperationContext.Current.Host == null) throw new InvalidOperationException();
            if (OperationContext.Current.Channel == null) throw new InvalidOperationException();   

            // ServiceHost
            _serviceHost = OperationContext.Current.Host;

            // Channel
            _channel = OperationContext.Current.Channel;

            // ServiceMediator
            _serviceMediator = this.GetResource<ConnectionServiceMediator>();
            if (_serviceMediator == null) throw new InvalidOperationException();

            // Notify
            this.OnConnected();
        }

        public void Dispose()
        {
            // IsConnected
            lock (_syncRoot)
            {
                if (_isConnected == false) return;
                _isConnected = false;
            }
                        
            // Notify
            this.OnDisconnected();

            // Channel
            _channel.Abort();
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
        protected TResource GetResource<TResource>() where TResource : class
        {
            // Resource
            return ConnectionServiceResource.Current.GetResource<TResource>(_serviceHost);
        }


        // Events
        public event EventHandler Connected;
        private void OnConnected()
        {
            // Notify
            _serviceMediator.OnConnected(this);

            // Handler
            var handler = this.Connected;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public event EventHandler Disconnected;
        private void OnDisconnected()
        {
            // Notify
            _serviceMediator.OnDisconnected(this);

            // Handler
            var handler = this.Disconnected;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }

    [ServiceBehavior(UseSynchronizationContext = false, IncludeExceptionDetailInFaults = true)]
    public abstract class ConnectionService<TService> : ConnectionService, IConnectionService
        where TService : class, IConnectionService
    {
        // Constructors
        public ConnectionService() : base() { }


        // Methods  
        void IConnectionService.Heartbeat()
        {
            // Nothing

        }
    }

    [ServiceBehavior(UseSynchronizationContext = false, IncludeExceptionDetailInFaults = true)]
    public abstract class ConnectionService<TService, TCallback> : ConnectionService<TService>
        where TService : class, IConnectionService
        where TCallback : class
    {
        // Constructors
        public ConnectionService()
            : base()
        {
            // Callback
            this.Callback = OperationContext.Current.GetCallbackChannel<TCallback>();
            if (this.Callback == null) throw new InvalidOperationException();
        }


        // Propertie
        public TCallback Callback { get; private set; }
    }
}
