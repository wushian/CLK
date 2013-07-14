using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Configuration
{
    public class FreeAttributeConfigurationElement : ConfigurationElement
    {
        // Fields
        private readonly FreeAttributeProvider _freeAttributeProvider = null;

        private readonly FreeAttributeDictionary _freeAttributeDictionary = null;


        // Constructors
        public FreeAttributeConfigurationElement()
        {
            // FreeAttributeProvider
            _freeAttributeProvider = new FreeAttributeProvider(this.Properties, this.AddProperty, this.RemoveProperty, this.GetPropertyValue, this.SetPropertyValue);

            // FreeAttributeDictionary
            _freeAttributeDictionary = new FreeAttributeDictionary(_freeAttributeProvider);
        }


        // Properties
        public FreeAttributeDictionary FreeAttributes
        {
            get
            {
                return _freeAttributeDictionary;
            }
        }


        // Methods   
        private void AddProperty(string name)
        {
            #region Contracts

            if (string.IsNullOrEmpty(name) == true) throw new ArgumentNullException();

            #endregion

            // AddProperty
            if (this.Properties.Contains(name) == false)
            {
                this.Properties.Add(new ConfigurationProperty(name, typeof(string), null));
            }
        }

        private void RemoveProperty(string name)
        {
            #region Contracts

            if (string.IsNullOrEmpty(name) == true) throw new ArgumentNullException();

            #endregion

            // RemoveProperty
            if (this.Properties.Contains(name) == true)
            {
                this.Properties.Remove(name);
            }
        }

        private string GetPropertyValue(string name)
        {
            #region Contracts

            if (string.IsNullOrEmpty(name) == true) throw new ArgumentNullException();

            #endregion

            // Require
            if (this.Properties.Contains(name) == false) throw new InvalidOperationException();

            // GetPropertyValue
            return this[name] as string;
        }

        private void SetPropertyValue(string name, string value)
        {
            #region Contracts

            if (string.IsNullOrEmpty(name) == true) throw new ArgumentNullException();
            if (value == null) throw new ArgumentNullException();

            #endregion

            // Require
            if (this.Properties.Contains(name) == false) throw new InvalidOperationException();

            // SetPropertyValue
            this[name] = value;
        }       

 
        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            #region Contracts

            if (string.IsNullOrEmpty(name) == true) throw new ArgumentNullException();
            if (value == null) throw new ArgumentNullException();

            #endregion

            // Base
            if (base.OnDeserializeUnrecognizedAttribute(name, value) == true)
            {
                return true;
            }

            // AddProperty
            this.AddProperty(name);

            // SetPropertyValue
            this.SetPropertyValue(name, value);

            // Return
            return true;
        }

        protected override void PostDeserialize()
        {
            // Base
            base.PostDeserialize();

            // Provider            
            _freeAttributeProvider.Refresh(this.Properties);
        }
    }
}
