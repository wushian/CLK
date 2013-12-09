using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceModel
{
    public abstract class ChannelServiceHost<TChannelService> : ChannelHost<TChannelService>
        where TChannelService : ChannelService, new()
    {
        // Fields   
        private readonly ServiceHost _serviceHost = null;

        private readonly ChannelServiceMediator _serviceMediator = null;
               

        // Constructors
        public ChannelServiceHost(Type contract, Binding binding, string address)
        {
            #region Contracts

            if (contract == null) throw new ArgumentNullException();
            if (binding == null) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(address) == true) throw new ArgumentNullException();

            #endregion

            // ServiceHost
            _serviceHost = new ServiceHost(typeof(TChannelService));
            _serviceHost.AddServiceEndpoint(contract, binding, address);

            // ServiceMediator
            _serviceMediator = new ChannelServiceMediator();
            _serviceMediator.Connected += this.ChannelService_Connected;
            _serviceMediator.Disconnected += this.ChannelService_Disconnected;
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
            ChannelServiceResource.Current.AttachResource(_serviceHost, resource);
        }

        protected void DetachResource(object resource)
        {
            #region Contracts

            if (resource == null) throw new ArgumentNullException();

            #endregion
                        
            // Resource
            ChannelServiceResource.Current.DetachResource(_serviceHost, resource);
        }


        // Handlers
        private void ChannelService_Connected(object sender, EventArgs e)
        {
            #region Contracts

            if (sender == null) throw new ArgumentException();
            if (e == null) throw new ArgumentException();

            #endregion

            // Require
            TChannelService channelService = sender as TChannelService;
            if (channelService == null) return;

            // Attach
            this.Attach(channelService);
        }

        private void ChannelService_Disconnected(object sender, EventArgs e)
        {
            #region Contracts

            if (sender == null) throw new ArgumentException();
            if (e == null) throw new ArgumentException();

            #endregion

            // Require
            TChannelService channelService = sender as TChannelService;
            if (channelService == null) return;

           

            // Detach
            this.Detach(channelService);
        }
    }
}
