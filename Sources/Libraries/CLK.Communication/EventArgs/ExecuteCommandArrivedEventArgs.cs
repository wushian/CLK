using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CLK.Communication
{
    public sealed class ExecuteCommandArrivedEventArgs<TAddress, TRequest> : EventArgs
        where TAddress : DeviceAddress
        where TRequest : class
    {
        // Constructors
        internal ExecuteCommandArrivedEventArgs(Guid taskId, TAddress localAddress, TAddress remoteAddress, TRequest request)
        {
            #region Contracts

            if (taskId == Guid.Empty) throw new ArgumentException();
            if (localAddress == null) throw new ArgumentNullException();
            if (remoteAddress == null) throw new ArgumentNullException();          
            if (request == null) throw new ArgumentNullException();

            #endregion

            // Arguments
            this.TaskId = taskId;
            this.LocalAddress = localAddress;
            this.RemoteAddress = remoteAddress;           
            this.Request = request;
        }


        // Properties       
        public Guid TaskId { get; private set; }

        public TAddress LocalAddress { get; private set; }

        public TAddress RemoteAddress { get; private set; }

        public TRequest Request { get; private set; }
    }
}