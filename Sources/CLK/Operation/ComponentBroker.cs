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
        internal protected virtual void Start() { }

        internal protected virtual void Stop() { }


        internal virtual void Attach(IEnumerable<ComponentWrapper> componentWrapperCollection, IEnumerable<object> componentCollection) { }

        internal virtual void Detach(IEnumerable<ComponentWrapper> componentWrapperCollection, IEnumerable<object> componentCollection) { }
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
        internal void Attach<TAdapter>(IEnumerable<ComponentWrapper> componentWrapperCollection, IEnumerable<object> componentCollection, Action<IEnumerable<TAdapter>> attachDelegate, ref IEnumerable<TAdapter> adapterCollection) where TAdapter : class
        {
            #region Contracts

            if (componentWrapperCollection == null) throw new ArgumentNullException();
            if (componentCollection == null) throw new ArgumentNullException();
            if (attachDelegate == null) throw new ArgumentNullException();

            #endregion

            // AdapterCollection
            adapterCollection = this.CreateAll<TAdapter>(componentWrapperCollection, componentCollection);
            if (adapterCollection == null) throw new InvalidOperationException();

            // AttachDelegate
            attachDelegate(adapterCollection);
        }

        internal void Detach<TAdapter>(IEnumerable<ComponentWrapper> componentWrapperCollection, IEnumerable<object> componentCollection, Action<IEnumerable<TAdapter>> detachDelegate, ref IEnumerable<TAdapter> adapterCollection) where TAdapter : class
        {
            #region Contracts

            if (componentWrapperCollection == null) throw new ArgumentNullException();
            if (componentCollection == null) throw new ArgumentNullException();
            if (detachDelegate == null) throw new ArgumentNullException();

            #endregion

            // Require
            if (adapterCollection == null) return;

            // DetachDelegate
            detachDelegate(adapterCollection);

            // AdapterCollection
            adapterCollection = null;
        }

        private IEnumerable<TAdapter> CreateAll<TAdapter>(IEnumerable<ComponentWrapper> componentWrapperCollection, IEnumerable<object> componentCollection) where TAdapter : class
        {
            #region Contracts

            if (componentWrapperCollection == null) throw new ArgumentNullException();
            if (componentCollection == null) throw new ArgumentNullException();

            #endregion

            // Result
            IEnumerable<TAdapter> adapterCollection = new TAdapter[0];

            // Create
            foreach (object component in componentCollection)
            {
                IEnumerable<TAdapter> resultCollection = this.CreateAll<TAdapter>(componentWrapperCollection, component);
                if (resultCollection == null) throw new InvalidOperationException();
                adapterCollection = adapterCollection.Concat(resultCollection);
            }

            // Return
            return adapterCollection;
        }

        private IEnumerable<TAdapter> CreateAll<TAdapter>(IEnumerable<ComponentWrapper> componentWrapperCollection, object component) where TAdapter : class
        {
            #region Contracts

            if (componentWrapperCollection == null) throw new ArgumentNullException();
            if (component == null) throw new ArgumentNullException();

            #endregion

            // Result
            List<TAdapter> adapterCollection = new List<TAdapter>();
            TAdapter adapter = null;

            // Component
            adapter = component as TAdapter;
            if (adapter != null) adapterCollection.Add(adapter);

            // Wrapper
            foreach (ComponentWrapper componentWrapper in componentWrapperCollection)
            {
                adapter = componentWrapper.Create<TAdapter>(component);
                if (adapter != null) adapterCollection.Add(adapter);
            }

            // Return
            return adapterCollection;
        }
    }

    public abstract class ComponentBroker<TComponent, T1> : ComponentBroker<TComponent>
        where TComponent : class
        where T1 : class
    {
        // Fields        
        private IEnumerable<T1> _adapterCollection = null;


        // Constructors
        public ComponentBroker(TComponent component) : base(component) { }


        // Methods
        internal override void Attach(IEnumerable<ComponentWrapper> componentWrapperCollection, IEnumerable<object> componentCollection)
        {
            #region Contracts

            if (componentWrapperCollection == null) throw new ArgumentNullException();
            if (componentCollection == null) throw new ArgumentNullException();

            #endregion

            // Base
            base.Attach(componentWrapperCollection, componentCollection);

            // Attach
            this.Attach<T1>(componentWrapperCollection, componentCollection, this.Attach, ref _adapterCollection);
        }

        internal override void Detach(IEnumerable<ComponentWrapper> componentWrapperCollection, IEnumerable<object> componentCollection)
        {
            #region Contracts

            if (componentWrapperCollection == null) throw new ArgumentNullException();
            if (componentCollection == null) throw new ArgumentNullException();

            #endregion

            // Detach
            this.Detach<T1>(componentWrapperCollection, componentCollection, this.Detach, ref _adapterCollection);

            // Base
            base.Detach(componentWrapperCollection, componentCollection);
        }

        protected abstract void Attach(IEnumerable<T1> adapterCollection);

        protected abstract void Detach(IEnumerable<T1> adapterCollection);
    }

    public abstract class ComponentBroker<TComponent, T1, T2> : ComponentBroker<TComponent, T1>
        where TComponent : class
        where T1 : class
        where T2 : class
    {
        // Fields        
        private IEnumerable<T2> _adapterCollection = null;


        // Constructors
        public ComponentBroker(TComponent component) : base(component) { }


        // Methods
        internal override void Attach(IEnumerable<ComponentWrapper> componentWrapperCollection, IEnumerable<object> componentCollection)
        {
            #region Contracts

            if (componentWrapperCollection == null) throw new ArgumentNullException();
            if (componentCollection == null) throw new ArgumentNullException();

            #endregion

            // Base
            base.Attach(componentWrapperCollection, componentCollection);

            // Attach
            this.Attach<T2>(componentWrapperCollection, componentCollection, this.Attach, ref _adapterCollection);
        }

        internal override void Detach(IEnumerable<ComponentWrapper> componentWrapperCollection, IEnumerable<object> componentCollection)
        {
            #region Contracts

            if (componentWrapperCollection == null) throw new ArgumentNullException();
            if (componentCollection == null) throw new ArgumentNullException();

            #endregion

            // Detach
            this.Detach<T2>(componentWrapperCollection, componentCollection, this.Detach, ref _adapterCollection);

            // Base
            base.Detach(componentWrapperCollection, componentCollection);
        }

        protected abstract void Attach(IEnumerable<T2> adapterCollection);

        protected abstract void Detach(IEnumerable<T2> adapterCollection);
    }

    public abstract class ComponentBroker<TComponent, T1, T2, T3> : ComponentBroker<TComponent, T1, T2>
        where TComponent : class
        where T1 : class
        where T2 : class
        where T3 : class
    {
        // Fields        
        private IEnumerable<T3> _adapterCollection = null;


        // Constructors
        public ComponentBroker(TComponent component) : base(component) { }


        // Methods
        internal override void Attach(IEnumerable<ComponentWrapper> componentWrapperCollection, IEnumerable<object> componentCollection)
        {
            #region Contracts

            if (componentWrapperCollection == null) throw new ArgumentNullException();
            if (componentCollection == null) throw new ArgumentNullException();

            #endregion

            // Base
            base.Attach(componentWrapperCollection, componentCollection);

            // Attach
            this.Attach<T3>(componentWrapperCollection, componentCollection, this.Attach, ref _adapterCollection);
        }

        internal override void Detach(IEnumerable<ComponentWrapper> componentWrapperCollection, IEnumerable<object> componentCollection)
        {
            #region Contracts

            if (componentWrapperCollection == null) throw new ArgumentNullException();
            if (componentCollection == null) throw new ArgumentNullException();

            #endregion

            // Detach
            this.Detach<T3>(componentWrapperCollection, componentCollection, this.Detach, ref _adapterCollection);

            // Base
            base.Detach(componentWrapperCollection, componentCollection);
        }

        protected abstract void Attach(IEnumerable<T3> adapterCollection);

        protected abstract void Detach(IEnumerable<T3> adapterCollection);
    }
}