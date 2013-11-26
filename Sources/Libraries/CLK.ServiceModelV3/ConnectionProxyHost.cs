using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CLK.ServiceModel
{    
    public abstract class ConnectionProxyHost<TConnectionProxy> : ConnectionHost<TConnectionProxy>
        where TConnectionProxy : ConnectionProxy
    {
        // Fields
        private readonly object _syncObject = new object();

        private readonly IEnumerable<TConnectionProxy> _connectionProxyCollection = null;

        private readonly Func<IEnumerable<ConnectionProxy>, bool> _connectedPredicate = null;

        private bool _isConnected = false;


        // Constructors        
        public ConnectionProxyHost(IEnumerable<TConnectionProxy> connectionProxyCollection, Func<IEnumerable<ConnectionProxy>, bool> connectedPredicate)
        {
            #region Contracts

            if (connectionProxyCollection == null) throw new ArgumentNullException();
            if (connectedPredicate == null) throw new ArgumentNullException();

            #endregion
                        
            // ConnectionProxyCollection
            _connectionProxyCollection = connectionProxyCollection;

            // ConnectedPredicate 
            _connectedPredicate = connectedPredicate;

            // Attach
            foreach (TConnectionProxy connectionProxy in _connectionProxyCollection)
            {
                this.Attach(connectionProxy);
            }
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
            // Open
            foreach (TConnectionProxy connectionProxy in _connectionProxyCollection)
            {                
                connectionProxy.Connected += this.ConnectionProxy_Connected;
                connectionProxy.Disconnected += this.ConnectionProxy_Disconnected;
                connectionProxy.Open();
            }
        }

        public virtual void Close()
        {
            // Close
            foreach (TConnectionProxy connectionProxy in _connectionProxyCollection)
            {
                connectionProxy.Close();
                connectionProxy.Connected -= this.ConnectionProxy_Connected;
                connectionProxy.Disconnected -= this.ConnectionProxy_Disconnected;
            }
        }

        private void Refresh()
        {
            // IsConnected
            bool isConnected = _connectedPredicate(_connectionProxyCollection);

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
        private void ConnectionProxy_Connected(object sender, EventArgs e)
        {
            #region Contracts

            if (sender == null) throw new ArgumentException();
            if (e == null) throw new ArgumentException();

            #endregion

            // Refresh
            this.Refresh();
        }

        private void ConnectionProxy_Disconnected(object sender, EventArgs e)
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

    public static class ConnectionProxyHost
    {
        // ConnectedPredicate
        public static Func<IEnumerable<ConnectionProxy>, bool> OneConnectedPredicate
        {
            get
            {
                return delegate(IEnumerable<ConnectionProxy> connectionProxyCollection)
                {
                    foreach (ConnectionProxy connectionProxy in connectionProxyCollection)
                    {
                        if (connectionProxy.IsConnected == true)
                        {
                            return true;
                        }
                    }
                    return false;
                };
            }
        }

        public static Func<IEnumerable<ConnectionProxy>, bool> AllConnectedPredicate
        {
            get
            {
                return delegate(IEnumerable<ConnectionProxy> connectionProxyCollection)
                {
                    foreach (ConnectionProxy connectionProxy in connectionProxyCollection)
                    {
                        if (connectionProxy.IsConnected == false)
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