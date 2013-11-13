using CLK.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Timers;

namespace CLK.Communication
{
    public abstract class Device<TDeviceAddress> : IDisposable
        where TDeviceAddress : DeviceAddress
    {
        // Fields        
        private readonly object _syncRoot = new object();

        private readonly PortableStarterStoperLock _operateLock = new PortableStarterStoperLock();

        private readonly TDeviceAddress _localDeviceAddress = null;

        private readonly TDeviceAddress _remoteDeviceAddress = null;

        private readonly IEnumerable<DeviceCommand<TDeviceAddress>> _commandCollection = null;

        private bool _isDisposed = false;

        
        // Constructors
        public Device(TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress, IEnumerable<DeviceCommand<TDeviceAddress>> commandCollection)
        {
            #region Contracts

            if (localDeviceAddress == null) throw new ArgumentNullException();
            if (remoteDeviceAddress == null) throw new ArgumentNullException();
            if (commandCollection == null) throw new ArgumentNullException();           

            #endregion

            // Arguments   
            _localDeviceAddress = localDeviceAddress;
            _remoteDeviceAddress = remoteDeviceAddress;
            _commandCollection = commandCollection;
        }

        public void Dispose()
        {
            // Flag
            lock(_syncRoot)
            {
                if (_isDisposed == true) return;
                _isDisposed = true;
            }

            // Notify
            this.OnDeviceDisposed();                       
        }


        // Properties
        public TDeviceAddress LocalDeviceAddress { get { return _localDeviceAddress; } }

        public TDeviceAddress RemoteDeviceAddress { get { return _remoteDeviceAddress; } }


        // Methods
        internal void ApplyTimeTicked(long nowTicks)
        {
            // Command
            foreach (DeviceCommand<TDeviceAddress> command in _commandCollection)
            {
                command.ApplyTimeTicked(nowTicks);
            }
        }

        internal void Open()
        {
            // EnterStartLock
            if (_operateLock.EnterStartLock() == false) return;

            // Open
            try
            {
                // Flag
                lock (_syncRoot)
                {
                    _isDisposed = false;
                }
                
                // Command
                foreach (DeviceCommand<TDeviceAddress> command in _commandCollection)
                {
                    command.Start();
                }
            }
            finally
            {
                // ExitStartLock
                _operateLock.ExitStartLock();
            }
        }

        internal void Close()
        {
            // EnterStopLock
            if (_operateLock.EnterStopLock() == false) return;

            // Close
            try
            {
                // Flag
                lock (_syncRoot)
                {
                    _isDisposed = true;
                }

                // Command
                foreach (DeviceCommand<TDeviceAddress> command in _commandCollection)
                {
                    command.Stop();
                }
            }
            finally
            {
                // ExitStopLock
                _operateLock.ExitStopLock();
            }
        }


        public TCommand GetCommand<TCommand>() where TCommand : DeviceCommand<TDeviceAddress>
        {
            // Command
            return _commandCollection.OfType<TCommand>().FirstOrDefault();
        }
        
        
        // Events
        internal event Action<TDeviceAddress, TDeviceAddress> DeviceDisposed;
        private void OnDeviceDisposed()
        {
            var handler = this.DeviceDisposed;
            if (handler != null)
            {
                handler(this.LocalDeviceAddress, this.RemoteDeviceAddress);
            }
        }
    }
}
