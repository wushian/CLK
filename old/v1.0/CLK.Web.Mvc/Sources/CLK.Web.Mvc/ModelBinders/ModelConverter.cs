using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CLK.Web.Mvc
{
    public interface IModelConverter
    {
        // Methods
        string Serialize(object value);

        object Deserialize(string value);
    }

    public abstract class ModelConverter : IModelConverter, IModelBinder
    {
        // Methods
        object IModelBinder.BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            #region Contracts

            if (controllerContext == null) throw new ArgumentNullException();
            if (bindingContext == null) throw new ArgumentNullException();

            #endregion

            // BindModel
            try
            {
                // ValueProviderResult
                var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                if (valueProviderResult == null) throw new InvalidOperationException();

                // Deserialize
                object result = this.Deserialize(valueProviderResult.AttemptedValue.Trim());

                // Return
                return result;
            }
            catch
            {
                // Return
                return null;
            }
        }

        public abstract string Serialize(object value);

        public abstract object Deserialize(string value);
    }
}
