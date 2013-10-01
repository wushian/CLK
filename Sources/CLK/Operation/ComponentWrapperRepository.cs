using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ComponentModel.Operation
{
    public interface IComponentWrapperRepository
    {
        // Methods
        IEnumerable<ComponentWrapper> GetAllComponentWrapper();
    }
}
