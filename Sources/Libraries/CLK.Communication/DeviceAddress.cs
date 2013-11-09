using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Communication
{
    public abstract class DeviceAddress
    {
        // Methods
        public abstract bool EqualAddress(DeviceAddress address);
    }
}
