using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.FrameClient.Transport
{
    public class UnauthorizedException : Exception
    {
        // Constructors
        public UnauthorizedException() : base("Unauthorized") { }
    }
}
