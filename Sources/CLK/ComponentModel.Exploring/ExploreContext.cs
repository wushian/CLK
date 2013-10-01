using CLK.Collections.Concurrent;
using CLK.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.ComponentModel.Exploring
{
    public abstract class ExploreContext<TComponent>
       where TComponent : Component
    {
        // Fields     
        private readonly PortableStarterStoperLock _operateLock = new PortableStarterStoperLock();

        private readonly ComponentCollection<TComponent> _componentCollection = null;

        private readonly IEnumerable<ComponentHost<TComponent>> _componentHostCollection = null;


        // Constructor 
        public ExploreContext(IEnumerable<ComponentHost<TComponent>> componentHostCollection)
        {
            #region Contracts

            if (componentHostCollection == null) throw new ArgumentNullException();

            #endregion

            // ComponentCollection
            _componentCollection = new ComponentCollection<TComponent>(this.EqualComponent);

            // ComponentHostCollection
            _componentHostCollection = componentHostCollection.ToArray();
        }


        // Properties   
        protected IEnumerable<TComponent> ComponentCollection
        {
            get
            {
                return _componentCollection;
            }
        }
        

        // Methods  
        public virtual void Start()
        {
            // EnterStartLock
            if (_operateLock.EnterStartLock() == false) return;

            // Start
            try
            {
                // ComponentCollection
                foreach (TComponent component in _componentCollection.ToArray())
                {
                    this.AttachComponent(component);
                }

                // ComponentHostCollection
                foreach (ComponentHost<TComponent> componentHost in _componentHostCollection)
                {
                    componentHost.ComponentArrived += this.ComponentHost_ComponentArrived;
                    componentHost.ComponentDeparted += this.ComponentHost_ComponentDeparted;
                    componentHost.Start();
                }
            }
            finally
            {
                // ExitStartLock
                _operateLock.ExitStartLock();
            }
        }

        public virtual void Stop()
        {
            // EnterStopLock
            if (_operateLock.EnterStopLock() == false) return;

            // Stop
            try
            {
                // ComponentHostCollection
                foreach (ComponentHost<TComponent> componentHost in _componentHostCollection.Reverse().ToArray())
                {
                    componentHost.Stop();
                    componentHost.ComponentArrived -= this.ComponentHost_ComponentArrived;
                    componentHost.ComponentDeparted -= this.ComponentHost_ComponentDeparted;
                }

                // ComponentCollection
                foreach (TComponent component in _componentCollection.ToArray())
                {
                    this.DetachComponent(component);
                }
            }
            finally
            {
                // ExitStopLock
                _operateLock.ExitStopLock();
            }            
        }


        private void AttachComponent(TComponent component)
        {
            #region Contracts

            if (component == null) throw new ArgumentNullException();

            #endregion

            // Attach
            component = _componentCollection.Attach(component);
            if (component == null) return;

            // Notify
            this.OnComponentArrived(component);

            // Start            
            component.Start();
        }

        private void DetachComponent(TComponent component)
        {
            #region Contracts

            if (component == null) throw new ArgumentNullException();

            #endregion

            // Attach
            component = _componentCollection.Detach(component);
            if (component == null) return;

            // Stop            
            component.Stop();

            // Notify
            this.OnComponentDeparted(component);
        }

        protected abstract bool EqualComponent(TComponent componentA, TComponent componentB);


        // Handlers
        private void ComponentHost_ComponentArrived(TComponent component)
        {
            #region Contracts

            if (component == null) throw new ArgumentNullException();

            #endregion

            // Attach
            this.AttachComponent(component);
        }  

        private void ComponentHost_ComponentDeparted(TComponent component)
        {
            #region Contracts

            if (component == null) throw new ArgumentNullException();

            #endregion

            // Detach
            this.DetachComponent(component);
        }


        // Events
        protected event Action<TComponent> ComponentArrived;
        private void OnComponentArrived(TComponent component)
        {
            #region Contracts

            if (component == null) throw new ArgumentNullException();

            #endregion

            var handler = this.ComponentArrived;
            if (handler != null)
            {
                handler(component);
            }
        }

        protected event Action<TComponent> ComponentDeparted;
        private void OnComponentDeparted(TComponent component)
        {
            #region Contracts

            if (component == null) throw new ArgumentNullException();

            #endregion

            var handler = this.ComponentDeparted;
            if (handler != null)
            {
                handler(component);
            }
        }
    }
}
