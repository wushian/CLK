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
    public abstract class DeviceHost<TDeviceAddress>
        where TDeviceAddress : DeviceAddress
    {
        // Fields
        private readonly object _syncRoot = new object();

        private readonly PortableStarterStoperLock _operateLock = new PortableStarterStoperLock();

        private readonly List<Device<TDeviceAddress>> _deviceCollection = new List<Device<TDeviceAddress>>();

        private readonly TDeviceAddress _localDeviceAddress = null;

        private readonly IDeviceCommandFactory<TDeviceAddress> _commandFactory = null;

        private readonly PortableTimer _operateTimer = null;


        // Constructors
        public DeviceHost(TDeviceAddress localDeviceAddress, IDeviceCommandFactory<TDeviceAddress> commandFactory, PortableTimer operateTimer)
        {
            #region Contracts

            if (localDeviceAddress == null) throw new ArgumentNullException();
            if (commandFactory == null) throw new ArgumentNullException();
            if (operateTimer == null) throw new ArgumentNullException();

            #endregion

            // Arguments 
            _localDeviceAddress = localDeviceAddress;
            _commandFactory = commandFactory;
            _operateTimer = operateTimer;
        }


        // Properties
        public TDeviceAddress LocalDeviceAddress { get { return _localDeviceAddress; } }


        // Methods
        public void Open()
        {
            // EnterStartLock
            if (_operateLock.EnterStartLock() == false) return;

            // Open
            try
            {
                // Timer
                _operateTimer.Ticked += this.Timer_Ticked;
                _operateTimer.Start();

                // Open
                this.OpenHost();
            }
            finally
            {
                // ExitStartLock
                _operateLock.ExitStartLock();
            }
        }

        public void Close()
        {
            // EnterStopLock
            if (_operateLock.EnterStopLock() == false) return;

            // Close
            try
            {
                // Close
                this.CloseHost();

                // Timer
                _operateTimer.Stop();
            }
            finally
            {
                // ExitStopLock
                _operateLock.ExitStopLock();
            }
        }

        protected abstract void OpenHost();

        protected abstract void CloseHost();


        public Device<TDeviceAddress> FindDevice(Func<Device<TDeviceAddress>, bool> predicate)
        {
            #region Contracts

            if (predicate == null) throw new ArgumentNullException();

            #endregion

            // Result
            Device<TDeviceAddress> device = null;

            // Search 
            lock (_syncRoot)
            {

                foreach (Device<TDeviceAddress> existDevice in _deviceCollection)
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

        public IEnumerable<Device<TDeviceAddress>> FindAllDevice(Func<Device<TDeviceAddress>, bool> predicate)
        {
            #region Contracts

            if (predicate == null) throw new ArgumentNullException();

            #endregion

            // Result
            List<Device<TDeviceAddress>> deviceCollection = null;

            // Search 
            lock (_syncRoot)
            {

                foreach (Device<TDeviceAddress> existDevice in _deviceCollection)
                {
                    if (predicate(existDevice) == true)
                    {
                        deviceCollection.Add(existDevice);
                    }
                }
            }

            // Return
            return deviceCollection;
        }

        protected Device<TDeviceAddress> CreateDevice(TDeviceAddress remoteDeviceAddress)
        {
            #region Contracts

            if (remoteDeviceAddress == null) throw new ArgumentNullException();

            #endregion

            // Result
            Device<TDeviceAddress> device = null;

            // Get
            lock (_syncRoot)
            {
                // Search 
                foreach (Device<TDeviceAddress> existDevice in _deviceCollection)
                {
                    if (existDevice.RemoteDeviceAddress.EqualAddress(remoteDeviceAddress) == true)
                    {
                        return existDevice;
                    }
                }

                // Create
                device = new Device<TDeviceAddress>(_localDeviceAddress, remoteDeviceAddress, _commandFactory);

                // Add
                _deviceCollection.Add(device);

                // Events
                device.DeviceDisposed += this.Device_DeviceDisposed;
            }

            // Open
            device.Open();

            // Notify
            this.OnDeviceArrived(device);

            // Return
            return device;
        }

        private void DetachDevice(Device<TDeviceAddress> device)
        {
            #region Contracts

            if (device == null) throw new ArgumentNullException();

            #endregion

            // Detach
            lock (_syncRoot)
            {
                // Events
                device.DeviceDisposed -= this.Device_DeviceDisposed;

                // Remove
                _deviceCollection.Remove(device);
            }

            // Notify
            this.OnDeviceDeparted(device);
        }


        // Handlers
        private void Timer_Ticked(object sender, EventArgs e)
        {
            // NowTicks
            long nowTicks = DateTime.Now.Ticks;

            // Device
            lock (_syncRoot)
            {
                foreach (Device<TDeviceAddress> device in _deviceCollection)
                {
                    WaitCallback executeDelegate = delegate(object state)
                    {
                        try
                        {
                            device.ApplyTimeTicked(nowTicks);
                        }
                        catch (Exception ex)
                        {
                            Debug.Fail(string.Format("Action:{0}, State:{1}, Message:{2}", "ApplyTimeTicked", "Exception", ex.Message));
                        }
                    };
                    ThreadPool.QueueUserWorkItem(executeDelegate);
                }
            }
        }

        private void Device_DeviceDisposed(Device<TDeviceAddress> device)
        {
            #region Contracts

            if (device == null) throw new ArgumentNullException();

            #endregion

            // Detach
            this.DetachDevice(device);
        }


        // Events
        public event Action<Device<TDeviceAddress>> DeviceArrived;
        private void OnDeviceArrived(Device<TDeviceAddress> device)
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

        public event Action<Device<TDeviceAddress>> DeviceDeparted;
        private void OnDeviceDeparted(Device<TDeviceAddress> device)
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
