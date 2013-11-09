using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CLK.Communication
{
    public sealed class ExecuteCommandCompletedEventArgs<TDeviceAddress, TRequest, TResponse> : AsyncCompletedEventArgs
        where TDeviceAddress : DeviceAddress
        where TRequest : class
        where TResponse : class
    {
        // Constructors
        internal ExecuteCommandCompletedEventArgs(Guid taskId, TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress, 
                                                TRequest request, TResponse response) : base(null, false, null)
        {
            #region Contracts

            if (taskId == Guid.Empty) throw new ArgumentException();
            if (localDeviceAddress == null) throw new ArgumentNullException();
            if (remoteDeviceAddress == null) throw new ArgumentNullException();          
            if (request == null) throw new ArgumentNullException();
            if (response == null) throw new ArgumentNullException();

            #endregion

            // Arguments
            this.TaskId = taskId;
            this.LocalDeviceAddress = localDeviceAddress;
            this.RemoteDeviceAddress = remoteDeviceAddress;           
            this.Request = request;
            this.Response = response;
        }

        internal ExecuteCommandCompletedEventArgs(Guid taskId, TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress, 
                                                TRequest request, Exception error) : base(error, true, null)
        {
            #region Contracts
                       
            if (taskId == Guid.Empty) throw new ArgumentException();
            if (localDeviceAddress == null) throw new ArgumentNullException();
            if (remoteDeviceAddress == null) throw new ArgumentNullException();    
            if (request == null) throw new ArgumentNullException();
            if (error == null) throw new ArgumentNullException();

            #endregion

            // Arguments
            this.TaskId = taskId;
            this.LocalDeviceAddress = localDeviceAddress;
            this.RemoteDeviceAddress = remoteDeviceAddress;   
            this.Request = request;
            this.Response = null;
        }


        // Properties              
        public Guid TaskId { get; private set; }

        public TDeviceAddress LocalDeviceAddress { get; private set; }

        public TDeviceAddress RemoteDeviceAddress { get; private set; }

        public TRequest Request { get; private set; }

        public TResponse Response { get; private set; }
    }
}