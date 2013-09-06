using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Net.Sockets
{
    internal sealed class RouteUdpPacket
    {
        // Constructors
        public RouteUdpPacket(IPEndPoint endPoint, byte[] contents)
        {
            #region Contracts

            if (endPoint == null) throw new ArgumentNullException();
            if (contents == null) throw new ArgumentNullException();

            #endregion

            // Arguments
            this.EndPoint = endPoint;
            this.Contents = contents;
        }


        // Properties
        public IPEndPoint EndPoint { get; private set; }

        public byte[] Contents { get; private set; }
    }
}
