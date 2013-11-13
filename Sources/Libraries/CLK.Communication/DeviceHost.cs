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
    public abstract class DeviceHost<TDevice, TDeviceAddress>
        where TDevice : Device<TDeviceAddress>
        where TDeviceAddress : DeviceAddress
    {
        // Fields
        private readonly object _syncRoot = new object();

        private readonly PortableStarterStoperLock _operateLock = new PortableStarterStoperLock();

        private readonly List<TDevice> _deviceCollection = new List<TDevice>();

        private readonly IDeviceStrategy<TDeviceAddress> _deviceStrategy = null;

        private readonly PortableTimer _operateTimer = null;


        // Constructors
        public DeviceHost(IDeviceStrategy<TDeviceAddress> deviceStrategy, PortableTimer operateTimer)
        {
            #region Contracts

            if (deviceStrategy == null) throw new ArgumentNullException();
            if (operateTimer == null) throw new ArgumentNullException();

            #endregion

            // Arguments 
            _deviceStrategy = deviceStrategy;
            _operateTimer = operateTimer;
        }
        

        // Methods
        public void Open()
        {
            // EnterStartLock
            if (_operateLock.EnterStartLock() == false) return;

            // Open
            try
            {
                // Device
                IEnumerable<TDevice> deviceCollection = null;
                lock (_syncRoot) { deviceCollection = _deviceCollection.ToArray(); }
                foreach (TDevice device in deviceCollection)
                {
                    device.Dispose();
                }

                // Timer
                _operateTimer.Ticked += this.Timer_Ticked;
                _operateTimer.Start();

                // Strategy
                _deviceStrategy.DeviceArrived += this.DeviceStrategy_DeviceArrived;
                _deviceStrategy.DeviceDeparted += this.DeviceStrategy_DeviceDeparted;
                _deviceStrategy.Start();
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
                // Strategy
                _deviceStrategy.Stop();
                _deviceStrategy.DeviceArrived -= this.DeviceStrategy_DeviceArrived;
                _deviceStrategy.DeviceDeparted -= this.DeviceStrategy_DeviceDeparted;

                // Timer
                _operateTimer.Stop();
                _operateTimer.Ticked -= this.Timer_Ticked;

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


        private void AttachDevice(TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress)
        {
            #region Contracts

            if (localDeviceAddress == null) throw new ArgumentNullException();
            if (remoteDeviceAddress == null) throw new ArgumentNullException();

            #endregion

            // Result
            TDevice device = null;

            // Attach
            lock (_syncRoot)
            {
                // Search 
                foreach (TDevice existDevice in _deviceCollection)
                {
                    if (existDevice.LocalDeviceAddress.EqualAddress(localDeviceAddress) == true)
                    {
                        if (existDevice.RemoteDeviceAddress.EqualAddress(remoteDeviceAddress) == true)
                        {
                            device = existDevice;
                            break;
                        }
                    }
                }
                if (device != null) return;

                // Create
                device = this.CreateDevice(localDeviceAddress, remoteDeviceAddress, _deviceStrategy);
                if (device == null) return;

                // Add
                _deviceCollection.Add(device);

                // Events
                device.DeviceDisposed += this.DeviceInstance_DeviceDisposed;
            }

            // Open
            device.Open();

            // Notify
            this.OnDeviceArrived(device);
        }

        private void DetachDevice(TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress)
        {
            #region Contracts

            if (localDeviceAddress == null) throw new ArgumentNullException();
            if (remoteDeviceAddress == null) throw new ArgumentNullException();

            #endregion

            // Result
            TDevice device = null;

            // Detach
            lock (_syncRoot)
            {
                // Search 
                foreach (TDevice existDevice in _deviceCollection)
                {
                    if (existDevice.LocalDeviceAddress.EqualAddress(localDeviceAddress) == true)
                    {
                        if (existDevice.RemoteDeviceAddress.EqualAddress(remoteDeviceAddress) == true)
                        {
                            device = existDevice;
                            break;
                        }
                    }
                }
                if (device == null) return;
                                
                // Remove
                _deviceCollection.Remove(device);

                // Events
                device.DeviceDisposed -= this.DeviceInstance_DeviceDisposed;  
            }

            // Close
            device.Close();

            // Notify
            this.OnDeviceDeparted(device);            
        }

        private TDevice CreateDevice(TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress, IDeviceStrategy<TDeviceAddress> deviceStrategy)
        {
            #region Contracts

            if (localDeviceAddress == null) throw new ArgumentNullException();
            if (remoteDeviceAddress == null) throw new ArgumentNullException();
            if (deviceStrategy == null) throw new ArgumentNullException();

            #endregion

            // Command
            var commandCollection = deviceStrategy.GetAllCommand(localDeviceAddress, remoteDeviceAddress);
            if (commandCollection == null) throw new InvalidOperationException();

            // CommandStrategy
            var commandStrategyCollection = deviceStrategy.GetAllCommandStrategy();
            if (commandStrategyCollection == null) throw new InvalidOperationException();

            // Initialize
            foreach (DeviceCommand<TDeviceAddress> command in commandCollection)
            {
                foreach (IDeviceCommandStrategy<TDeviceAddress> commandStrategy in commandStrategyCollection)
                {
                    command.Initialize(commandStrategy);
                }
            }

            // Device
            TDevice device = this.CreateDevice(localDeviceAddress, remoteDeviceAddress, commandCollection);
            if (device == null) throw new InvalidOperationException();

            // Return
            return device;
        }

        protected abstract TDevice CreateDevice(TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress, IEnumerable<DeviceCommand<TDeviceAddress>> _commandCollection);
                

        public TDevice GetDevice(Func<TDevice, bool> predicate)
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
        

        // Handlers
        private void Timer_Ticked(object sender, EventArgs e)
        {
            // NowTicks
            long nowTicks = DateTime.Now.Ticks;

            // Device
            lock (_syncRoot)
            {
                foreach (TDevice device in _deviceCollection)
                {
                    WaitCallback executeDelegate = delegate(object state)
                    {
                        try
                        {
                            device.ApplyTimeTicked(nowTicks);
                        }
                        catch (Exception ex)
                        {
                            DebugContext.Current.Fail(string.Format("Action:{0}, State:{1}, Message:{2}", "ApplyTimeTicked", "Exception", ex.Message));
                        }
                    };
                    ThreadPool.QueueUserWorkItem(executeDelegate);
                }
            }
        }

        private void DeviceStrategy_DeviceArrived(TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress)
        {
            #region Contracts

            if (localDeviceAddress == null) throw new ArgumentNullException();
            if (remoteDeviceAddress == null) throw new ArgumentNullException();

            #endregion

            // Attach
            this.AttachDevice(localDeviceAddress, remoteDeviceAddress);
        }

        private void DeviceStrategy_DeviceDeparted(TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress)
        {
            #region Contracts

            if (localDeviceAddress == null) throw new ArgumentNullException();
            if (remoteDeviceAddress == null) throw new ArgumentNullException();

            #endregion

            // Result
            TDevice device = null;

            // Search 
            Func<TDevice, bool> predicate = delegate(TDevice existDevice)
            {
                if (existDevice.LocalDeviceAddress.EqualAddress(localDeviceAddress) == true)
                {
                    if (existDevice.RemoteDeviceAddress.EqualAddress(remoteDeviceAddress) == true)
                    {
                        return true;
                    }
                }
                return false;
            };
            device = this.GetDevice(predicate);

            // Dispose
            if (device != null)
            {
                device.Dispose();
            }
        }

        private void DeviceInstance_DeviceDisposed(TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress)
        {
            #region Contracts

            if (localDeviceAddress == null) throw new ArgumentNullException();
            if (remoteDeviceAddress == null) throw new ArgumentNullException();

            #endregion
                                    
            // Detach
            this.DetachDevice(localDeviceAddress, remoteDeviceAddress);
        }


        // Events
        public event Action<TDevice> DeviceArrived;
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

        public event Action<TDevice> DeviceDeparted;
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
