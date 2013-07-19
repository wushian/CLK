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
            this.SectionName = string.Empty;
            this.BuilderName = string.Empty;
            this.Parameters = new Dictionary<string, string>();
            this.ReflectContext = null;
        }


        // Properties
        public string SectionName { get; set; }

        public string BuilderName { get; set; }

        public Dictionary<string, string> Parameters { get; private set; }

        protected SettingContext SettingContext { get; private set; }

        protected IReflectContext ReflectContext { get; private set; }


        // Methods
        internal void Initialize(SettingContext settingContext, IReflectContext reflectContext)
        {
            #region Contracts

            if (settingContext == null) throw new ArgumentNullException();
            if (reflectContext == null) throw new ArgumentNullException();

            #endregion

            // Arguments
            this.SettingContext = settingContext;
            this.ReflectContext = reflectContext;
        }
               

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


        internal protected abstract object CreateEntity();
    }
}
