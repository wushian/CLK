using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Communication
{
    public abstract class DevicePassiveCommand<TDeviceAddress, TRequest, TResponse> : DeviceCommand<TDeviceAddress, TRequest, TResponse>
        where TDeviceAddress : DeviceAddress
        where TRequest : class
        where TResponse : class
    {
        // Constructors
        public DevicePassiveCommand(TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress, DeviceCommandPipeline commandPipeline) : base(localDeviceAddress, remoteDeviceAddress, commandPipeline) { }


        // Methods
        internal override void ApplyExecute(ExecuteCommandArrivedEventArgs<TDeviceAddress, TRequest> eventArgs)
        {
            #region Contracts

            if (eventArgs == null) throw new ArgumentNullException();

            #endregion

            // Nothing
                        
        }

        internal override void ApplyExecute(ExecuteCommandCompletedEventArgs<TDeviceAddress, TRequest, TResponse> eventArgs)
        {
            #region Contracts

            if (eventArgs == null) throw new ArgumentNullException();

            #endregion

            // Apply
            if (eventArgs.Cancelled == false)
            {
                this.ApplyExecute(eventArgs.TaskId, eventArgs.LocalDeviceAddress, eventArgs.RemoteDeviceAddress, eventArgs.Request, eventArgs.Response);
            }
            else
            {
                this.ApplyExecute(eventArgs.TaskId, eventArgs.LocalDeviceAddress, eventArgs.RemoteDeviceAddress, eventArgs.Request, eventArgs.Error);
            }
        }
        
        protected abstract void ApplyExecute(Guid taskId, TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress, TRequest request, TResponse response);

        protected abstract void ApplyExecute(Guid taskId, TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress, TRequest request, Exception error);
    }
}
