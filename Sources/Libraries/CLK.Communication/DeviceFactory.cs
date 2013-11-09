using CLK.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Communication
{
    public sealed class DeviceFactory<TDeviceAddress> 
        where TDeviceAddress : DeviceAddress
    {
        // Fields
        private readonly IDeviceCommandFactory<TDeviceAddress> _commandFactory = null;

        private readonly Func<PortableTimer> _createTimerDelegate = null;
        

        // Constructors
        public DeviceFactory(IDeviceCommandFactory<TDeviceAddress> commandFactory, Func<PortableTimer> createTimerDelegate)
        {
            #region Contracts
            
            if (commandFactory == null) throw new ArgumentNullException();
            if (createTimerDelegate == null) throw new ArgumentNullException();   

            #endregion

            // Arguments 
            _commandFactory = commandFactory;
            _createTimerDelegate = createTimerDelegate;
        }


        // Methods
        public Device<TDeviceAddress> CreateDevice(TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress)
        {
            #region Contracts

            if (localDeviceAddress == null) throw new ArgumentNullException();
            if (remoteDeviceAddress == null) throw new ArgumentNullException();

            #endregion

            // Timer
            PortableTimer operateTimer = _createTimerDelegate();
            if (operateTimer == null) throw new InvalidOperationException();

            // Device
            Device<TDeviceAddress> device = new Device<TDeviceAddress>(localDeviceAddress, remoteDeviceAddress, _commandFactory);

            // Open
            device.Open();

            // Return
            return device;
        }
    }
}
