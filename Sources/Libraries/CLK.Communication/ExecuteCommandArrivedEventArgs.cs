using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CLK.Communication
{
    public sealed class ExecuteCommandArrivedEventArgs<TDeviceAddress, TRequest> : EventArgs
        where TDeviceAddress : DeviceAddress
        where TRequest : class
    {
        // Constructors
        internal ExecuteCommandArrivedEventArgs(Guid taskId, TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress, TRequest request)
        {
            #region Contracts

            if (taskId == Guid.Empty) throw new ArgumentException();
            if (localDeviceAddress == null) throw new ArgumentNullException();
            if (remoteDeviceAddress == null) throw new ArgumentNullException();          
            if (request == null) throw new ArgumentNullException();

            #endregion

            // Arguments
            this.TaskId = taskId;
            this.LocalDeviceAddress = localDeviceAddress;
            this.RemoteDeviceAddress = remoteDeviceAddress;           
            this.Request = request;
        }


        // Properties       
        public Guid TaskId { get; private set; }

        public TDeviceAddress LocalDeviceAddress { get; private set; }

        public TDeviceAddress RemoteDeviceAddress { get; private set; }

        public TRequest Request { get; private set; }
    }
}