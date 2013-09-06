using CLK.Collections.Concurrent;
using CLK.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Projection
{
    public abstract class DeviceManager<TDevice>
       where TDevice : Device
    {
        // Fields     
        private readonly PortableStarterStoperLock _operateLock = new PortableStarterStoperLock();

        private readonly DeviceCollection<TDevice> _deviceCollection = null;

        private readonly IEnumerable<DeviceExplorer<TDevice>> _deviceExplorerCollection = null;


        // Constructor 
        public DeviceManager(IEnumerable<DeviceExplorer<TDevice>> deviceExplorerCollection)
        {
            #region Contracts

            if (deviceExplorerCollection == null) throw new ArgumentNullException();

            #endregion

            // DeviceCollection
            _deviceCollection = new DeviceCollection<TDevice>(this.EqualDevice);

            // DeviceExplorerCollection
            _deviceExplorerCollection = deviceExplorerCollection.ToArray();
        }


        // Properties   
        public IEnumerable<TDevice> DeviceCollection
        {
            get
            {
                return _deviceCollection;
            }
        }
        

        // Methods  
        public virtual void Start()
        {
            // EnterStartLock
            if (_operateLock.EnterStartLock() == false) return;

            // Start
            try
            {
                // DeviceCollection
                foreach (TDevice device in _deviceCollection.ToArray())
                {
                    this.AttachDevice(device);
                }

                // DeviceExplorerCollection
                foreach (DeviceExplorer<TDevice> deviceExplorer in _deviceExplorerCollection)
                {
                    deviceExplorer.DeviceArrived += this.DeviceExplorer_DeviceArrived;
                    deviceExplorer.DeviceDeparted += this.DeviceExplorer_DeviceDeparted;
                    deviceExplorer.Start();
                }
            }
            finally
            {
                // ExitStartLock
                _operateLock.ExitStartLock();
            }
        }

        public virtual void Stop()
        {
            // EnterStopLock
            if (_operateLock.EnterStopLock() == false) return;

            // Stop
            try
            {
                // DeviceExplorerCollection
                foreach (DeviceExplorer<TDevice> deviceExplorer in _deviceExplorerCollection.Reverse().ToArray())
                {
                    deviceExplorer.Stop();
                    deviceExplorer.DeviceArrived -= this.DeviceExplorer_DeviceArrived;
                    deviceExplorer.DeviceDeparted -= this.DeviceExplorer_DeviceDeparted;
                }

                // DeviceCollection
                foreach (TDevice device in _deviceCollection.ToArray())
                {
                    this.DetachDevice(device);
                }
            }
            finally
            {
                // ExitStopLock
                _operateLock.ExitStopLock();
            }            
        }


        private void AttachDevice(TDevice device)
        {
            #region Contracts

            if (device == null) throw new ArgumentNullException();

            #endregion

            // Attach
            device = _deviceCollection.Attach(device);
            if (device == null) return;

            // Notify
            this.OnDeviceArrived(device);

            // Start            
            device.Start();
        }

        private void DetachDevice(TDevice device)
        {
            #region Contracts

            if (device == null) throw new ArgumentNullException();

            #endregion

            // Attach
            device = _deviceCollection.Detach(device);
            if (device == null) return;

            // Stop            
            device.Stop();

            // Notify
            this.OnDeviceDeparted(device);
        }

        protected abstract bool EqualDevice(TDevice deviceA, TDevice deviceB);


        // Handlers
        private void DeviceExplorer_DeviceArrived(TDevice device)
        {
            #region Contracts

            if (device == null) throw new ArgumentNullException();

            #endregion

            // Attach
            this.AttachDevice(device);
        }  

        private void DeviceExplorer_DeviceDeparted(TDevice device)
        {
            #region Contracts

            if (device == null) throw new ArgumentNullException();

            #endregion

            // Detach
            this.DetachDevice(device);
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
