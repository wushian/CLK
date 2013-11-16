using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Communication
{
    public interface IDeviceCommandStrategy<TAddress>
        where TAddress : DeviceAddress
    {
        
    }

    public interface IDeviceCommandStrategy<TAddress, TRequest, TResponse> : IDeviceCommandStrategy<TAddress>
        where TAddress : DeviceAddress
        where TRequest : class
        where TResponse : class
    {
        // Properties
        int ExpireMillisecond { get; }

        int RetryCount { get; }


        // Methods
        void ApplyExecute(ExecuteCommandArrivedEventArgs<TAddress, TRequest> eventArgs);

        void ApplyExecute(ExecuteCommandCompletedEventArgs<TAddress, TRequest, TResponse> eventArgs);
    }
}
