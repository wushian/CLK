using CLK.Promises;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.FrameClient.Transport
{
    public class TransportHandler : TransportProvider
    {
        // Fields    
        private TransportProvider _provider = null;


        // Constructor
        public TransportHandler()
        {
           
        }
    
        public void Dispose()
        {
            // Dispose
            if (_provider != null)
            {
                _provider.Dispose();
            }
        }


        // Methods
        public void Attach(TransportProvider provider)
        {
            #region Contracts

            if (provider == null) throw new ArgumentNullException();

            #endregion

            // Require
            if (_provider != null) throw new InvalidOperationException();
            
            // Provider
            _provider = provider;
        }

        public virtual ResultPromise<TransportResponse> Send(TransportRequest request)
        {
            #region Contracts

            if (request == null) throw new ArgumentNullException();

            #endregion
            
            // Require
            if (_provider == null) throw new InvalidOperationException();

            // Send
            return _provider.Send(request);
        }
    }
}
