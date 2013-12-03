using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLK.Collections;

namespace CLK.Reflection
{
    internal interface IReflectSectionRepository
    {
        // Methods
        void Add(string sectionName);

        void Remove(string sectionName);

        bool ContainsKey(string sectionName);

        IEnumerable<string> GetAllKey();

        ReflectSection GetValue(string sectionName);     
    }

    internal sealed class ReflectSectionRepository : IReflectSectionRepository
    {
        // Fields        
        private readonly IReflectRepository _repository = null;    


        // Constructors
        public ReflectSectionRepository(IReflectRepository repository)
        {
            #region Contracts

            if (repository == null) throw new ArgumentNullException();

            #endregion

            // Arguments
            _repository = repository;
        }
        

        // Methods
        public void Add(string sectionName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(sectionName) == true) throw new ArgumentNullException();

            #endregion
                        
            // Repository
            _repository.AddSection(sectionName);
        }

        public void Remove(string sectionName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(sectionName) == true) throw new ArgumentNullException();

            #endregion

            // Repository
            _repository.RemoveSection(sectionName);
        }

        public bool ContainsKey(string sectionName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(sectionName) == true) throw new ArgumentNullException();

            #endregion

            // Repository
            return _repository.ContainsSection(sectionName);
        }
        
        public IEnumerable<string> GetAllKey()
        {
            // Repository
            return _repository.GetAllSectionName();
        }

        public ReflectSection GetValue(string sectionName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(sectionName) == true) throw new ArgumentNullException();

            #endregion

            // Repository
            if (_repository.ContainsSection(sectionName) == true)
            {
                new ReflectSection(_repository, sectionName);
            }
            return null;
        }
    }

    internal sealed class CacheReflectSectionRepository : IReflectSectionRepository
    {
        // Fields    
        private readonly object _syncRoot = new object();

        private readonly IReflectSectionRepository _repository = null;

        private string _cacheSectionName = null;

        private ReflectSection _cacheSectionInstance = null;


        // Constructors
        public CacheReflectSectionRepository(IReflectSectionRepository repository)
        {
            #region Contracts

            if (repository == null) throw new ArgumentNullException();

            #endregion

            // Arguments
            _repository = repository;
        }


        // Methods
        public void Add(string sectionName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(sectionName) == true) throw new ArgumentNullException();

            #endregion
                        
            lock (_syncRoot)
            {
                // Cache
                _cacheSectionName = null;
                _cacheSectionInstance = null;

                // Repository
                _repository.Add(sectionName);
            }            
        }

        public void Remove(string sectionName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(sectionName) == true) throw new ArgumentNullException();

            #endregion
                        
            lock (_syncRoot)
            {
                // Cache
                _cacheSectionName = null;
                _cacheSectionInstance = null;

                // Repository
                _repository.Remove(sectionName);
            }
        }

        public bool ContainsKey(string sectionName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(sectionName) == true) throw new ArgumentNullException();

            #endregion

            // Repository
            return _repository.ContainsKey(sectionName);
        }

        public IEnumerable<string> GetAllKey()
        {
            // Repository
            return _repository.GetAllKey();
        }

        public ReflectSection GetValue(string sectionName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(sectionName) == true) throw new ArgumentNullException();

            #endregion
                        
            lock (_syncRoot)
            {
                // Cache
                if (_cacheSectionName == sectionName) return _cacheSectionInstance;
                _cacheSectionName = sectionName;
                _cacheSectionInstance = _repository.GetValue(sectionName);

                // Return
                return _cacheSectionInstance;
            }
        }
    }
}
