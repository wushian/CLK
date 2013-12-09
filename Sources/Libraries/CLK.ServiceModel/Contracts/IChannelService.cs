using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceModel
{
    [ServiceContract(SessionMode=SessionMode.Required)]
    public interface IChannelService
    {
        [OperationContract(IsOneWay = true)]
        void Heartbeat();
    }
}
