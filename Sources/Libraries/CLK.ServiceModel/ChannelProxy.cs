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
    public abstract class ChannelProxy : Channel
    {
        // Constructors
        internal ChannelProxy() { }


        // Properties
        public abstract bool IsConnected { get; }


        // Methods
        public abstract void Open();

        public abstract void Close();


        // Events
        public event EventHandler Connected;
        internal void OnConnected()
        {
            var handler = this.Connected;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public event EventHandler Disconnected;
        internal void OnDisconnected()
        {
            var handler = this.Disconnected;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }

    public abstract class ChannelProxy<TService> : ChannelProxy
        where TService : class, IChannelService
    {
        // Fields
        private readonly object _syncRoot = new object();

        private readonly Binding _binding = null;

        private readonly string _adress = null;

        private int _heartbeatInterval = 5000;

        private int _reconnectInterval = 5000;


        private bool _isClosed = true;

        private readonly ManualResetEvent _isClosedEvent = new ManualResetEvent(true);


        private Thread _executeThread = null;

        private readonly ManualResetEvent _executeThreadEvent = new ManualResetEvent(true);

        private readonly AutoResetEvent _executeTriggerEvent = new AutoResetEvent(false);


        private ChannelFactory<TService> _channelFactory = null;

        private IContextChannel _contextChannel = null;

        private TService _service = null;

        private bool _isConnected = false;


        // Constructors
        public ChannelProxy(Binding binding, string adress)
        {
            #region Contracts

            if (binding == null) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(adress) == true) throw new ArgumentNullException();

            #endregion

            // Arguments
            _binding = binding;
            _adress = adress;
        }

        internal virtual ChannelFactory<TService> CreateChannelFactory(Binding binding, string adress)
        {
            #region Contracts

            if (binding == null) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(adress) == true) throw new ArgumentNullException();

            #endregion

            // Return
            return new ChannelFactory<TService>(binding, new EndpointAddress(adress));
        }


        // Properties        
        public int HeartbeatInterval
        {
            get
            {
                return _heartbeatInterval;
            }
            set
            {
                if (0 >= value) value = Timeout.Infinite;
                _heartbeatInterval = value;
            }
        }

        public int ReconnectInterval
        {
            get
            {
                return _reconnectInterval;
            }
            set
            {
                if (0 >= value) value = Timeout.Infinite;
                _reconnectInterval = value;
            }
        }

        public override bool IsConnected
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

        public TService Service
        {
            get
            {
                // Get
                TService service = _service;
                if (service == null) throw new CommunicationException();

                // Return
                return service;
            }
        }


        // Methods
        public override void Open()
        {
            // Require
            lock (_syncRoot)
            {
                if (_isClosed == false) return;
                _isClosed = false;
                _isClosedEvent.Reset();
            }

            // ChannelFactory 
            if (_channelFactory == null) _channelFactory = this.CreateChannelFactory(_binding, _adress);
            if (_channelFactory == null) throw new InvalidOperationException();
            _channelFactory.Open();

            // ExecuteThread
            _executeThreadEvent.Reset();
            _executeThread = new Thread(this.ExecuteOperation);
            _executeThread.IsBackground = false;
            _executeThread.Start();
        }

        public override void Close()
        {
            // Require
            lock (_syncRoot)
            {
                if (_isClosed == true) return;
                _isClosed = true;
                _isClosedEvent.Set();
            }

            // ExecuteThread
            _executeThreadEvent.WaitOne();

            // ChannelFactory 
            if (_channelFactory != null)
            {
                try
                {
                    _channelFactory.Close();
                }
                catch
                {
                    _channelFactory.Abort();
                }
            }
        }


        private void ExecuteOperation()
        {
            try
            {
                // WaitHandles
                WaitHandle[] waitHandles = new WaitHandle[]
                {
                    _isClosedEvent,
                    _executeTriggerEvent,
                };

                // Execute
                while (true)
                {
                    // Require
                    if (_isClosed == true) return;

                    // Execute       
                    this.ExecuteOperation_Connect();
                    this.ExecuteOperation_Heartbeat();
                    this.ExecuteOperation_Disconnect();

                    // Wait
                    if (_contextChannel == null)
                    {
                        WaitHandle.WaitAny(waitHandles, this.ReconnectInterval);
                    }
                    else
                    {
                        WaitHandle.WaitAny(waitHandles, this.HeartbeatInterval);
                    }
                }
            }
            catch (Exception ex)
            {
                // Fail
                Debug.Fail(string.Format("Action:{0}, State:{1}, Message:{2}", "ExecuteOperation", "Exception", ex.Message));
            }
            finally
            {
                // End
                _executeThreadEvent.Set();
            }
        }

        private void ExecuteOperation_Connect()
        {
            // Require
            if (_service != null) return;
            if (_contextChannel != null) return;

            // Result
            bool executeResult = false;

            // Connect
            try
            {
                // Create
                object channelObject = _channelFactory.CreateChannel();
                if (channelObject == null) throw new InvalidOperationException();
                if ((channelObject is TService) == false) throw new InvalidOperationException();
                if ((channelObject is IContextChannel) == false) throw new InvalidOperationException();

                // Service
                _service = channelObject as TService;

                // ContextChannel
                _contextChannel = channelObject as IContextChannel;
                _contextChannel.Closed += this.ContextChannel_Closed;
                _contextChannel.Faulted += this.ContextChannel_Faulted;
                _contextChannel.Open();

                // Result
                executeResult = true;
            }
            catch
            {
                // Result
                executeResult = false;
            }

            // IsConnected
            if (executeResult == true)
            {
                lock (_syncRoot)
                {
                    if (_isConnected == true) return;
                    _isConnected = true;
                }
            }

            // Notify
            if (executeResult == true)
            {
                this.OnConnected();
            }
        }

        private void ExecuteOperation_Disconnect()
        {
            // Require
            if (_service == null) return;
            if (_contextChannel == null) return;
            if (_contextChannel.State == CommunicationState.Opened && _isClosed == false) return;

            // Result
            bool executeResult = false;

            // Disconnect
            try
            {
                // Service
                _service = null;

                // ContextChannel
                _contextChannel.Close();
                _contextChannel.Closed -= this.ContextChannel_Closed;
                _contextChannel.Faulted -= this.ContextChannel_Faulted;
                _contextChannel = null;
            }
            catch
            {
                // Service
                _service = null;

                // ContextChannel
                _contextChannel.Abort();
                _contextChannel.Closed -= this.ContextChannel_Closed;
                _contextChannel.Faulted -= this.ContextChannel_Faulted;
                _contextChannel = null;
            }
            finally
            {
                // Result
                executeResult = true;
            }

            // IsConnected
            if (executeResult == true)
            {
                lock (_syncRoot)
                {
                    if (_isConnected == false) return;
                    _isConnected = false;
                }
            }

            // Notify
            if (executeResult == true)
            {
                this.OnDisconnected();
            }
        }

        private void ExecuteOperation_Heartbeat()
        {
            // Require
            if (_service == null) return;
            if (_contextChannel == null) return;
            if (_contextChannel.State != CommunicationState.Opened) return;

            // Heartbeat            
            try
            {
                // Service
                _service.Heartbeat();
            }
            catch
            {
                // Nothing

            }
        }


        // Handlers
        private void ContextChannel_Closed(object sender, EventArgs e)
        {
            // Trigger
            _executeTriggerEvent.Set();
        }

        private void ContextChannel_Faulted(object sender, EventArgs e)
        {
            // Trigger
            _executeTriggerEvent.Set();
        }
    }

    public abstract class ChannelProxy<TService, TCallback> : ChannelProxy<TService>
        where TService : class, IChannelService
        where TCallback : class
    {
        // Constructors
        public ChannelProxy(Binding binding, string adress)
            : base(binding, adress)
        {
            // Require
            if (typeof(TCallback).IsAssignableFrom(this.GetType()) == false) throw new InvalidOperationException();
        }

        internal override ChannelFactory<TService> CreateChannelFactory(Binding binding, string adress)
        {
            #region Contracts

            if (binding == null) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(adress) == true) throw new ArgumentNullException();

            #endregion

            // Return
            return new DuplexChannelFactory<TService>(this, binding, new EndpointAddress(adress));
        }
    }
}
