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

            // ReflectBuilderDictionary        
            IReflectBuilderRepository reflectBuilderRepository = null;
            reflectBuilderRepository = new ReflectBuilderRepository(repository, sectionName);
            this.ReflectBuilders = new ReflectBuilderDictionary(reflectBuilderRepository);
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

        public ReflectBuilderDictionary ReflectBuilders { get; private set; }
    }
}
