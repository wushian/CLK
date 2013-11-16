using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Communication
{
    public interface IDeviceActiveCommandStrategy<TAddress, TRequest, TResponse> : IDeviceCommandStrategy<TAddress, TRequest, TResponse>
        where TAddress : DeviceAddress
        where TRequest : class
        where TResponse : class
    {
        // Events
        event Action<Guid , TAddress , TAddress , TRequest, TResponse> ExecuteSucceedCompleted;

        event Action<Guid , TAddress , TAddress , TRequest, Exception> ExecuteFailCompleted;
    }
}
