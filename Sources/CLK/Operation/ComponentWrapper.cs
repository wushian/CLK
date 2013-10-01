using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ComponentModel.Operation
{
    public abstract class ComponentWrapper
    {
        // Constructor         
        internal ComponentWrapper() { }


        // Methods
        internal abstract TImport Create<TImport>(object component) where TImport : class;
    }

    public abstract class ComponentWrapper<TSource, TResult> : ComponentWrapper
        where TSource : class
         where TResult : class
    {
        // Constructor         
        public ComponentWrapper() { }


        // Methods
        internal override TImport Create<TImport>(object component) 
        {
            #region Contracts

            if (component == null) throw new ArgumentNullException();

            #endregion

            // Require       
            if (typeof(TImport) != typeof(TResult)) return null;

            // Source
            TSource source = component as TSource;
            if (source == null) return null;

            // Create
            TResult result = this.Create(source);
            if (result == null) return null;

            // Return
            return result as TImport;
        }

        protected abstract TResult Create(TSource source);
    }
}
