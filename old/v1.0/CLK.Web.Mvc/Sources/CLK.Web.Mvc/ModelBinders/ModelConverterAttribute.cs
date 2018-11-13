using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class ModelConverterAttribute : CustomModelConverterAttribute
    {
        // Methods
        public override System.Web.Mvc.IModelBinder GetBinder()
        {
            return this.GetModelConverter();
        }

        public override CLK.Web.Mvc.IModelConverter GetConverter()
        {
            return this.GetModelConverter();
        }

        protected abstract ModelConverter GetModelConverter();
    }
}
