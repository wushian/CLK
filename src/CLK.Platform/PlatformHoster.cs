using System;
using System.Collections.Generic;
using System.Text;

namespace CLK.Platform
{
    public interface PlatformHoster : IDisposable
    {
        // Constructors
        void Start();
    }
}
