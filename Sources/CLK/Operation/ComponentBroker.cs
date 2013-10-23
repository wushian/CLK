using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Operation
{
    public abstract class ComponentBroker
    {
        // Constructors
        internal ComponentBroker(object component)
        {
            #region Contracts

            if (component == null) throw new ArgumentNullException();

            #endregion

            // Arguments
            this.NativeComponent = component;
        }


        // Properties
        internal object NativeComponent { get; private set; }


        // Methods          
        internal virtual void Initialize(IEnumerable<object> componentCollection, IEnumerable<ComponentWrapper> componentWrapperCollection) { }

        internal protected virtual void Start() { }

        internal protected virtual void Stop() { }        
    }

    public class ComponentBroker<TComponent> : ComponentBroker
        where TComponent : class
    {
        // Constructors
        public ComponentBroker(TComponent component)
            : base(component)
        {
            #region Contracts

            if (component == null) throw new ArgumentNullException();

            #endregion

            // Arguments
            this.Component = component;
        }


        // Properties
        protected TComponent Component { get; private set; }


        // Methods
        internal void Initialize<TResource>(IEnumerable<object> componentCollection, IEnumerable<ComponentWrapper> componentWrapperCollection, Action<IEnumerable<TResource>> initializeDelegate) where TResource : class
        {
            #region Contracts

            if (componentWrapperCollection == null) throw new ArgumentNullException();
            if (componentCollection == null) throw new ArgumentNullException();
            if (initializeDelegate == null) throw new ArgumentNullException();

            #endregion

            // ResourceCollection
            IEnumerable<TResource> resourceCollection = this.CreateAll<TResource>(componentCollection, componentWrapperCollection);
            if (resourceCollection == null) throw new InvalidOperationException();

            // InitializeDelegate
            initializeDelegate(resourceCollection);
        }
         
        private IEnumerable<TResource> CreateAll<TResource>(IEnumerable<object> componentCollection, IEnumerable<ComponentWrapper> componentWrapperCollection) where TResource : class
        {
            #region Contracts
                        
            if (componentCollection == null) throw new ArgumentNullException();
            if (componentWrapperCollection == null) throw new ArgumentNullException();

            #endregion

            // Result
            List<TResource> resourceCollection = new List<TResource>();

            // Create
            foreach (object component in componentCollection)
            {
                IEnumerable<TResource> resultCollection = this.CreateAll<TResource>(component, componentWrapperCollection);
                if (resultCollection != null) resourceCollection.AddRange(resultCollection);
            }

            // Return
            return resourceCollection;
        }

        private IEnumerable<TResource> CreateAll<TResource>(object component, IEnumerable<ComponentWrapper> componentWrapperCollection) where TResource : class
        {
            #region Contracts
                        
            if (component == null) throw new ArgumentNullException();
            if (componentWrapperCollection == null) throw new ArgumentNullException();

            #endregion

            // Result
            List<TResource> resourceCollection = new List<TResource>();

            // Component
            TResource resource = component as TResource;
            if (resource != null) resourceCollection.Add(resource);

            // Wrapper
            foreach (ComponentWrapper componentWrapper in componentWrapperCollection)
            {
                IEnumerable<TResource> resultCollection = componentWrapper.Create<TResource>(component);
                if (resultCollection != null) resourceCollection.AddRange(resultCollection);
            }

            // Return
            return resourceCollection;
        }
    }

    public abstract class ComponentBroker<TComponent, T1> : ComponentBroker<TComponent>
        where TComponent : class
        where T1 : class
    {
        // Constructors
        public ComponentBroker(TComponent component) : base(component) { }


        // Methods
        internal override void Initialize(IEnumerable<object> componentCollection, IEnumerable<ComponentWrapper> componentWrapperCollection)
        {
            #region Contracts

            if (componentWrapperCollection == null) throw new ArgumentNullException();
            if (componentCollection == null) throw new ArgumentNullException();

            #endregion
  
            // Base
            base.Initialize(componentCollection, componentWrapperCollection);

            // Initialize
            this.Initialize<T1>(componentCollection, componentWrapperCollection, this.Initialize);
        }

        protected abstract void Initialize(IEnumerable<T1> resourceCollection);
    }

    public abstract class ComponentBroker<TComponent, T1, T2> : ComponentBroker<TComponent, T1>
        where TComponent : class
        where T1 : class
        where T2 : class
    {
        // Constructors
        public ComponentBroker(TComponent component) : base(component) { }


        // Methods
        internal override void Initialize(IEnumerable<object> componentCollection, IEnumerable<ComponentWrapper> componentWrapperCollection)
        {
            #region Contracts

            if (componentWrapperCollection == null) throw new ArgumentNullException();
            if (componentCollection == null) throw new ArgumentNullException();

            #endregion

            // Base
            base.Initialize(componentCollection, componentWrapperCollection);

            // Initialize
            this.Initialize<T2>(componentCollection, componentWrapperCollection, this.Initialize);
        }

        protected abstract void Initialize(IEnumerable<T2> resourceCollection);
    }

    public abstract class ComponentBroker<TComponent, T1, T2, T3> : ComponentBroker<TComponent, T1, T2>
        where TComponent : class
        where T1 : class
        where T2 : class
        where T3 : class
    {
        // Constructors
        public ComponentBroker(TComponent component) : base(component) { }


        // Methods
        internal override void Initialize(IEnumerable<object> componentCollection, IEnumerable<ComponentWrapper> componentWrapperCollection)
        {
            #region Contracts

            if (componentWrapperCollection == null) throw new ArgumentNullException();
            if (componentCollection == null) throw new ArgumentNullException();

            #endregion

            // Base
            base.Initialize(componentCollection, componentWrapperCollection);

            // Initialize
            this.Initialize<T3>(componentCollection, componentWrapperCollection, this.Initialize);
        }

        protected abstract void Initialize(IEnumerable<T3> resourceCollection);
    }
}