using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Projection
{
    public abstract class Device 
    {
        // Methods
        internal protected abstract void Start();

        internal protected abstract void Stop();
    }
}
