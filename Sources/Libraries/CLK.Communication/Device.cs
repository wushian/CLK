using CLK.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Timers;

namespace CLK.Communication
{
    public abstract class Device<TAddress> : IDisposable
        where TAddress : DeviceAddress
    {
        // Fields        
        private readonly object _syncRoot = new object();

        private IDeviceStrategy<TAddress> _deviceStrategy = null;
        
        private IEnumerable<DeviceCommand<TAddress>> _commandCollection = null;

        private bool _isOpend = false;

        private bool _isClosed = false;

        private bool _isDisposed = false;

        
        // Constructors
        public Device() { }

        internal void Initialize(IDeviceStrategy<TAddress> deviceStrategy)
        {
            #region Contracts

            if (deviceStrategy == null) throw new ArgumentNullException();

            #endregion

            // Strategy   
            _deviceStrategy = deviceStrategy;

            // Command
            _commandCollection = this.CreateAllCommand();
            if (_commandCollection == null) throw new InvalidOperationException();

            // CommandStrategy
            var commandStrategyCollection = deviceStrategy.GetAllCommandStrategy();
            if (commandStrategyCollection == null) throw new InvalidOperationException();

            // Initialize
            foreach (DeviceCommand<TAddress> command in _commandCollection)
            {
                // Address
                command.Initialize(_deviceStrategy.LocalAddress, _deviceStrategy.RemoteAddress);

                // Strategy
                foreach (IDeviceCommandStrategy<TAddress> commandStrategy in commandStrategyCollection)
                {
                    command.Initialize(commandStrategy);
                }
            }
        }

        public void Dispose()
        {
            // Close
            this.Close();
        }


        // Properties        
        public TAddress LocalAddress { get { return _deviceStrategy.LocalAddress; } }

        public TAddress RemoteAddress { get { return _deviceStrategy.RemoteAddress; } }


        // Methods
        private void ApplyTimeTicked(long nowTicks)
        {
            // Command
            foreach (DeviceCommand<TAddress> command in _commandCollection)
            {
                command.ApplyTimeTicked(nowTicks);
            }
        }

        private void Open()
        {
            // Flag
            lock (_syncRoot)
            {
                if (_isOpend == true) return;
                _isOpend = true;
                _isDisposed = false;
            }

            // Require
            if (_deviceStrategy == null) throw new InvalidOperationException();
            if (_commandCollection == null) throw new InvalidOperationException();

            // Command
            foreach (DeviceCommand<TAddress> command in _commandCollection)
            {
                command.Start();
            }

            // Strategy
            _deviceStrategy.Ticked += this.DeviceStrategy_Ticked;
            _deviceStrategy.Start();

            // Notify
            this.OnDeviceOpened();
        }

        private void Close()
        {
            // Flag
            lock (_syncRoot)
            {
                if (_isClosed == true) return;
                _isClosed = true;
                _isDisposed = true;
            }
                                    
            // Strategy
            _deviceStrategy.Stop();
            _deviceStrategy.Ticked -= this.DeviceStrategy_Ticked;

            // Command
            foreach (DeviceCommand<TAddress> command in _commandCollection)
            {
                command.Stop();
            }

            // Notify
            this.OnDeviceClosed();
        }


        public void Connect()
        {
            // Require
            lock (_syncRoot)
            {
                if (_isDisposed == true) throw new ObjectDisposedException(this.GetType().Name);
            }

            // Open
            this.Open();
        }
        
        public TCommand GetCommand<TCommand>() where TCommand : DeviceCommand<TAddress>
        {
            // Command
            return _commandCollection.OfType<TCommand>().FirstOrDefault();
        }
        
        protected abstract IEnumerable<DeviceCommand<TAddress>> CreateAllCommand();


        // Handlers
        private void DeviceStrategy_Ticked(long nowTicks)
        {
            // Apply
            this.ApplyTimeTicked(nowTicks);
        }


        // Events
        internal event Action<TAddress, TAddress> DeviceOpened;
        private void OnDeviceOpened()
        {
            var handler = this.DeviceOpened;
            if (handler != null)
            {
                handler(_deviceStrategy.LocalAddress, _deviceStrategy.RemoteAddress);
            }
        }

        internal event Action<TAddress, TAddress> DeviceClosed;
        private void OnDeviceClosed()
        {
            var handler = this.DeviceClosed;
            if (handler != null)
            {
                handler(_deviceStrategy.LocalAddress, _deviceStrategy.RemoteAddress);
            }
        }
    }
}
