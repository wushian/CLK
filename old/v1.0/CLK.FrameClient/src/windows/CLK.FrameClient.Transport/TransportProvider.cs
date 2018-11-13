using CLK.Promises;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.FrameClient.Transport
{
    public interface TransportProvider : IDisposable
    {
        // Methods
        ResultPromise<TransportResponse> Send(TransportRequest request);
    }
}
