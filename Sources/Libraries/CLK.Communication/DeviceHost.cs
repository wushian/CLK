using CLK.Diagnostics;
using CLK.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace CLK.Communication
{
    public abstract class DeviceHost<TDevice, TAddress>
        where TDevice : Device<TAddress>
        where TAddress : DeviceAddress
    {
        // Fields
        private readonly object _syncRoot = new object();

        private readonly PortableStarterStoperLock _operateLock = new PortableStarterStoperLock();

        private readonly List<TDevice> _deviceCollection = new List<TDevice>();

        private readonly IDeviceHostStrategy<TAddress> _deviceHostStrategy = null;


        // Constructors
        public DeviceHost(IDeviceHostStrategy<TAddress> deviceHostStrategy) 
        {
            #region Contracts

            if (deviceHostStrategy == null) throw new ArgumentNullException();

            #endregion

            // Arguments
            _deviceHostStrategy = deviceHostStrategy;
        }


        // Methods
        public virtual void Open()
        {
            // EnterStartLock
            if (_operateLock.EnterStartLock() == false) return;

            // Start
            try
            {               
                // Device
                IEnumerable<TDevice> deviceCollection = null;
                lock (_syncRoot) { deviceCollection = _deviceCollection.ToArray(); }
                foreach (TDevice device in deviceCollection)
                {
                    device.Dispose();
                }

                // Strategy
                _deviceHostStrategy.DeviceArrived += this.DeviceHostStrategy_DeviceArrived;
                _deviceHostStrategy.DeviceDeparted += this.DeviceHostStrategy_DeviceDeparted;
                _deviceHostStrategy.Start();                
            }
            finally
            {
                // ExitStartLock
                _operateLock.ExitStartLock();
            }
        }        
                
        public virtual void Close()
        {
            // EnterStopLock
            if (_operateLock.EnterStopLock() == false) return;

            // Close
            try
            {
                // Strategy
                _deviceHostStrategy.Stop();
                _deviceHostStrategy.DeviceArrived -= this.DeviceHostStrategy_DeviceArrived;
                _deviceHostStrategy.DeviceDeparted -= this.DeviceHostStrategy_DeviceDeparted;
                
                // Device
                IEnumerable<TDevice> deviceCollection = null;
                lock (_syncRoot) { deviceCollection = _deviceCollection.ToArray(); }
                foreach (TDevice device in deviceCollection)
                {
                    device.Dispose();
                }                
            }
            finally
            {
                // ExitStopLock
                _operateLock.ExitStopLock();
            }
        }


        protected TDevice GetDevice(Func<TDevice, bool> predicate)
        {
            #region Contracts

            if (predicate == null) throw new ArgumentNullException();

            #endregion

            // Result
            TDevice device = null;

            // Search 
            lock (_syncRoot)
            {
                foreach (TDevice existDevice in _deviceCollection)
                {
                    if (predicate(existDevice) == true)
                    {
                        device = existDevice;
                        break;
                    }
                }
            }

            // Return
            return device;
        }

        private TDevice GetDevice(TAddress localAddress, TAddress remoteAddress)
        {
            #region Contracts

            if (localAddress == null) throw new ArgumentNullException();
            if (remoteAddress == null) throw new ArgumentNullException();

            #endregion

            // Search 
            foreach (TDevice existDevice in _deviceCollection)
            {
                if (existDevice.LocalAddress.EqualAddress(localAddress) == true)
                {
                    if (existDevice.RemoteAddress.EqualAddress(remoteAddress) == true)
                    {
                        return existDevice;
                    }
                }
            }
            return null;
        }

        private void AttachDevice(TAddress localAddress, TAddress remoteAddress)
        {
            #region Contracts

            if (localAddress == null) throw new ArgumentNullException();
            if (remoteAddress == null) throw new ArgumentNullException();

            #endregion

            // Result
            TDevice device = null;

            // Attach
            lock (_syncRoot)
            {
                // Search 
                device = this.GetDevice(localAddress, remoteAddress);
                if (device != null) return;

                // Create
                device = this.CreateDevice(localAddress, remoteAddress);
                if (device == null) return;

                // Add
                _deviceCollection.Add(device);

                // Events
                device.DeviceOpened += this.Device_DeviceOpened;
                device.DeviceClosed += this.Device_DeviceClosed;
            }

            // Notify
            this.OnDeviceArrived(device);

            // Open
            device.Connect();
        }

        private void DetachDevice(TAddress localAddress, TAddress remoteAddress)
        {
            #region Contracts

            if (localAddress == null) throw new ArgumentNullException();
            if (remoteAddress == null) throw new ArgumentNullException();

            #endregion

            // Result
            TDevice device = null;

            // Detach
            lock (_syncRoot)
            {
                // Search 
                device = this.GetDevice(localAddress, remoteAddress);
                if (device == null) return;

                // Remove
                _deviceCollection.Remove(device);

                // Events
                device.DeviceOpened -= this.Device_DeviceOpened;
                device.DeviceClosed -= this.Device_DeviceClosed;
            }

            // Close
            device.Dispose();

            // Notify
            this.OnDeviceDeparted(device);
        }


        private TDevice CreateDevice(TAddress localAddress, TAddress remoteAddress)
        {
            #region Contracts

            if (localAddress == null) throw new ArgumentNullException();
            if (remoteAddress == null) throw new ArgumentNullException();

            #endregion

            // Result
            TDevice device = null;

            // Device
            device = this.CreateDevice();
            if (device == null) throw new InvalidOperationException();

            // Strategy
            IDeviceStrategy<TAddress> deviceStrategy = _deviceHostStrategy.GetDeviceStrategy(localAddress, remoteAddress);
            if (device == null) throw new InvalidOperationException();        

            // Initialize
            device.Initialize(deviceStrategy);

            // Return
            return device;
        }

        protected abstract TDevice CreateDevice();
                

        // Handlers
        private void DeviceHostStrategy_DeviceArrived(TAddress localAddress, TAddress remoteAddress)
        {
            #region Contracts

            if (localAddress == null) throw new ArgumentNullException();
            if (remoteAddress == null) throw new ArgumentNullException();

            #endregion

            // Attach
            this.AttachDevice(localAddress, remoteAddress);
        }

        private void DeviceHostStrategy_DeviceDeparted(TAddress localAddress, TAddress remoteAddress)
        {
            #region Contracts

            if (localAddress == null) throw new ArgumentNullException();
            if (remoteAddress == null) throw new ArgumentNullException();

            #endregion

            // Result
            TDevice device = null;

            // Detach
            lock (_syncRoot)
            {
                // Search 
                device = this.GetDevice(localAddress, remoteAddress);
                if (device == null) return;
            }

            // Dispose
            device.Dispose();
        }

        private void Device_DeviceOpened(TAddress localAddress, TAddress remoteAddress)
        {
            #region Contracts

            if (localAddress == null) throw new ArgumentNullException();
            if (remoteAddress == null) throw new ArgumentNullException();

            #endregion

            // Nothing

        }

        private void Device_DeviceClosed(TAddress localAddress, TAddress remoteAddress)
        {
            #region Contracts

            if (localAddress == null) throw new ArgumentNullException();
            if (remoteAddress == null) throw new ArgumentNullException();

            #endregion

            // Detach
            this.DetachDevice(localAddress, remoteAddress);
        }


        // Events
        protected event Action<TDevice> DeviceArrived;
        private void OnDeviceArrived(TDevice device)
        {
            #region Contracts

            if (device == null) throw new ArgumentNullException();

            #endregion

            var handler = this.DeviceArrived;
            if (handler != null)
            {
                handler(device);
            }
        }

        protected event Action<TDevice> DeviceDeparted;
        private void OnDeviceDeparted(TDevice device)
        {
            #region Contracts

            if (device == null) throw new ArgumentNullException();

            #endregion

            var handler = this.DeviceDeparted;
            if (handler != null)
            {
                handler(device);
            }
        }
    }
}
