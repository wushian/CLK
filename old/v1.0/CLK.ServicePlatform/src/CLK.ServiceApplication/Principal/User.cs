using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceApplication.Principal
{
    public interface User
    {
        // Properties
        bool IsAuthenticated { get; }

        string Name { get; }
    }
}
