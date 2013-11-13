using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Communication
{
    public interface IDeviceStrategy<TDeviceAddress> 
        where TDeviceAddress : DeviceAddress
    {
        // Methods
        void Start();

        void Stop();


        IEnumerable<DeviceCommand<TDeviceAddress>> GetAllCommand(TDeviceAddress LocalDeviceAddress, TDeviceAddress remoteDeviceAddress);

        IEnumerable<IDeviceCommandStrategy<TDeviceAddress>> GetAllCommandStrategy();


        // Events
        event Action<TDeviceAddress, TDeviceAddress> DeviceArrived;

        event Action<TDeviceAddress, TDeviceAddress> DeviceDeparted;
    }
}
