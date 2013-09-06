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
        private readonly IReflectRepository _repository = null;       
                        
        private string _cacheSectionName = null;

        private ReflectSection _cacheSectionInstance = null;


        // Constructors
        internal ReflectSectionDictionary(IReflectRepository repository)
        {
            #region Contracts

            if (repository == null) throw new ArgumentNullException();

            #endregion

            // Arguments
            _repository = repository;
        }


        // Properties   
        public IEnumerable<string> Keys
        {
            get
            {
                // Repository
                return _repository.GetAllSectionName();
            }
        }

        public ReflectSection this[string key]
        {
            get
            {
                // Require
                if (string.IsNullOrEmpty(key) == true) throw new ArgumentNullException();

                // Create
                if (_cacheSectionName != key)
                {
                    if (_repository.ContainsSection(key) == false)
                    {
                        this.Add(key);
                    }
                }

                // Cache
                if (_cacheSectionName != key)
                {
                    _cacheSectionName = key;
                    _cacheSectionInstance = new ReflectSection(_repository, key);
                }
                
                // Return
                return _cacheSectionInstance;
            }
        }


        // Methods
        public void Add(string key)
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentNullException();

            #endregion

            // Repository
            _repository.RemoveSection(key);
            _repository.AddSection(key);
        }

        public bool Remove(string key)
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentNullException();

            #endregion

            // Require
            if (_repository.ContainsSection(key) == false) return false;

            // Repository
            _repository.RemoveSection(key);

            // Return
            return true;
        }

        public bool ContainsKey(string key)
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentNullException();

            #endregion

            // Repository
            return _repository.ContainsSection(key);
        }
    }
}
