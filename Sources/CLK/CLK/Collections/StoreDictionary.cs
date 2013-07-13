using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Collections
{
    public class StoreDictionary<TKey, TValue>
    {
        // Fields
        private readonly IStoreRepository<TKey, TValue> _repository = null;


        // Constructors
        public StoreDictionary(IStoreRepository<TKey, TValue> repository)
        {
            #region Contracts

            if (repository == null) throw new ArgumentNullException();

            #endregion

            // Repository
            _repository = repository;
        }


        // Properties   
        public IEnumerable<TKey> Keys
        {
            get
            {
                // Repository
                return _repository.GetAllKey();
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                // Repository
                return _repository.GetValue(key);
            }
            set
            {
                // Repository
                _repository.Remove(key);
                _repository.Add(key, value);
            }
        }


        // Methods  
        public void Add(TKey key, TValue value)
        {
            // Repository
            _repository.Remove(key);
            _repository.Add(key, value);
        }

        public bool Remove(TKey key)
        {
            // Require
            if (_repository.ContainsKey(key) == false) return false;

            // Repository
            _repository.Remove(key);

            // Return
            return true;
        }

        public bool ContainsKey(TKey key)
        {
            // Repository
            return _repository.ContainsKey(key);
        }
    }
}
