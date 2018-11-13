using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.FrameClient.Transport
{
    public class TransportResponse
    {
        // Constructors
        public TransportResponse()
        {
            // Default
            this.Headers = new Dictionary<string, string>();
            this.Content = null;
        }


        // Properties
        public Dictionary<string, string> Headers { get; }

        public Object Content { get; set; }
    }
}
