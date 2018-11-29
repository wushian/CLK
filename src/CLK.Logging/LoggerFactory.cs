using System;
using System.Collections.Generic;
using System.Text;

namespace CLK.Logging
{
    public interface LoggerFactory
    {
        // Methods
        Logger<TCategory> Create<TCategory>();
    }
}
