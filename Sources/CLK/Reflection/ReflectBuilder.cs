using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLK.Settings;

namespace CLK.Reflection
{
    public abstract class ReflectBuilder
    {
        // Constructors
        public ReflectBuilder()
        {
            // Default            
            this.Parameters = new Dictionary<string, string>();
        }


        // Properties
        internal protected Dictionary<string, string> Parameters { get; private set; }

        internal protected IReflectContext ReflectContext { get; internal set; }

        internal protected SettingContext SettingContext { get; internal set; }  


        // Methods        
        internal protected abstract object CreateEntity();


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
                resultValue = convertDelegate(this.Parameters[parameterName]);
            }

            // Return
            return resultValue;
        }


        protected void SetParameterValue(string parameterName, string parameterValue)
        {
            #region Contracts

            if (string.IsNullOrEmpty(parameterName) == true) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(parameterValue) == true) throw new ArgumentNullException();

            #endregion

            // Set
            if (this.Parameters.ContainsKey(parameterName) == true)
            {
                this.Parameters[parameterName] = parameterValue;
            }
            else
            {
                this.Parameters.Add(parameterName, parameterValue);
            }
        }

        protected void SetParameterValue<TValue>(string parameterName, TValue parameterValue, Func<TValue, string> convertDelegate)
        {
            #region Contracts

            if (string.IsNullOrEmpty(parameterName) == true) throw new ArgumentNullException();
            if (parameterValue == null) throw new ArgumentNullException();
            if (convertDelegate == null) throw new ArgumentNullException();

            #endregion

            // Result
            string parameterValueString = convertDelegate(parameterValue);
            if (string.IsNullOrEmpty(parameterValueString) == true) throw new ArgumentNullException();

            // Set
            if (this.Parameters.ContainsKey(parameterName) == true)
            {
                this.Parameters[parameterName] = parameterValueString;
            }
            else
            {
                this.Parameters.Add(parameterName, parameterValueString);
            }
        }
    }
}
