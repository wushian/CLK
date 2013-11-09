using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Communication
{
    public interface IDeviceCommandFactory<TDeviceAddress>
        where TDeviceAddress : DeviceAddress
    {
        // Methods
        IEnumerable<DeviceCommand> CreateAll(TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress);
    }
}
