using CLK.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ComponentModel.Operation
{
    public class OperateContext
    {
        // Fields    
        private readonly PortableStarterStoperLock _operateLock = new PortableStarterStoperLock();

        private readonly IEnumerable<ComponentBroker> _componentBrokerCollection = null;

        private readonly IEnumerable<ComponentWrapper> _componentWrapperCollection = null;

        private readonly IEnumerable<object> _componentCollection = null;


        // Constructors
        public OperateContext(IComponentBrokerRepository componentBrokerRepository, IComponentWrapperRepository componentWrapperRepository)
        {
            #region Contracts

            if (componentBrokerRepository == null) throw new ArgumentNullException();
            if (componentWrapperRepository == null) throw new ArgumentNullException();

            #endregion
            
            // ComponentBroker
            _componentBrokerCollection = componentBrokerRepository.GetAllComponentBroker();
            if (_componentBrokerCollection == null) throw new InvalidOperationException();                                   

            // ComponentWrapper
            _componentWrapperCollection = componentWrapperRepository.GetAllComponentWrapper();
            if (_componentWrapperCollection == null) throw new InvalidOperationException();

            // Component
            _componentCollection = (from componentBroker in _componentBrokerCollection select componentBroker.NativeComponent).ToArray();
            if (_componentCollection == null) throw new InvalidOperationException();
        }


        // Methods  
        public void Start()
        {
            // EnterStartLock
            if (_operateLock.EnterStartLock() == false) return;

            // Start
            try
            {
                // Attach
                foreach (ComponentBroker componentBroker in _componentBrokerCollection)
                {
                    componentBroker.Attach(_componentWrapperCollection, _componentBrokerCollection);
                }

                // Start
                foreach (ComponentBroker componentBroker in _componentBrokerCollection)
                {
                    componentBroker.Start();
                }
            }
            finally
            {
                // ExitStartLock
                _operateLock.ExitStartLock();
            }
        }

        public void Stop()
        {
            // EnterStopLock
            if (_operateLock.EnterStopLock() == false) return;

            // Stop
            try
            {
                // Stop
                foreach (ComponentBroker componentBroker in _componentBrokerCollection)
                {
                    componentBroker.Stop();
                }

                // Detach
                foreach (ComponentBroker componentBroker in _componentBrokerCollection)
                {
                    componentBroker.Detach(_componentWrapperCollection, _componentBrokerCollection);
                }
            }
            finally
            {
                // ExitStopLock
                _operateLock.ExitStopLock();
            }
        }


        public TComponent GetComponent<TComponent>() where TComponent : class
        {
            // Result
            TComponent component = null;

            // Component
            component = _componentCollection.OfType<TComponent>().FirstOrDefault();
            if (component != null) return component;

            // Return
            return null;
        }

        public IEnumerable<TComponent> GetAllComponent<TComponent>() where TComponent : class
        {
            // Result
            IEnumerable<TComponent> componentCollection = new TComponent[0];

            // Component
            componentCollection = componentCollection.Concat(_componentCollection.OfType<TComponent>());

            // Return
            return componentCollection;
        }
    }
}
