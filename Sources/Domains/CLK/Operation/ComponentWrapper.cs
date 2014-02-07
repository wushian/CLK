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
        internal abstract bool CanCreate<TResource>(Type sourceType);

        internal abstract IEnumerable<TResource> Create<TResource>(object source) where TResource : class;
    }

    public abstract class ComponentWrapper<TSource, TResult> : ComponentWrapper
        where TSource : class
        where TResult : class
    {
        // Constructor         
        public ComponentWrapper() { }


        // Methods
        internal override bool CanCreate<TResource>(Type sourceType)
        {
            #region Contracts

            if (sourceType == null) throw new ArgumentNullException();

            #endregion

            // Require       
            if (typeof(TResult) != typeof(TResource)) return false;
            if (sourceType != typeof(TSource)) return false;

            // Return
            return true;
        }

        internal override IEnumerable<TResource> Create<TResource>(object source)
        {
            #region Contracts

            if (source == null) throw new ArgumentNullException();

            #endregion

            // Require       
            if (typeof(TResult) != typeof(TResource)) return null;

            // Source
            TSource typedSource = source as TSource;
            if (typedSource == null) return null;

            // Create
            IEnumerable<TResult> resultCollection = this.Create(typedSource);
            if (resultCollection == null) return null;

            // Return
            return resultCollection as IEnumerable<TResource>;
        }

        protected abstract IEnumerable<TResult> Create(TSource source);
    }
}
