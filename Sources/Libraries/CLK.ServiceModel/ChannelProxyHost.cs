using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CLK.ServiceModel
{    
    public abstract class ChannelProxyHost<TChannelProxy> : ChannelHost<TChannelProxy>
        where TChannelProxy : ChannelProxy
    {
        // Fields
        private readonly object _syncObject = new object();

        private readonly IEnumerable<TChannelProxy> _channelProxyCollection = null;

        private readonly Func<IEnumerable<ChannelProxy>, bool> _connectedPredicate = null;

        private bool _isConnected = false;


        // Constructors        
        public ChannelProxyHost(IEnumerable<TChannelProxy> channelProxyCollection, Func<IEnumerable<ChannelProxy>, bool> connectedPredicate)
        {
            #region Contracts

            if (channelProxyCollection == null) throw new ArgumentNullException();
            if (connectedPredicate == null) throw new ArgumentNullException();

            #endregion
                        
            // ChannelProxyCollection
            _channelProxyCollection = channelProxyCollection;

            // ConnectedPredicate 
            _connectedPredicate = connectedPredicate;            
        }


        // Properties
        public bool IsConnected
        {
            get
            {
                lock (_syncObject)
                {
                    return _isConnected;
                }
            }
        }


        // Methods
        public virtual void Open()
        {
            // Attach
            foreach (TChannelProxy channelProxy in _channelProxyCollection)
            {
                this.Attach(channelProxy);
            }

            // Open
            foreach (TChannelProxy channelProxy in _channelProxyCollection)
            {                
                channelProxy.Connected += this.ChannelProxy_Connected;
                channelProxy.Disconnected += this.ChannelProxy_Disconnected;
                channelProxy.Open();
            }
        }

        public virtual void Close()
        {
            // Close
            foreach (TChannelProxy channelProxy in _channelProxyCollection)
            {
                channelProxy.Close();
                channelProxy.Connected -= this.ChannelProxy_Connected;
                channelProxy.Disconnected -= this.ChannelProxy_Disconnected;
            }

            // Detach
            foreach (TChannelProxy channelProxy in _channelProxyCollection)
            {
                this.Detach(channelProxy);
            }
        }

        private void Refresh()
        {
            // IsConnected
            bool isConnected = _connectedPredicate(_channelProxyCollection);

            // Require
            lock (_syncObject)
            {
                if (_isConnected == isConnected) return;
                _isConnected = isConnected;
            }

            // Notify
            if (isConnected == true)
            {
                this.OnConnected();
            }
            else
            {
                this.OnDisconnected();
            }
        }


        // Handlers
        private void ChannelProxy_Connected(object sender, EventArgs e)
        {
            #region Contracts

            if (sender == null) throw new ArgumentException();
            if (e == null) throw new ArgumentException();

            #endregion

            // Refresh
            this.Refresh();
        }

        private void ChannelProxy_Disconnected(object sender, EventArgs e)
        {
            #region Contracts

            if (sender == null) throw new ArgumentException();
            if (e == null) throw new ArgumentException();

            #endregion

            // Refresh
            this.Refresh();
        }


        // Events
        public event EventHandler Connected;
        private void OnConnected()
        {
            var handler = this.Connected;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public event EventHandler Disconnected;
        private void OnDisconnected()
        {
            var handler = this.Disconnected;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }

    public static class ChannelProxyHost
    {
        // ConnectedPredicate
        public static Func<IEnumerable<ChannelProxy>, bool> OneConnectedPredicate
        {
            get
            {
                return delegate(IEnumerable<ChannelProxy> channelProxyCollection)
                {
                    foreach (ChannelProxy channelProxy in channelProxyCollection)
                    {
                        if (channelProxy.IsConnected == true)
                        {
                            return true;
                        }
                    }
                    return false;
                };
            }
        }

        public static Func<IEnumerable<ChannelProxy>, bool> AllConnectedPredicate
        {
            get
            {
                return delegate(IEnumerable<ChannelProxy> channelProxyCollection)
                {
                    foreach (ChannelProxy channelProxy in channelProxyCollection)
                    {
                        if (channelProxy.IsConnected == false)
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