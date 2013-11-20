using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CLK.ServiceModel
{
    public abstract class ConnectionProxyHost
    {
        // Properties
        public abstract bool IsConnected { get; }


        // Constructors
        internal ConnectionProxyHost() { }


        // Methods
        public abstract void Open();

        public abstract void Close();


        // Events
        public event EventHandler Connected;
        protected void OnConnected()
        {
            var handler = this.Connected;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public event EventHandler Disconnected;
        protected void OnDisconnected()
        {
            var handler = this.Disconnected;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public event EventHandler Heartbeating;
        protected void OnHeartbeating()
        {
            var handler = this.Heartbeating;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }


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
    
    public class ConnectionProxyHost<TService> : ConnectionProxyHost
        where TService : class, IConnectionService
    {
        // Fields
        private readonly object _syncObject = new object();

        private readonly IEnumerable<ConnectionProxy<TService>> _connectionProxyCollection = null;

        private readonly Func<IEnumerable<ConnectionProxy<TService>>, bool> _isConnectedPredicate = null;

        private bool _isConnected = false;


        // Constructors        
        public ConnectionProxyHost(IEnumerable<ConnectionProxy<TService>> connectionProxyCollection, Func<IEnumerable<ConnectionProxy>, bool> isConnectedPredicate)
        {
            #region Contracts

            if (connectionProxyCollection == null) throw new ArgumentNullException();
            if (isConnectedPredicate == null) throw new ArgumentNullException();

            #endregion

            // ConnectionProxyCollection
            _connectionProxyCollection = connectionProxyCollection;

            // IsConnectedPredicate 
            _isConnectedPredicate = isConnectedPredicate;
        }


        // Properties
        public override bool IsConnected
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
        public override void Open()
        {
            // ConnectionProxyCollection
            foreach (ConnectionProxy<TService> connectionProxy in _connectionProxyCollection)
            {
                connectionProxy.Connected += this.Proxy_Connected;
                connectionProxy.Disconnected += this.Proxy_Disconnected;
                connectionProxy.Heartbeating += this.Proxy_Heartbeating;
                connectionProxy.Open();
            }
        }

        public override void Close()
        {
            // ConnectionProxyCollection
            foreach (ConnectionProxy<TService> connectionProxy in _connectionProxyCollection)
            {
                connectionProxy.Close();
                connectionProxy.Connected -= this.Proxy_Connected;
                connectionProxy.Disconnected -= this.Proxy_Disconnected;
                connectionProxy.Heartbeating -= this.Proxy_Heartbeating;
            }
        }

        private void Refresh()
        {
            // IsConnected
            bool isConnected = _isConnectedPredicate(_connectionProxyCollection);

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


        public void Execute(Action<ConnectionProxy<TService>> executeDelegate)
        {
            #region Contracts

            if (executeDelegate == null) throw new ArgumentNullException();

            #endregion

            // ConnectionProxyCollection
            foreach (ConnectionProxy<TService> connectionProxy in _connectionProxyCollection)
            {
                try
                {
                    // Execute
                    executeDelegate(connectionProxy);

                    // Return
                    return;
                }
                catch
                {
                    // Nothing

                }
            }

            // Throw
            throw new ExecuteIgnoredException();
        }

        public TResult Execute<TResult>(Func<ConnectionProxy<TService>, TResult> executeDelegate)
        {
            #region Contracts

            if (executeDelegate == null) throw new ArgumentNullException();

            #endregion

            // ConnectionProxyCollection
            foreach (ConnectionProxy<TService> connectionProxy in _connectionProxyCollection)
            {
                try
                {
                    // Execute
                    TResult result = executeDelegate(connectionProxy);

                    // Return
                    return result;
                }
                catch
                {
                    // Nothing

                }
            }

            // Throw
            throw new ExecuteIgnoredException();
        }

        public TResult Execute<TResult>(Func<ConnectionProxy<TService>, TResult> executeDelegate, Func<TResult, bool> finishPredicate)
        {
            #region Contracts

            if (executeDelegate == null) throw new ArgumentNullException();
            if (finishPredicate == null) throw new ArgumentNullException();

            #endregion

            // ConnectionProxyCollection
            foreach (ConnectionProxy<TService> connectionProxy in _connectionProxyCollection)
            {
                try
                {
                    // Execute
                    TResult result = executeDelegate(connectionProxy);

                    // Return
                    if (finishPredicate(result) == true) return result;                    
                }
                catch
                {
                    // Nothing

                }
            }

            // Throw
            throw new ExecuteIgnoredException();
        }

        
        public void ExecuteAll(Action<ConnectionProxy<TService>> executeDelegate)
        {
            #region Contracts

            if (executeDelegate == null) throw new ArgumentNullException();

            #endregion

            // ConnectionProxyCollection
            foreach (ConnectionProxy<TService> connectionProxy in _connectionProxyCollection)
            {
                try
                {
                    // Execute
                    executeDelegate(connectionProxy);
                }
                catch
                {
                    // Throw
                    throw;
                }
            }
        }

        public IEnumerable<TResult> ExecuteAll<TResult>(Func<ConnectionProxy<TService>, TResult> executeDelegate)
        {
            #region Contracts

            if (executeDelegate == null) throw new ArgumentNullException();

            #endregion

            // Result
            List<TResult> resultCollection = new List<TResult>();

            // ConnectionProxyCollection
            foreach (ConnectionProxy<TService> connectionProxy in _connectionProxyCollection)
            {
                try
                {
                    // Execute
                    TResult result = executeDelegate(connectionProxy);

                    // Add
                    resultCollection.Add(result);
                }
                catch
                {
                    // Throw
                    throw;
                }
            }

            // Return
            return resultCollection;
        }


        // Handlers
        private void Proxy_Connected(object sender, EventArgs e)
        {
            // Refresh
            this.Refresh();
        }

        private void Proxy_Disconnected(object sender, EventArgs e)
        {
            // Refresh
            this.Refresh();
        }

        private void Proxy_Heartbeating(object sender, EventArgs e)
        {
            #region Contracts

            if (sender == null) throw new ArgumentException();
            if (e == null) throw new ArgumentException();

            #endregion

            // Require
            if (this.IsConnected == false) return;

            // Notify
            this.OnHeartbeating();
        }
    }

    public class ConnectionProxyHost<TService, TCallback> : ConnectionProxyHost
        where TService : class, IConnectionService
        where TCallback : class
    {
        // Fields
        private readonly object _syncObject = new object();

        private readonly IEnumerable<ConnectionProxy<TService, TCallback>> _connectionProxyCollection = null;

        private readonly Func<IEnumerable<ConnectionProxy<TService, TCallback>>, bool> _isConnectedPredicate = null;

        private bool _isConnected = false;


        // Constructors
        public ConnectionProxyHost(IEnumerable<ConnectionProxy<TService, TCallback>> connectionProxyCollection, Func<IEnumerable<ConnectionProxy>, bool> isConnectedPredicate)
        {
            #region Contracts

            if (connectionProxyCollection == null) throw new ArgumentNullException();
            if (isConnectedPredicate == null) throw new ArgumentNullException();

            #endregion

            // ConnectionProxyCollection
            _connectionProxyCollection = connectionProxyCollection;

            // IsConnectedPredicate 
            _isConnectedPredicate = isConnectedPredicate;
        }


        // Properties
        public override bool IsConnected
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
        public override void Open()
        {
            // ConnectionProxyCollection
            foreach (ConnectionProxy<TService, TCallback> connectionProxy in _connectionProxyCollection)
            {
                connectionProxy.Connected += this.Proxy_Connected;
                connectionProxy.Disconnected += this.Proxy_Disconnected;
                connectionProxy.Heartbeating += this.Proxy_Heartbeating;
                connectionProxy.Open();
            }
        }

        public override void Close()
        {
            // ConnectionProxyCollection
            foreach (ConnectionProxy<TService, TCallback> connectionProxy in _connectionProxyCollection)
            {
                connectionProxy.Close();
                connectionProxy.Connected -= this.Proxy_Connected;
                connectionProxy.Disconnected -= this.Proxy_Disconnected;
                connectionProxy.Heartbeating -= this.Proxy_Heartbeating;
            }
        }

        private void Refresh()
        {
            // IsConnected
            bool isConnected = _isConnectedPredicate(_connectionProxyCollection);

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


        public void Execute(Action<ConnectionProxy<TService, TCallback>> executeDelegate)
        {
            #region Contracts

            if (executeDelegate == null) throw new ArgumentNullException();

            #endregion

            // ConnectionProxyCollection
            foreach (ConnectionProxy<TService, TCallback> connectionProxy in _connectionProxyCollection)
            {
                try
                {
                    // Execute
                    executeDelegate(connectionProxy);

                    // Return
                    return;
                }
                catch
                {
                    // Nothing

                }
            }

            // Throw
            throw new ExecuteIgnoredException();
        }

        public TResult Execute<TResult>(Func<ConnectionProxy<TService, TCallback>, TResult> executeDelegate)
        {
            #region Contracts

            if (executeDelegate == null) throw new ArgumentNullException();

            #endregion

            // ConnectionProxyCollection
            foreach (ConnectionProxy<TService, TCallback> connectionProxy in _connectionProxyCollection)
            {
                try
                {
                    // Execute
                    TResult result = executeDelegate(connectionProxy);

                    // Return
                    return result;
                }
                catch
                {
                    // Nothing

                }
            }

            // Throw
            throw new ExecuteIgnoredException();
        }
        
        public void ExecuteAll(Action<ConnectionProxy<TService, TCallback>> executeDelegate)
        {
            #region Contracts

            if (executeDelegate == null) throw new ArgumentNullException();

            #endregion

            // ConnectionProxyCollection
            foreach (ConnectionProxy<TService, TCallback> connectionProxy in _connectionProxyCollection)
            {
                try
                {
                    // Execute
                    executeDelegate(connectionProxy);
                }
                catch
                {
                    // Throw
                    throw;
                }
            }
        }

        public IEnumerable<TResult> ExecuteAll<TResult>(Func<ConnectionProxy<TService, TCallback>, TResult> executeDelegate)
        {
            #region Contracts

            if (executeDelegate == null) throw new ArgumentNullException();

            #endregion

            // Result
            List<TResult> resultCollection = new List<TResult>();

            // ConnectionProxyCollection
            foreach (ConnectionProxy<TService, TCallback> connectionProxy in _connectionProxyCollection)
            {
                try
                {
                    // Execute
                    TResult result = executeDelegate(connectionProxy);

                    // Add
                    resultCollection.Add(result);
                }
                catch
                {
                    // Throw
                    throw;
                }
            }

            // Return
            return resultCollection;
        }


        // Handlers
        private void Proxy_Connected(object sender, EventArgs e)
        {
            // Refresh
            this.Refresh();
        }

        private void Proxy_Disconnected(object sender, EventArgs e)
        {
            // Refresh
            this.Refresh();
        }

        private void Proxy_Heartbeating(object sender, EventArgs e)
        {
            #region Contracts

            if (sender == null) throw new ArgumentException();
            if (e == null) throw new ArgumentException();

            #endregion

            // Require
            if (this.IsConnected == false) return;

            // Notify
            this.OnHeartbeating();
        }
    }    
}