using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
