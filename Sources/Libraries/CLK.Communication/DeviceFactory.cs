using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Communication
{
    public abstract class DeviceFactory<TDevice, TAddress>
        where TDevice : Device<TAddress>
        where TAddress : DeviceAddress
    {
        // Fields
        private readonly IDeviceFactoryStrategy<TAddress> _deviceFactoryStrategy = null;


        // Constructors
        public DeviceFactory(IDeviceFactoryStrategy<TAddress> deviceFactoryStrategy)
        {
            #region Contracts

            if (deviceFactoryStrategy == null) throw new ArgumentNullException();

            #endregion

            // Arguments
            _deviceFactoryStrategy = deviceFactoryStrategy;
        }


        // Methods
        protected TDevice CreateDevice(TAddress localAddress, TAddress remoteAddress)
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
            IDeviceStrategy<TAddress> deviceStrategy = _deviceFactoryStrategy.GetDeviceStrategy(localAddress, remoteAddress);
            if (device == null) throw new InvalidOperationException();

            // Initialize
            device.Initialize(deviceStrategy);

            // Return
            return device;
        }

        protected abstract TDevice CreateDevice();
    }
}
