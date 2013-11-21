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
    public abstract class ConnectionService<TService> : ConnectionServiceBase<Connection<TService>>
        where TService : class, IConnectionService
    {
        // Constructors
        public ConnectionService() : base() { }


        // Propertie
        public abstract TService Service
        {
            get;
        }


        // Methods   
        internal override Connection<TService> CreateConnection()
        {
            // Service
            TService service = this.Service;
            if (service == null) throw new InvalidOperationException();

            // Return
            return new ConnectionAdapter(service);
        }


        // Class
        private sealed class ConnectionAdapter : Connection<TService>
        {
            // Fields
            private readonly TService _service = null;
            

            // Constructors        
            public ConnectionAdapter(TService service)
            {
                #region Contracts

                if (service == null) throw new ArgumentNullException();

                #endregion

                // Arguments
                _service = service;
            }


            // Properties
            public override TService Service { get { return _service; } }
        }
    }

    [ServiceBehavior(UseSynchronizationContext = false, IncludeExceptionDetailInFaults = true)]
    public abstract class ConnectionService<TService, TCallback> : ConnectionServiceBase<Connection<TService, TCallback>>
        where TService : class, IConnectionService
        where TCallback : class
    {
        // Fields
        private TCallback _callback = null;


        // Constructors
        public ConnectionService() : base() { }


        // Propertie
        public abstract TService Service 
        { 
            get; 
        }

        public TCallback Callback
        {
            get
            {                
                if (_callback == null)
                {
                    _callback = OperationContext.Current.GetCallbackChannel<TCallback>();
                }
                return _callback;
            }
        }


        // Methods   
        internal override Connection<TService, TCallback> CreateConnection()
        {
            // Service
            TService service = this.Service;
            if (service == null) throw new InvalidOperationException();

            // Callback
            TCallback callback = this.Callback;
            if (callback == null) throw new InvalidOperationException();

            // Return
            return new ConnectionAdapter(service, callback);
        }


        // Class
        private sealed class ConnectionAdapter : Connection<TService, TCallback>
        {
            // Fields
            private readonly TService _service = null;

            private readonly TCallback _callback = null;


            // Constructors        
            public ConnectionAdapter(TService service, TCallback callback)
            {
                #region Contracts

                if (service == null) throw new ArgumentNullException();
                if (callback == null) throw new ArgumentNullException();

                #endregion

                // Arguments
                _service = service;
                _callback = callback;
            }


            // Properties
            public override TService Service { get { return _service; } }

            public override TCallback Callback { get { return _callback; } }
        }
    }
}
