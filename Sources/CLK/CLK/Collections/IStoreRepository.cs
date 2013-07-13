using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Collections
{
    public interface IStoreRepository<TKey, TValue>
    {
        // Methods  
        void Add(TKey key, TValue value);

        void Remove(TKey key);

        TValue GetValue(TKey key);

        IEnumerable<TKey> GetAllKey();

        bool ContainsKey(TKey key);
    }
}
