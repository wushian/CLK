using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Operation
{
    public abstract class ComponentBroker
    {
        // Constructors
        internal ComponentBroker() { }


        // Properties
        internal abstract Type ComponentType { get; }

        internal abstract object NativeComponent { get; }


        // Methods          
        internal virtual void Initialize(IEnumerable<ComponentBroker> componentBrokerCollection, IEnumerable<ComponentWrapper> componentWrapperCollection) { }

        internal protected virtual void Start() { }

        internal protected virtual void Stop() { }


        internal bool CanCreateAll<TResource>(IEnumerable<ComponentBroker> componentBrokerCollection, IEnumerable<ComponentWrapper> componentWrapperCollection) where TResource : class
        {
            #region Contracts

            if (componentBrokerCollection == null) throw new ArgumentNullException();
            if (componentWrapperCollection == null) throw new ArgumentNullException();

            #endregion

            // Require    
            foreach (ComponentBroker componentBroker in componentBrokerCollection)
            {
                if (componentBroker.NativeComponent == null)
                {
                    foreach (ComponentWrapper componentWrapper in componentWrapperCollection)
                    {
                        if (componentWrapper.CanCreate<TResource>(componentBroker.ComponentType) == true)
                        {
                            return false;
                        }
                    }
                }
            }

            // Return
            return true;
        }

        internal IEnumerable<TResource> CreateAll<TResource>(IEnumerable<ComponentBroker> componentBrokerCollection, IEnumerable<ComponentWrapper> componentWrapperCollection) where TResource : class
        {
            #region Contracts

            if (componentBrokerCollection == null) throw new ArgumentNullException();
            if (componentWrapperCollection == null) throw new ArgumentNullException();

            #endregion

            // Result
            List<TResource> resourceCollection = new List<TResource>();

            // Create            
            foreach (ComponentBroker componentBroker in componentBrokerCollection)
            {
                if (componentBroker.NativeComponent != null)
                {
                    IEnumerable<TResource> resultCollection = this.CreateAll<TResource>(componentBroker.NativeComponent, componentWrapperCollection);
                    if (resultCollection != null) resourceCollection.AddRange(resultCollection);
                }
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

    public class ComponentBroker<TComponent> : ComponentBroker
        where TComponent : class
    {
        // Constructors
        public ComponentBroker(TComponent component)
        {
            #region Contracts

            if (component == null) throw new ArgumentNullException();

            #endregion

            // Arguments
            this.Component = component;
        }

        internal ComponentBroker() { }


        // Properties
        internal override Type ComponentType { get { return typeof(TComponent); } }

        internal override object NativeComponent { get { return this.Component; } }

        internal protected TComponent Component { get; internal set; }
    }

    public abstract class ComponentBroker<TComponent, T1> : ComponentBroker<TComponent>
        where TComponent : class
        where T1 : class
    {
        // Constructors
        public ComponentBroker() { }
        

        // Methods
        internal override void Initialize(IEnumerable<ComponentBroker> componentBrokerCollection, IEnumerable<ComponentWrapper> componentWrapperCollection)
        {
            #region Contracts

            if (componentBrokerCollection == null) throw new ArgumentNullException();
            if (componentWrapperCollection == null) throw new ArgumentNullException();

            #endregion

            // Require
            if (this.CanCreateAll<T1>(componentBrokerCollection, componentWrapperCollection) == false) return;

            // T1
            IEnumerable<T1> argument01Collection = this.CreateAll<T1>(componentBrokerCollection, componentWrapperCollection);
            if (argument01Collection == null) throw new InvalidOperationException();

            // Component
            this.Component = this.CreateInstance(argument01Collection);
            if (this.Component == null) throw new InvalidOperationException();
        }

        protected abstract TComponent CreateInstance(IEnumerable<T1> argument01Collection);
    }

    public abstract class ComponentBroker<TComponent, T1, T2> : ComponentBroker<TComponent>
        where TComponent : class
        where T1 : class
        where T2 : class
    {
        // Constructors
        public ComponentBroker() { }


        // Methods
        internal override void Initialize(IEnumerable<ComponentBroker> componentBrokerCollection, IEnumerable<ComponentWrapper> componentWrapperCollection)
        {
            #region Contracts

            if (componentBrokerCollection == null) throw new ArgumentNullException();
            if (componentWrapperCollection == null) throw new ArgumentNullException();

            #endregion

            // Require
            if (this.CanCreateAll<T1>(componentBrokerCollection, componentWrapperCollection) == false) return;
            if (this.CanCreateAll<T2>(componentBrokerCollection, componentWrapperCollection) == false) return;

            // T1
            IEnumerable<T1> argument01Collection = this.CreateAll<T1>(componentBrokerCollection, componentWrapperCollection);
            if (argument01Collection == null) throw new InvalidOperationException();

            // T2
            IEnumerable<T2> argument02Collection = this.CreateAll<T2>(componentBrokerCollection, componentWrapperCollection);
            if (argument02Collection == null) throw new InvalidOperationException();

            // Component
            this.Component = this.CreateInstance(argument01Collection, argument02Collection);
            if (this.Component == null) throw new InvalidOperationException();
        }

        protected abstract TComponent CreateInstance(IEnumerable<T1> argument01Collection, IEnumerable<T2> argument02Collection);
    }

    public abstract class ComponentBroker<TComponent, T1, T2, T3> : ComponentBroker<TComponent>
        where TComponent : class
        where T1 : class
        where T2 : class
        where T3 : class
    {
        // Constructors
        public ComponentBroker() { }


        // Methods
        internal override void Initialize(IEnumerable<ComponentBroker> componentBrokerCollection, IEnumerable<ComponentWrapper> componentWrapperCollection)
        {
            #region Contracts

            if (componentBrokerCollection == null) throw new ArgumentNullException();
            if (componentWrapperCollection == null) throw new ArgumentNullException();

            #endregion

            // Require
            if (this.CanCreateAll<T1>(componentBrokerCollection, componentWrapperCollection) == false) return;
            if (this.CanCreateAll<T2>(componentBrokerCollection, componentWrapperCollection) == false) return;
            if (this.CanCreateAll<T3>(componentBrokerCollection, componentWrapperCollection) == false) return;

            // T1
            IEnumerable<T1> argument01Collection = this.CreateAll<T1>(componentBrokerCollection, componentWrapperCollection);
            if (argument01Collection == null) throw new InvalidOperationException();

            // T2
            IEnumerable<T2> argument02Collection = this.CreateAll<T2>(componentBrokerCollection, componentWrapperCollection);
            if (argument02Collection == null) throw new InvalidOperationException();

            // T3
            IEnumerable<T3> argument03Collection = this.CreateAll<T3>(componentBrokerCollection, componentWrapperCollection);
            if (argument03Collection == null) throw new InvalidOperationException();

            // Component
            this.Component = this.CreateInstance(argument01Collection, argument02Collection, argument03Collection);
            if (this.Component == null) throw new InvalidOperationException();
        }

        protected abstract TComponent CreateInstance(IEnumerable<T1> argument01Collection, IEnumerable<T2> argument02Collection, IEnumerable<T3> argument03Collection);
    }
}