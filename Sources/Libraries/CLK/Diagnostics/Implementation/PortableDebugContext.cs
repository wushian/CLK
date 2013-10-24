using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Diagnostics
{
    public sealed class PortableDebugContext : DebugContext
    {
        // Constructors
        public PortableDebugContext()
        {
            // DebugProvider
            IDebugProvider debugProvider = new PortableDebugProvider();

            // Initialize
            this.Initialize(debugProvider);
        }
    }
}
