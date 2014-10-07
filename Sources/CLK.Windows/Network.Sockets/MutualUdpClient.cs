using CLK.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Network.Sockets
{
    public partial class MutualUdpClient
    {
        // Singleton
        private static RouteUdpClientPool _pool = null;

        private static RouteUdpClientPool Pool
        {
            get
            {
                if (_pool == null)
                {
                    _pool = new RouteUdpClientPool();
                }
                return _pool;
            }
        }
    }

    public partial class MutualUdpClient
    {
        // Fields
        private readonly object _syncRoot = new object();

        private readonly Guid _consumerId = Guid.NewGuid();

        private readonly IPEndPoint _localEndPoint = null;       

        private readonly RouteUdpClientPool _routeUdpClientPool = null;

        private readonly Func<IPEndPoint, byte[], bool> _acceptedReceiveFilter = null;


        private readonly RouteUdpClient _routeUdpClient = null;

        private readonly BlockingQueue<RouteUdpPacket> _receiveQueue = new BlockingQueue<RouteUdpPacket>();

        public bool _isRunning = false;
              

        // Constructor
        public MutualUdpClient(IPEndPoint localEndPoint, Func<IPEndPoint, byte[], bool> acceptedReceiveFilter = null) : this(localEndPoint, MutualUdpClient.Pool, acceptedReceiveFilter) { }

        private MutualUdpClient(IPEndPoint localEndPoint, RouteUdpClientPool routeUdpClientPool, Func<IPEndPoint, byte[], bool> acceptedReceiveFilter = null)
        {
            #region Contracts

            if (localEndPoint == null) throw new ArgumentNullException();
            if (routeUdpClientPool == null) throw new ArgumentNullException();

            #endregion

            // LocalEndPoint
            _localEndPoint = localEndPoint;

            // RouteUdpClientPool
            _routeUdpClientPool = routeUdpClientPool;

            // ReceiveFilter
            _acceptedReceiveFilter = acceptedReceiveFilter;

            // RouteUdpClient
            _routeUdpClient = _routeUdpClientPool.Create(_consumerId, _localEndPoint);
            if (_routeUdpClient == null) throw new InvalidOperationException("Create RouteUdpClient failed.");
            _routeUdpClient.ExceptionReceived += this.RouteUdpClient_ExceptionReceived;
            _routeUdpClient.PacketReceived += this.RouteUdpClient_PacketReceived;

            // Flag
            _isRunning = true;
        }
        

        // Methods            
        public void Close()
        {
            // Sync
            lock (_syncRoot)
            {
                // Require
                if (_isRunning == false) return;

                // Flag
                _isRunning = false;                
            }            

            // RouteUdpClient
            _routeUdpClient.ExceptionReceived -= this.RouteUdpClient_ExceptionReceived;
            _routeUdpClient.PacketReceived -= this.RouteUdpClient_PacketReceived;            

            // RouteUdpClientPool
            _routeUdpClientPool.Release(_consumerId, _localEndPoint);

            // Queue
            _receiveQueue.Release();
        }

        public byte[] Receive(ref IPEndPoint remoteEndPoint)
        {
            #region Contracts

            if (remoteEndPoint == null) throw new ArgumentNullException();

            #endregion

            while (true)
            {
                // Sync
                lock (_syncRoot)
                {
                    // Flag
                    if (_isRunning == false) return null;

                    // ReceiveException
                    if (_routeUdpClient.ReceiveException != null) throw _routeUdpClient.ReceiveException;
                }

                // ReceivePacket
                RouteUdpPacket packet = _receiveQueue.Dequeue();
                if (packet == null) continue;

                // Return
                remoteEndPoint.Address = packet.EndPoint.Address;
                remoteEndPoint.Port = packet.EndPoint.Port;
                return packet.Contents;
            }
        }

        public int Send(byte[] contents, int count, IPEndPoint remoteEndPoint)
        {
            #region Contracts

            if (contents == null) throw new ArgumentNullException();
            if (remoteEndPoint == null) throw new ArgumentNullException();

            #endregion
                        
            // RouteUdpClient
            return _routeUdpClient.Send(contents, count, remoteEndPoint);
        }


        // Handlers
        private void RouteUdpClient_PacketReceived(RouteUdpPacket packet)
        {
            #region Contracts

            if (packet == null) throw new ArgumentNullException();

            #endregion

            // Sync
            lock (_syncRoot)
            {
                // Require
                if (_isRunning == false) return;
            }

            // Filter
            if (_acceptedReceiveFilter != null)
            {
                if (_acceptedReceiveFilter(packet.EndPoint, packet.Contents) == false)
                {
                    return;
                }
            }

            // Enqueue
            _receiveQueue.Enqueue(packet);
        }

        private void RouteUdpClient_ExceptionReceived(Exception exception)
        {
            #region Contracts

            if (exception == null) throw new ArgumentNullException();

            #endregion

            // Sync
            lock (_syncRoot)
            {
                // Require
                if (_isRunning == false) return;
            }

            // Release
            _receiveQueue.Release();
        }   
    }    
}
