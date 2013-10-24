using CLK.Operation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Composition.Operation
{
    public interface IMefComponentWrapperBuilder
    {
        // Methods
        ComponentWrapper Create();
    }
}
