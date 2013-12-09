using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CLK.ServiceModel
{     
    public abstract class ChannelService : Channel, IDisposable
    {
        // Fields
        private readonly object _syncRoot = new object();        

        private readonly ServiceHostBase _serviceHost = null;

        private readonly IContextChannel _contextChannel = null;

        private readonly ChannelServiceMediator _serviceMediator = null;

        private bool _isConnected = true;


        // Constructors
        internal ChannelService()
        {
            // Require
            if (OperationContext.Current == null) throw new InvalidOperationException();
            if (OperationContext.Current.Host == null) throw new InvalidOperationException();
            if (OperationContext.Current.Channel == null) throw new InvalidOperationException();   

            // ServiceHost
            _serviceHost = OperationContext.Current.Host;

            // ContextChannel
            _contextChannel = OperationContext.Current.Channel;

            // ServiceMediator
            _serviceMediator = this.GetResource<ChannelServiceMediator>();
            if (_serviceMediator == null) throw new InvalidOperationException();

            // Notify
            _serviceMediator.OnConnected(this);
        }

        public virtual void Dispose()
        {
            // IsConnected
            lock (_syncRoot)
            {
                if (_isConnected == false) return;
                _isConnected = false;
            }

            // Notify
            _serviceMediator.OnDisconnected(this);

            // ContextChannel
            _contextChannel.Abort();
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
            return ChannelServiceResource.Current.GetResource<TResource>(_serviceHost);
        }
    }

    public abstract class ChannelService<TService> : ChannelService, IChannelService
        where TService : class, IChannelService
    {
        // Constructors
        public ChannelService()
            : base()
        {
            // Require
            if (typeof(TService).IsAssignableFrom(this.GetType()) == false) throw new InvalidOperationException();
        }


        // Methods  
        void IChannelService.Heartbeat()
        {
            // Nothing

        }
    }

    public abstract class ChannelService<TService, TCallback> : ChannelService<TService>
        where TService : class, IChannelService
        where TCallback : class
    {
        // Constructors
        public ChannelService()
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
