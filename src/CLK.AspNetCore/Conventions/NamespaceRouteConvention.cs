using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace CLK.AspNetCore
{
    public class NamespaceRouteConvention : IApplicationModelConvention
    {
        // Methods
        public void Apply(ApplicationModel application)
        {
            #region Contracts

            if (application == null) throw new ArgumentException();

            #endregion

            // Apply
            foreach (var controller in application.Controllers)
            {
                // Require
                var hasAttributeRouteModels = controller.Selectors.Any(selector => selector.AttributeRouteModel != null);
                if (hasAttributeRouteModels == true) return;

                // Attach
                controller.Selectors[0].AttributeRouteModel = new AttributeRouteModel()
                {
                    Template = controller.ControllerType.Namespace.Replace('.', '/') + "/[controller]/[action]"
                };
            }
        }
    }
}
