using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLK.Collections;

namespace CLK.Reflection
{
    public sealed class ReflectSectionDictionary
    {
        // Fields        
        private readonly StoreDictionary<string, ReflectSection> _sectionDictionary = null;

        private readonly IStoreProvider<string, ReflectBuilder> _builderProvider = null;


        // Constructors
        internal ReflectSectionDictionary(IStoreProvider<string, ReflectSection> sectionProvider, IStoreProvider<string, ReflectBuilder> builderProvider)
        {
            #region Contracts

            if (sectionProvider == null) throw new ArgumentNullException();
            if (builderProvider == null) throw new ArgumentNullException();

            #endregion

            // Base
            _sectionDictionary = new StoreDictionary<string, ReflectSection>(sectionProvider);

            // Provider
            _builderProvider = builderProvider;
        }


        // Properties   
        public IEnumerable<string> Keys
        {
            get
            {
                // Base
                return _sectionDictionary.Keys;
            }
        }

        public ReflectSection this[string key]
        {
            get
            {
                // Base
                return _sectionDictionary[key];
            }
            set
            {
                // Base
                _sectionDictionary[key] = value;
            }
        }


        // Methods
        public void Add(string key)
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentNullException();

            #endregion

            // Create
            ReflectSection reflectSection = new ReflectSection(key, _builderProvider);

            // Base
            _sectionDictionary.Add(key, reflectSection);
        }

        public bool Remove(string key)
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentNullException();

            #endregion

            // Base
            return _sectionDictionary.Remove(key);
        }

        public bool ContainsKey(string key)
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentNullException();

            #endregion

            // Base
            return _sectionDictionary.ContainsKey(key);
        }
    }
}
