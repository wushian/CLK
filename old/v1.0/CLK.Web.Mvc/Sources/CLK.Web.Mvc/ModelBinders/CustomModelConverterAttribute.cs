using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public abstract class CustomModelConverterAttribute : System.Web.Mvc.CustomModelBinderAttribute
    {
        // Methods
        public abstract CLK.Web.Mvc.IModelConverter GetConverter();
    }
}
