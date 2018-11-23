using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLK.Promises;
using System.Net.Http;

namespace CLK.FrameClient.Transport.Http
{
    public class HttpTransportProvider : TransportProvider
    {
        // Fields    
        private HttpClient _httpClient = null;


        // Constructor
        public HttpTransportProvider()
        {
            // Default
            _httpClient = new HttpClient();
        }

        public void Dispose()
        {
            // Dispose
            _httpClient.Dispose();
        }


        // Methods
        public ResultPromise<TransportResponse> Send(TransportRequest request)
        {
            #region Contracts

            if (request == null) throw new ArgumentNullException();

            #endregion

            // Send
            throw new NotImplementedException();
        }
    }
}
