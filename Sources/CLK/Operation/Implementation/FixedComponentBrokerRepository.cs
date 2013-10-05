using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLK.Operation;

namespace CLK.Operation
{
    public class FixedComponentBrokerRepository<TComponent> : IComponentBrokerRepository
        where TComponent : class
    {
        // Fields 
        private readonly IEnumerable<ComponentBroker> _componentBrokerCollection = null;


        // Constructors
        public FixedComponentBrokerRepository(TComponent component) : this(CreateComponentBrokerCollection(new TComponent[] { component })) { }

        public FixedComponentBrokerRepository(IEnumerable<TComponent> componentCollection) : this(CreateComponentBrokerCollection(componentCollection)) { }

        public FixedComponentBrokerRepository(IEnumerable<ComponentBroker> componentBrokerCollection)
        {
            #region Contracts

            if (componentBrokerCollection == null) throw new ArgumentNullException();

            #endregion

            // Arguments
            _componentBrokerCollection = componentBrokerCollection;
        }


        // Methods
        private static IEnumerable<ComponentBroker> CreateComponentBrokerCollection(IEnumerable<TComponent> componentCollection)
        {
            #region Contracts

            if (componentCollection == null) throw new ArgumentNullException();

            #endregion

            // Result
            List<ComponentBroker> componentBrokerCollection = new List<ComponentBroker>();

            // Create
            foreach (TComponent component in componentCollection)
            {
                componentBrokerCollection.Add(new ComponentBroker<TComponent>(component));
            }

            // Return
            return componentBrokerCollection;
        }
        
        public IEnumerable<ComponentBroker> GetAllComponentBroker()
        {
            // Return
            return _componentBrokerCollection;
        }
    }
}
