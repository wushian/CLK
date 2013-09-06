using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLK.Collections;

namespace CLK.Reflection
{
    public sealed class ReflectSection
    {
        // Fields        
        private readonly IReflectRepository _repository = null;

        private readonly string _sectionName = null;


        // Constructors
        internal ReflectSection(IReflectRepository repository, string sectionName)
        {
            #region Contracts
                       
            if (repository == null) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(sectionName) == true) throw new ArgumentNullException();

            #endregion

            // Arguments            
            _repository = repository;
            _sectionName = sectionName;

            // Default        
            this.ReflectBuilders = new StoreDictionary<string, ReflectBuilder>(new ReflectBuilderRepository(repository, sectionName));
        }


        // Properties
        public string DefaultEntityName
        {
            get
            {
                // Require
                if (_repository.ContainsDefaultEntityName(_sectionName) == false) return string.Empty;

                // Repository
                return _repository.GetDefaultEntityName(_sectionName);
            }
            set
            {
                // Require     
                if (string.IsNullOrEmpty(value) == true)
                {
                    if (_repository.ContainsSetting(_sectionName, value) == false)
                    {
                        throw new InvalidOperationException();
                    }
                }                

                // Repository
                if (string.IsNullOrEmpty(value) == false)
                {
                    // Add
                    _repository.RemoveDefaultEntityName(_sectionName);
                    _repository.AddDefaultEntityName(_sectionName, value);
                }
                else
                {
                    // Remove
                    _repository.RemoveDefaultEntityName(_sectionName);                    
                }                              
            }
        }

        public StoreDictionary<string, ReflectBuilder> ReflectBuilders { get; private set; }
    }
}
