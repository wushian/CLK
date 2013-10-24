using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Operation
{
    public interface IComponentWrapperRepository
    {
        // Methods
        IEnumerable<ComponentWrapper> GetAllComponentWrapper();
    }
}
