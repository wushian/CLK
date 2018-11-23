using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.FrameClient.Identity
{
    public interface IdentityHandler
    {
        // Events  
        event Action Unauthorized;
    }
}
