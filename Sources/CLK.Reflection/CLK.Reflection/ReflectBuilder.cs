using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLK.Settings;

namespace CLK.Reflection
{
    public abstract class ReflectBuilder
    {
        // Properties
        public Dictionary<string, string> Parameters { get; private set; }       


        // Methods        
        internal protected abstract object CreateEntity(IReflectContext reflectContext, SettingContext settingContext);


        protected string GetParameterValue(string parameterName, string defaultValue = default(string))
        {
            #region Contracts

            if (string.IsNullOrEmpty(parameterName) == true) throw new ArgumentNullException();

            #endregion

            // Result
            string resultValue = defaultValue;

            // Get
            if (this.Parameters.ContainsKey(parameterName) == true)
            {
                resultValue = this.Parameters[parameterName];
            }

            // Return
            return resultValue;
        }

        protected TValue GetParameterValue<TValue>(string parameterName, TValue defaultValue, Func<string, TValue> convertDelegate)
        {
            #region Contracts

            if (string.IsNullOrEmpty(parameterName) == true) throw new ArgumentNullException();
            if (convertDelegate == null) throw new ArgumentNullException();

            #endregion

            // Result
            TValue resultValue = defaultValue;

            // Get
            if (this.Parameters.ContainsKey(parameterName) == true)
            {
                if (convertDelegate != null)
                {
                    resultValue = convertDelegate(this.Parameters[parameterName]);
                }
            }

            // Return
            return resultValue;
        }             
    }
}
