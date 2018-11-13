using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CLK.Web.Mvc
{
    public class DefaultModelBinder : System.Web.Mvc.DefaultModelBinder
    {
        // Methods
        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            #region Contracts

            if (controllerContext == null) throw new ArgumentNullException();
            if (bindingContext == null) throw new ArgumentNullException();
            if (propertyDescriptor == null) throw new ArgumentNullException();

            #endregion

            // PropertyModelBinderAttribute
            var propertyModelBinderAttribute = propertyDescriptor.Attributes.OfType<CustomModelBinderAttribute>().FirstOrDefault();
            if (propertyModelBinderAttribute == null)
            {
                // BindProperty
                base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
            }
            else
            {
                // PropertyModelBinder
                var propertyModelBinder = propertyModelBinderAttribute.GetBinder();
                if (propertyModelBinder == null) return;

                // PropertyModelBindingContext
                var propertyModelBindingContext = new ModelBindingContext()
                {
                    ModelMetadata = bindingContext.PropertyMetadata[propertyDescriptor.Name],
                    ModelName = this.CreatePropertyName(bindingContext.ModelName, propertyDescriptor.Name),
                    ModelState = bindingContext.ModelState,
                    ValueProvider = bindingContext.ValueProvider
                };

                // BindProperty
                var value = propertyModelBinder.BindModel(controllerContext, propertyModelBindingContext);
                if (value == null) return;

                // SetValue
                propertyDescriptor.SetValue(bindingContext.Model, value);
            }
        }

        private string CreatePropertyName(string prefix = null, string propertyName = null)
        {
            // PropertyName
            if (String.IsNullOrEmpty(prefix) == true)
            {
                return propertyName;
            }

            // Prefix
            if (String.IsNullOrEmpty(propertyName))
            {
                return prefix;
            }

            // Prefix.PropertyName
            return prefix + "." + propertyName;
        }
    }
}
