using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLK.Collections;

namespace CLK.Configuration
{
    internal sealed class FreeAttributeProvider : IStoreProvider<string, string>
    {
        // Fields
        private readonly List<string> _fixedAttributeCollection = new List<string>();

        private readonly List<string> _freeAttributeDictionary = new List<string>();

        private readonly Action<string> _addPropertyDelegate = null;

        private readonly Action<string> _removePropertyDelegate = null;

        private readonly Func<string, string> _getPropertyValueDelegate = null;

        private readonly Action<string, string> _setPropertyValueDelegate = null;       


        // Constructors
        public FreeAttributeProvider(ConfigurationPropertyCollection configurationPropertyCollection,
                                     Action<string> addPropertyDelegate, Action<string> removePropertyDelegate,
                                     Func<string, string> getPropertyValueDelegate, Action<string, string> setPropertyValueDelegate)
        {
            #region Contracts

            if (configurationPropertyCollection == null) throw new ArgumentNullException();
            if (addPropertyDelegate == null) throw new ArgumentNullException();
            if (removePropertyDelegate == null) throw new ArgumentNullException();
            if (getPropertyValueDelegate == null) throw new ArgumentNullException();
            if (setPropertyValueDelegate == null) throw new ArgumentNullException();

            #endregion
                        
            // FixedAttributeNameCollection
            foreach (ConfigurationProperty property in configurationPropertyCollection)
            {
                _fixedAttributeCollection.Add(property.Name);
            }

            // Delegate
            _addPropertyDelegate = addPropertyDelegate;
            _removePropertyDelegate = removePropertyDelegate;
            _getPropertyValueDelegate = getPropertyValueDelegate;
            _setPropertyValueDelegate = setPropertyValueDelegate;           
        }


        // Methods
        private void AddProperty(string name)
        {
            #region Contracts

            if (string.IsNullOrEmpty(name) == true) throw new ArgumentNullException();

            #endregion

            // AddPropertyDelegate
            _addPropertyDelegate(name);
        }

        private void RemoveProperty(string name)
        {
            #region Contracts

            if (string.IsNullOrEmpty(name) == true) throw new ArgumentNullException();

            #endregion

            // RemovePropertyDelegate
            _removePropertyDelegate(name);
        }

        private string GetPropertyValue(string name)
        {
            #region Contracts

            if (string.IsNullOrEmpty(name) == true) throw new ArgumentNullException();

            #endregion

            // GetPropertyValue
            return _getPropertyValueDelegate(name) as string;
        }

        private void SetPropertyValue(string name, string value)
        {
            #region Contracts

            if (string.IsNullOrEmpty(name) == true) throw new ArgumentNullException();
            if (value == null) throw new ArgumentNullException();

            #endregion

            // SetPropertyValue
            _setPropertyValueDelegate(name, value);
        }        

        public void Refresh(ConfigurationPropertyCollection configurationPropertyCollection)
        {
            #region Contracts

            if (configurationPropertyCollection == null) throw new ArgumentNullException();

            #endregion

            // Clear
            _freeAttributeDictionary.Clear();

            // Create            
            foreach (ConfigurationProperty property in configurationPropertyCollection)
            {
                // Require
                if (_fixedAttributeCollection.Contains(property.Name) == true) continue;

                // Value
                string value = this.GetPropertyValue(property.Name);
                if (value == null) throw new InvalidOperationException();

                // Add
                if (_freeAttributeDictionary.Contains(property.Name) == false)
                {
                    _freeAttributeDictionary.Add(property.Name);
                }
            }
        }


        public void Add(string name, string value)
        {
            #region Contracts

            if (string.IsNullOrEmpty(name) == true) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(value) == true) throw new ArgumentNullException();

            #endregion

            // Base
            this.AddProperty(name);
            this.SetPropertyValue(name, value);

            // FreeAttributeDictionary        
            if (_freeAttributeDictionary.Contains(name) == false)
            {
                _freeAttributeDictionary.Add(name);
            }
        }

        public void Remove(string name)
        {
            #region Contracts

            if (string.IsNullOrEmpty(name) == true) throw new ArgumentNullException();

            #endregion

            // Base
            this.RemoveProperty(name);

            // FreeAttributeDictionary        
            if (_freeAttributeDictionary.Contains(name) == true)
            {
                _freeAttributeDictionary.Remove(name);
            }
        }

        public string GetValue(string name)
        {
            #region Contracts

            if (string.IsNullOrEmpty(name) == true) throw new ArgumentNullException();

            #endregion

            // FreeAttributeDictionary
            if (_freeAttributeDictionary.Contains(name) == false)
            {
                return null;
            }

            // Base
            return this.GetPropertyValue(name);
        }

        public IEnumerable<string> GetAllKey()
        {
            // FreeAttributeDictionary
            return _freeAttributeDictionary.ToArray();
        }

        public bool ContainsKey(string name)
        {
            #region Contracts

            if (string.IsNullOrEmpty(name) == true) throw new ArgumentNullException();

            #endregion

            // FreeAttributeDictionary
            return _freeAttributeDictionary.Contains(name);
        }
    }
}
