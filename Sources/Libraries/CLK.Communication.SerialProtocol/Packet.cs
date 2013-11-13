using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Communication.SerialProtocol
{
    public sealed class Packet<TDeviceAddress>
        where TDeviceAddress : DeviceAddress
    {
        // Constructors
        public Packet(TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress, byte[] contents)
        {
            #region Contracts

            if (localDeviceAddress == null) throw new ArgumentNullException();
            if (remoteDeviceAddress == null) throw new ArgumentNullException();
            if (contents == null) throw new ArgumentNullException();

            #endregion

            // Arguments                
            this.LocalDeviceAddress = localDeviceAddress;
            this.RemoteDeviceAddress = remoteDeviceAddress;
            this.Contents = contents;
        }


        // Properties        
        public TDeviceAddress LocalDeviceAddress { get; private set; }

        public TDeviceAddress RemoteDeviceAddress { get; private set; }

        public byte[] Contents { get; private set; }
    }
}
