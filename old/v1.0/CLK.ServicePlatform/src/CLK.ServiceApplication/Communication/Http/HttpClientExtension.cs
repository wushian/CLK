using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceApplication.Communication.Http
{
    public static class HttpClientExtension
    {
        // Methods
        public static HttpClient CreateHttpClient(this CommunicationContext context)
        {
            // Client
            var client = new HttpClient();

            // Attach
            context.Attach(client);

            // Return
            return client;
        }
    }
}
