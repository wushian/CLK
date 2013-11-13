using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Communication.SerialProtocol
{
    public interface IPacketAccess<TDeviceAddress>
        where TDeviceAddress : DeviceAddress
    {
        // Methods
        void Start();

        void Stop();
        
        void TransmitPacket(Packet<TDeviceAddress> packet);


        // Events
        event Action<Packet<TDeviceAddress>> PacketReceived;
    }
}
