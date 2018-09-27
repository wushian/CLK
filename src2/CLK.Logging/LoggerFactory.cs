using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Logging
{
    public interface LoggerFactory : IDisposable
    {
        // Methods
        Logger Create(Type target);
    }
}
