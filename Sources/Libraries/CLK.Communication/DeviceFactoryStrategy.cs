using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Communication
{
    public interface IDeviceFactoryStrategy<TAddress> 
        where TAddress : DeviceAddress
    {        
        // Methods    
        IDeviceStrategy<TAddress> GetDeviceStrategy(TAddress localAddress, TAddress remoteAddress);
    }
}
