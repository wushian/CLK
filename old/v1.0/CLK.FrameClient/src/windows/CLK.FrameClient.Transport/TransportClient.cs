using CLK.Promises;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.FrameClient.Transport
{
    public class TransportClient
    {
        // Fields    
        private TransportProvider _provider = null;


        // Constructor
        public TransportClient(TransportProvider provider)
        {
            #region Contracts

            if (provider == null) throw new ArgumentNullException();

            #endregion

            // Default
            _provider = provider;
        }

        public void Dispose()
        {
            // Dispose
            _provider.Dispose();
        }


        // Methods
        public void Attach(TransportHandler handler)
        {
            #region Contracts

            if (handler == null) throw new ArgumentNullException();

            #endregion

            // Attach
            handler.Attach(_provider);

            // Provider
            _provider = handler;
        }

        public ResultPromise<TransportResponse> Send(TransportRequest request)
        {
            #region Contracts

            if (request == null) throw new ArgumentNullException();

            #endregion

            // Send
            return _provider.Send(request);
        }
    }
}
