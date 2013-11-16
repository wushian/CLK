using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CLK.Network.Sockets
{
    internal sealed class RouteUdpClient : IDisposable
    {
        // Fields
        private readonly UdpClient _udpClient = null;

        private readonly Thread _receiveThread = null;

        private Exception _receiveException = null;


        // Constructors
        public RouteUdpClient(IPEndPoint localEndPoint)
        {
            #region Contracts

            if (localEndPoint == null) throw new ArgumentNullException();

            #endregion

            // UdpClient
            _udpClient = new UdpClient(localEndPoint);
            _udpClient.Client.IOControl(-1744830452, new byte[] { 0, 0, 0, 0 }, null); // Fix : UDP Socket 10054 Error Code
            _udpClient.Ttl = 255; // Fix : UDP Socket 10052 Error Code

            // ReceiveThread
            _receiveThread = new Thread(this.Receive);
            _receiveThread.IsBackground = false;
            _receiveThread.Start();
        }

        public void Dispose()
        {
            // Close
            _udpClient.Close();

            // Exception
            if (_receiveException != null)
            {
                _receiveException = new InvalidOperationException();
            }
        }


        // Properties
        public Exception ReceiveException
        {
            get
            {
                return _receiveException;
            }
        }


        // Methods            
        public int Send(byte[] contents, int count, IPEndPoint remoteEndPoint)
        {
            #region Contracts

            if (contents == null) throw new ArgumentNullException();
            if (remoteEndPoint == null) throw new ArgumentNullException();

            #endregion

            // Send
            return _udpClient.Send(contents, contents.Length, remoteEndPoint);
        }

        private void Receive()
        {
            try
            {
                while (true)
                {
                    // Receive
                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] content = _udpClient.Receive(ref remoteEndPoint);
                    if (content == null) continue;
                                        
                    // Notify
                    this.OnPacketReceived(new RouteUdpPacket(remoteEndPoint, content));
                }
            }
            catch (Exception ex)
            {
                // Exception
                _receiveException = ex;

                // Notify
                this.OnExceptionReceived(ex);
            }
        }       

        
        // Events
        public event Action<RouteUdpPacket> PacketReceived;
        private void OnPacketReceived(RouteUdpPacket packet)
        {
            #region Contracts

            if (packet == null) throw new ArgumentNullException();

            #endregion

            var handler = this.PacketReceived;
            if (handler != null)
            {
                handler(packet);
            }
        }

        public event Action<Exception> ExceptionReceived;
        private void OnExceptionReceived(Exception exception)
        {
            #region Contracts

            if (exception == null) throw new ArgumentNullException();

            #endregion

            var handler = this.ExceptionReceived;
            if (handler != null)
            {
                handler(exception);
            }
        }        
    }
}
