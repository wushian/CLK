using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Operation
{
    public abstract class ComponentWrapper
    {
        // Constructor         
        internal ComponentWrapper() { }


        // Methods
        internal abstract TAdapter Create<TAdapter>(object component) where TAdapter : class;
    }

    public abstract class ComponentWrapper<TSource, TResult> : ComponentWrapper
        where TSource : class
         where TResult : class
    {
        // Constructor         
        public ComponentWrapper() { }


        // Methods
        internal override TAdapter Create<TAdapter>(object component) 
        {
            #region Contracts

            if (component == null) throw new ArgumentNullException();

            #endregion

            // Require       
            if (typeof(TAdapter) != typeof(TResult)) return null;

            // Source
            TSource source = component as TSource;
            if (source == null) return null;

            // Create
            TResult result = this.Create(source);
            if (result == null) return null;

            // Return
            return result as TAdapter;
        }

        protected abstract TResult Create(TSource source);
    }
}
