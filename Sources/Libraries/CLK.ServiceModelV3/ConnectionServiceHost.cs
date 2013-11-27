using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceModel
{
    public abstract class ConnectionServiceHost<TConnectionService, TService> : ConnectionHost<TConnectionService>
        where TConnectionService : ConnectionService<TService>, TService
        where TService : class, IConnectionService
    {
        // Fields   
        private readonly ServiceHost _serviceHost = null;

        private readonly ConnectionServiceMediator _serviceMediator = null;


        // Constructors
        public ConnectionServiceHost(Binding binding, string address)
        {
            #region Contracts

            if (binding == null) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(address) == true) throw new ArgumentNullException();

            #endregion

            // ServiceHost
            _serviceHost = new ServiceHost(typeof(TConnectionService));
            _serviceHost.AddServiceEndpoint(typeof(TService), binding, address);

            // ServiceMediator
            _serviceMediator = new ConnectionServiceMediator();
            _serviceMediator.Connected += this.ConnectionService_Connected;
            _serviceMediator.Disconnected += this.ConnectionService_Disconnected;
        }


        // Methods
        public virtual void Open()
        {
            // AttachResource
            this.AttachResource(_serviceMediator);

            // ServiceHost
            _serviceHost.Open();
        }

        public virtual void Close()
        {
            // ServiceHost
            _serviceHost.Abort();

            // DetachResource
            this.DetachResource(_serviceMediator);
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


        // Handlers
        private void ConnectionService_Disconnected(object sender, EventArgs e)
        {
            #region Contracts

            if (sender == null) throw new ArgumentException();
            if (e == null) throw new ArgumentException();

            #endregion

            // Require
            TConnectionService connectionService = sender as TConnectionService;
            if (connectionService == null) return;

            // Attach
            this.Attach(connectionService);
        }

        private void ConnectionService_Connected(object sender, EventArgs e)
        {
            #region Contracts

            if (sender == null) throw new ArgumentException();
            if (e == null) throw new ArgumentException();

            #endregion

            // Require
            TConnectionService connectionService = sender as TConnectionService;
            if (connectionService == null) return;

            // Detach
            this.Detach(connectionService);
        }
    }

    public abstract class ConnectionServiceHost<TConnectionService, TService, TCallback> : ConnectionServiceHost<TConnectionService, TService>
        where TConnectionService : ConnectionService<TService, TCallback>, TService
        where TService : class, IConnectionService
        where TCallback : class
    {
        // Constructors
        public ConnectionServiceHost(Binding binding, string address) : base(binding, address) { }
    }
}
