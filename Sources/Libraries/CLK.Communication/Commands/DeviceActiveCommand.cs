using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Communication
{
    public abstract class DeviceActiveCommand<TDeviceAddress, TRequest, TResponse> : DeviceCommand<TDeviceAddress, TRequest, TResponse>
        where TDeviceAddress : DeviceAddress
        where TRequest : class
        where TResponse : class
    {
        // Constructors
        public DeviceActiveCommand(TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress, DeviceCommandPipeline commandPipeline) : base(localDeviceAddress, remoteDeviceAddress, commandPipeline) { }


        // Methods
        internal override void ApplyExecute(ExecuteCommandArrivedEventArgs<TDeviceAddress, TRequest> eventArgs)
        {
            #region Contracts

            if (eventArgs == null) throw new ArgumentNullException();

            #endregion

            // Apply
            this.ApplyExecute(eventArgs.TaskId, eventArgs.LocalDeviceAddress, eventArgs.RemoteDeviceAddress, eventArgs.Request);
        }

        internal override void ApplyExecute(ExecuteCommandCompletedEventArgs<TDeviceAddress, TRequest, TResponse> eventArgs)
        {
            #region Contracts

            if (eventArgs == null) throw new ArgumentNullException();

            #endregion

            // Nothing

        }
        
        protected abstract void ApplyExecute(Guid taskId, TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress, TRequest request);
    }
}
