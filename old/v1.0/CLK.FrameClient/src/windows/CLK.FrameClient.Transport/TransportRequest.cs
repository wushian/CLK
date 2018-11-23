using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.FrameClient.Transport
{
    public class TransportRequest
    {
        // Constructors
        public TransportRequest(String uri)
        {
            #region Contracts

            if (string.IsNullOrEmpty(uri) == true) throw new ArgumentNullException();

            #endregion

            // Default
            this.Uri = uri;
            this.Headers = new Dictionary<string, string>();
            this.Content = null;
        }


        // Properties
        public string Uri { get; }

        public Dictionary<string, string> Headers { get; }

        public Object Content { get; set; }
    }
}
