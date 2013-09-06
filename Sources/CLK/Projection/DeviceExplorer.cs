using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Projection
{
    public abstract class DeviceExplorer<TDevice>
       where TDevice : Device
    {
        // Methods
        internal protected abstract void Start();

        internal protected abstract void Stop();


        // Events
        internal event Action<TDevice> DeviceArrived;
        protected void OnDeviceArrived(TDevice device)
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

        internal event Action<TDevice> DeviceDeparted;
        protected void OnDeviceDeparted(TDevice device)
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
