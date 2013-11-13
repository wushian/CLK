using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Communication
{
    public interface IDeviceCommandStrategy<TDeviceAddress>
        where TDeviceAddress : DeviceAddress
    {
        
    }

    public interface IDeviceCommandStrategy<TDeviceAddress, TRequest, TResponse> : IDeviceCommandStrategy<TDeviceAddress>
        where TDeviceAddress : DeviceAddress
        where TRequest : class
        where TResponse : class
    {
        // Properties
        int ExpireMillisecond { get; }

        int RetryCount { get; }


        // Methods
        void ApplyExecute(ExecuteCommandArrivedEventArgs<TDeviceAddress, TRequest> eventArgs);

        void ApplyExecute(ExecuteCommandCompletedEventArgs<TDeviceAddress, TRequest, TResponse> eventArgs);
    }
}
