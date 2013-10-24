using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Operation
{
    public abstract class ComponentWrapper
    {
        // Constructor         
        internal ComponentWrapper() { }


        // Methods
        internal abstract IEnumerable<TResource> Create<TResource>(object component) where TResource : class;
    }

    public abstract class ComponentWrapper<TSource, TResult> : ComponentWrapper
        where TSource : class
         where TResult : class
    {
        // Constructor         
        public ComponentWrapper() { }


        // Methods
        internal override IEnumerable<TResource> Create<TResource>(object component) 
        {
            #region Contracts

            if (component == null) throw new ArgumentNullException();

            #endregion

            // Require       
            if (typeof(TResource) != typeof(TResult)) return null;

            // Source
            TSource source = component as TSource;
            if (source == null) return null;

            // Create
            IEnumerable<TResult> resultCollection = this.Create(source);
            if (resultCollection == null) return null;

            // Return
            return resultCollection as IEnumerable<TResource>;
        }

        protected abstract IEnumerable<TResult> Create(TSource source);
    }
}
