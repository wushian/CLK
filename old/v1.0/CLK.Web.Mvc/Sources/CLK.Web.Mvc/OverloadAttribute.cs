using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CLK.Web.Mvc
{
    public sealed class OverloadAttribute : ActionMethodSelectorAttribute
    {
        // Methods
        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            #region Contracts

            if (controllerContext == null) throw new ArgumentNullException();
            if (methodInfo == null) throw new ArgumentNullException();

            #endregion

            // ParameterInfoArray
            var parameterInfoArray = ParameterInfoFactory.Current.GetAll(controllerContext.Controller.GetType(), methodInfo);
            if (parameterInfoArray == null) throw new InvalidOperationException();

            // Current Overload Action Matching
            if (this.IsOverloadMatched(controllerContext, parameterInfoArray) == false)
            {
                return false;
            }

            // Other MethodInfoArray
            var otherMethodInfoArray = MethodInfoFactory.Current.GetAll(controllerContext.Controller.GetType());
            if (otherMethodInfoArray == null) throw new InvalidOperationException();
            
            // Other Overload Action Matching
            foreach (var otherMethodInfo in otherMethodInfoArray)
            {
                // MethodInfo
                if (otherMethodInfo.Name != methodInfo.Name) continue;
                if (otherMethodInfo.ToString() == methodInfo.ToString()) continue;
                if (otherMethodInfo.GetCustomAttribute<OverloadAttribute>() == null) continue;

                // ParameterInfoArray
                var otherParameterInfoArray = ParameterInfoFactory.Current.GetAll(controllerContext.Controller.GetType(), otherMethodInfo);
                if (otherParameterInfoArray == null) throw new InvalidOperationException();
                if (otherParameterInfoArray.Length < parameterInfoArray.Length) continue;

                // IsOverloadMatched
                if (this.IsOverloadMatched(controllerContext, otherParameterInfoArray) == true) return false;
            }

            // Return
            return true;
        }

        private bool IsOverloadMatched(ControllerContext controllerContext, ParameterInfo[] parameterInfoArray)
        {
            #region Contracts

            if (controllerContext == null) throw new ArgumentNullException();
            if (parameterInfoArray == null) throw new ArgumentNullException();

            #endregion

            // Match
            foreach (var parameterInfo in parameterInfoArray)
            {
                if (parameterInfo.HasDefaultValue == false)
                {
                    if (controllerContext.Controller.ValueProvider.ContainsPrefix(parameterInfo.Name) == false)
                    {
                        return false;
                    }
                }
            }

            // Return
            return true;
        }
    }
}