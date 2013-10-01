using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ComponentModel.Operation
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

    public abstract class ComponentBroker<TComponent> : ComponentBroker
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
        internal void Attach<TImport>(IEnumerable<ComponentWrapper> componentWrapperCollection, IEnumerable<object> componentCollection, Action<IEnumerable<TImport>> attachDelegate, ref IEnumerable<TImport> importCollection) where TImport : class
        {
            #region Contracts

            if (componentWrapperCollection == null) throw new ArgumentNullException();
            if (componentCollection == null) throw new ArgumentNullException();
            if (attachDelegate == null) throw new ArgumentNullException();

            #endregion

            // ImportCollection
            importCollection = this.CreateAll<TImport>(componentWrapperCollection, componentCollection);
            if (importCollection == null) throw new InvalidOperationException();

            // AttachDelegate
            attachDelegate(importCollection);
        }

        internal void Detach<TImport>(IEnumerable<ComponentWrapper> componentWrapperCollection, IEnumerable<object> componentCollection, Action<IEnumerable<TImport>> detachDelegate, ref IEnumerable<TImport> importCollection) where TImport : class
        {
            #region Contracts

            if (componentWrapperCollection == null) throw new ArgumentNullException();
            if (componentCollection == null) throw new ArgumentNullException();
            if (detachDelegate == null) throw new ArgumentNullException();

            #endregion

            // Require
            if (importCollection == null) return;

            // Detach
            detachDelegate(importCollection);

            // ImportCollection
            importCollection = null;
        }

        private IEnumerable<TImport> CreateAll<TImport>(IEnumerable<ComponentWrapper> componentWrapperCollection, IEnumerable<object> componentCollection) where TImport : class
        {
            #region Contracts

            if (componentWrapperCollection == null) throw new ArgumentNullException();
            if (componentCollection == null) throw new ArgumentNullException();

            #endregion

            // Result
            IEnumerable<TImport> importCollection = new TImport[0];

            // Create
            foreach (object component in componentCollection)
            {
                IEnumerable<TImport> resultCollection = this.CreateAll<TImport>(componentWrapperCollection, component);
                if (resultCollection == null) throw new InvalidOperationException();
                importCollection = importCollection.Concat(resultCollection);
            }

            // Return
            return importCollection;
        }

        private IEnumerable<TImport> CreateAll<TImport>(IEnumerable<ComponentWrapper> componentWrapperCollection, object component) where TImport : class
        {
            #region Contracts

            if (componentWrapperCollection == null) throw new ArgumentNullException();
            if (component == null) throw new ArgumentNullException();

            #endregion

            // Result
            List<TImport> importCollection = new List<TImport>();
            TImport import = null;

            // Component
            import = component as TImport;
            if (import != null) importCollection.Add(import);

            // Wrapper
            foreach (ComponentWrapper componentWrapper in componentWrapperCollection)
            {
                import = componentWrapper.Create<TImport>(component);
                if (import != null) importCollection.Add(import);
            }

            // Return
            return importCollection;
        }
    }

    public abstract class ComponentBroker<TComponent, T1> : ComponentBroker<TComponent>
        where TComponent : class
        where T1 : class
    {
        // Fields        
        private IEnumerable<T1> _importCollection = null;


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
            this.Attach<T1>(componentWrapperCollection, componentCollection, this.Attach, ref _importCollection);
        }

        internal override void Detach(IEnumerable<ComponentWrapper> componentWrapperCollection, IEnumerable<object> componentCollection)
        {
            #region Contracts

            if (componentWrapperCollection == null) throw new ArgumentNullException();
            if (componentCollection == null) throw new ArgumentNullException();

            #endregion

            // Detach
            this.Detach<T1>(componentWrapperCollection, componentCollection, this.Detach, ref _importCollection);

            // Base
            base.Detach(componentWrapperCollection, componentCollection);
        }

        protected abstract void Attach(IEnumerable<T1> importCollection);

        protected abstract void Detach(IEnumerable<T1> importCollection);
    }

    public abstract class ComponentBroker<TComponent, T1, T2> : ComponentBroker<TComponent, T1>
        where TComponent : class
        where T1 : class
        where T2 : class
    {
        // Fields        
        private IEnumerable<T2> _importCollection = null;


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
            this.Attach<T2>(componentWrapperCollection, componentCollection, this.Attach, ref _importCollection);
        }

        internal override void Detach(IEnumerable<ComponentWrapper> componentWrapperCollection, IEnumerable<object> componentCollection)
        {
            #region Contracts

            if (componentWrapperCollection == null) throw new ArgumentNullException();
            if (componentCollection == null) throw new ArgumentNullException();

            #endregion

            // Detach
            this.Detach<T2>(componentWrapperCollection, componentCollection, this.Detach, ref _importCollection);

            // Base
            base.Detach(componentWrapperCollection, componentCollection);
        }

        protected abstract void Attach(IEnumerable<T2> importCollection);

        protected abstract void Detach(IEnumerable<T2> importCollection);
    }

    public abstract class ComponentBroker<TComponent, T1, T2, T3> : ComponentBroker<TComponent, T1, T2>
        where TComponent : class
        where T1 : class
        where T2 : class
        where T3 : class
    {
        // Fields        
        private IEnumerable<T3> _importCollection = null;


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
            this.Attach<T3>(componentWrapperCollection, componentCollection, this.Attach, ref _importCollection);
        }

        internal override void Detach(IEnumerable<ComponentWrapper> componentWrapperCollection, IEnumerable<object> componentCollection)
        {
            #region Contracts

            if (componentWrapperCollection == null) throw new ArgumentNullException();
            if (componentCollection == null) throw new ArgumentNullException();

            #endregion

            // Detach
            this.Detach<T3>(componentWrapperCollection, componentCollection, this.Detach, ref _importCollection);

            // Base
            base.Detach(componentWrapperCollection, componentCollection);
        }

        protected abstract void Attach(IEnumerable<T3> importCollection);

        protected abstract void Detach(IEnumerable<T3> importCollection);
    }
}