using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.ComponentModel.Exploring
{
    public abstract class Component 
    {
        // Methods
        internal protected abstract void Start();

        internal protected abstract void Stop();
    }
}
