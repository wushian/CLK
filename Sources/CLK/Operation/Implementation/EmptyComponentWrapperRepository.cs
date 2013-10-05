using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLK.Operation;

namespace CLK.Operation
{
    public class EmptyComponentWrapperRepository : IComponentWrapperRepository
    {     
        // Constructors
        public EmptyComponentWrapperRepository() { }


        // Methods
        public IEnumerable<ComponentWrapper> GetAllComponentWrapper()
        {
            // Return
            return new ComponentWrapper[0];
        }
    }
}
