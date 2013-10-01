using CLK.ComponentModel.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Net.Sockets
{
    internal sealed class RouteUdpClientPool : ResourcePool<IPEndPoint, RouteUdpClient>
    {
        // Methods  
        protected override RouteUdpClient CreateResource(IPEndPoint localEndPoint)
        {
            #region Contracts

            if (localEndPoint == null) throw new ArgumentNullException();

            #endregion

            // Return
            return new RouteUdpClient(localEndPoint);
        }

        protected override void ReleaseResource(RouteUdpClient udpClient)
        {
            #region Contracts

            if (udpClient == null) throw new ArgumentNullException();

            #endregion

            // Dispose
            udpClient.Dispose();
        }

        protected override bool CompareResourceKey(IPEndPoint localEndPointA, IPEndPoint localEndPointB)
        {
            #region Contracts

            if (localEndPointA == null) throw new ArgumentNullException();
            if (localEndPointB == null) throw new ArgumentNullException();

            #endregion

            // Return
            return localEndPointA.Equals(localEndPointB);
        }
    }
}
