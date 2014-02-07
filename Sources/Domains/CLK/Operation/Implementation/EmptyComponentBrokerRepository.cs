using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Operation
{
    public class EmptyComponentBrokerRepository : IComponentBrokerRepository
    {
        // Constructors
        public EmptyComponentBrokerRepository() { }


        // Methods
        public IEnumerable<ComponentBroker> GetAllComponentBroker()
        {
            // Return
            return new ComponentBroker[0];
        }
    }
}
