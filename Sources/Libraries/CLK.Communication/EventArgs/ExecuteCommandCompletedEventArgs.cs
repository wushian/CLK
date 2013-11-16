using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CLK.Communication
{
    public sealed class ExecuteCommandCompletedEventArgs<TAddress, TRequest, TResponse> : AsyncCompletedEventArgs
        where TAddress : DeviceAddress
        where TRequest : class
        where TResponse : class
    {
        // Constructors
        internal ExecuteCommandCompletedEventArgs(Guid taskId, TAddress localAddress, TAddress remoteAddress, 
                                                TRequest request, TResponse response) : base(null, false, null)
        {
            #region Contracts

            if (taskId == Guid.Empty) throw new ArgumentException();
            if (localAddress == null) throw new ArgumentNullException();
            if (remoteAddress == null) throw new ArgumentNullException();          
            if (request == null) throw new ArgumentNullException();
            if (response == null) throw new ArgumentNullException();

            #endregion

            // Arguments
            this.TaskId = taskId;
            this.LocalAddress = localAddress;
            this.RemoteAddress = remoteAddress;           
            this.Request = request;
            this.Response = response;
        }

        internal ExecuteCommandCompletedEventArgs(Guid taskId, TAddress localAddress, TAddress remoteAddress, 
                                                TRequest request, Exception error) : base(error, true, null)
        {
            #region Contracts
                       
            if (taskId == Guid.Empty) throw new ArgumentException();
            if (localAddress == null) throw new ArgumentNullException();
            if (remoteAddress == null) throw new ArgumentNullException();    
            if (request == null) throw new ArgumentNullException();
            if (error == null) throw new ArgumentNullException();

            #endregion

            // Arguments
            this.TaskId = taskId;
            this.LocalAddress = localAddress;
            this.RemoteAddress = remoteAddress;   
            this.Request = request;
            this.Response = null;
        }


        // Properties              
        public Guid TaskId { get; private set; }

        public TAddress LocalAddress { get; private set; }

        public TAddress RemoteAddress { get; private set; }

        public TRequest Request { get; private set; }

        public TResponse Response { get; private set; }
    }
}