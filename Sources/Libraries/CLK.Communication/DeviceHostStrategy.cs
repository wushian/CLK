using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Communication
{
    public interface IDeviceHostStrategy<TAddress> 
        where TAddress : DeviceAddress
    {        
        // Methods        
        void Start();

        void Stop();

        IDeviceStrategy<TAddress> GetDeviceStrategy(TAddress localAddress, TAddress remoteAddress);


        // Events
        event Action<TAddress, TAddress> DeviceArrived;

        event Action<TAddress, TAddress> DeviceDeparted;
    }
}
