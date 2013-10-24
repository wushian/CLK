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
    public class ConnectionProxy<TChannel>
        where TChannel : class, IConnectionService
    {
        // Fields
        private readonly object _syncRoot = new object();

        private int _heartbeatInterval = 5000;

        private int _reconnectInterval = 5000;
                

        private bool _isClosed = true;

        private readonly ManualResetEvent _isClosedEvent = new ManualResetEvent(true);
        

        private Thread _executeThread = null;

        private readonly ManualResetEvent _executeThreadEvent = new ManualResetEvent(true);

        private readonly AutoResetEvent _executeTriggerEvent = new AutoResetEvent(false);
       

        private readonly ChannelFactory<TChannel> _channelFactory = null;
       
        private IContextChannel _channel = null;

        private TChannel _service = null;

        private bool _isConnected = false;
               

        // Constructors
        public ConnectionProxy(ChannelFactory<TChannel> channelFactory)
        {
            #region Contracts

            if (channelFactory == null) throw new ArgumentNullException();

            #endregion

            // ChannelFactory     
            _channelFactory = channelFactory;              
        }


        // Properties
        public TChannel Service
        {
            get
            {
                // Require
                TChannel service = _service;
                if (service == null) throw new CommunicationException();

                // Return
                return service;
            }
        }

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


        // Methods
        public virtual void Open()
        {
            // Require
            lock (_syncRoot)
            {
                if (_isClosed == false) return;
                _isClosed = false;
                _isClosedEvent.Reset();
            }

            // ChannelFactory 
            _channelFactory.Open();

            // ExecuteThread
            _executeThreadEvent.Reset();
            _executeThread = new Thread(this.ExecuteOperation);
            _executeThread.IsBackground = false;
            _executeThread.Start();
        }

        public virtual void Close()
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
            try
            {
                _channelFactory.Close();
            }
            catch (Exception)
            {
                _channelFactory.Abort();
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
                    if (_channel == null)
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
            if (_channel != null) return;

            // Result
            bool executeResult = false;

            // Connect
            try
            {
                // Service
                _service = _channelFactory.CreateChannel();
                if (_service == null) throw new InvalidOperationException();
                if ((_service is IContextChannel) == false) throw new InvalidOperationException();

                // Channel
                _channel = _service as IContextChannel;
                _channel.Closed += this.Channel_Closed;
                _channel.Faulted += this.Channel_Faulted;        
                _channel.Open();

                // Result
                executeResult = true;
            }
            catch (Exception)
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
            if (_channel == null) return;
            if (_channel.State == CommunicationState.Opened && _isClosed == false) return;           

            // Result
            bool executeResult = false;

            // Disconnect
            try
            {
                // Service
                _service = null;

                // Channel
                _channel.Close();
                _channel.Closed -= this.Channel_Closed;
                _channel.Faulted -= this.Channel_Faulted;        
                _channel = null;
            }
            catch (Exception)
            {
                // Service
                _service = null;

                // Channel
                _channel.Abort();
                _channel.Closed -= this.Channel_Closed;
                _channel.Faulted -= this.Channel_Faulted;                
                _channel = null;
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
            if (_channel == null) return;
            if (_channel.State != CommunicationState.Opened) return;

            // Result
            bool executeResult = false;

            // Heartbeat            
            try
            {
                // Service
                _service.Heartbeat();

                // Result
                executeResult = true;        
            }
            catch (Exception)
            {
                // Result
                executeResult = false;        
            }

            // Notify
            if (executeResult == true)
            {
                this.OnHeartbeating();
            }
        }


        // Handlers
        private void Channel_Closed(object sender, EventArgs e)
        {
            // Trigger
            _executeTriggerEvent.Set();
        }

        private void Channel_Faulted(object sender, EventArgs e)
        {
            // Trigger
            _executeTriggerEvent.Set();    
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

        public event EventHandler Heartbeating;
        private void OnHeartbeating()
        {
            var handler = this.Heartbeating;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
