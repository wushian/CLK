using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Communication
{
    public sealed class DeviceMessage<TDeviceAddress, TContents>
      where TDeviceAddress : DeviceAddress
    {
        // Constructors
        public DeviceMessage(TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress, TContents contents)
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

        public TContents Contents { get; private set; }
    }
}
