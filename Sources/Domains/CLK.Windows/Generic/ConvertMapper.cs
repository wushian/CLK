using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Generic
{
    public sealed class ConvertMapper<TSource, TResult>
        where TSource : class
        where TResult : class
    {
        // Fields
        private readonly object _syncRoot = new object();

        private readonly Dictionary<TSource, TResult> _resourceDelegateDictionary = new Dictionary<TSource, TResult>();

        private readonly Func<TSource, TResult> _convertDelegate = null;


        // Constructors
        public ConvertMapper(Func<TSource, TResult> convertDelegate)
        {
            #region Contracts

            if (convertDelegate == null) throw new ArgumentNullException();

            #endregion

            // Arguments
            _convertDelegate = convertDelegate;
        }


        // Methods
        public TResult Attach(TSource keyDelegate)
        {
            #region Contracts

            if (keyDelegate == null) throw new ArgumentNullException();

            #endregion

            lock (_syncRoot)
            {
                // Convert
                TResult valueDelegate = _convertDelegate(keyDelegate);
                if (valueDelegate == null) return null;

                // Attach
                _resourceDelegateDictionary.Add(keyDelegate, valueDelegate);

                // Return
                return valueDelegate;
            }
        }

        public TResult Detach(TSource keyDelegate)
        {
            #region Contracts

            if (keyDelegate == null) throw new ArgumentNullException();

            #endregion

            lock (_syncRoot)
            {
                // Require
                if (_resourceDelegateDictionary.ContainsKey(keyDelegate) == false) return null;

                // Detach
                TResult valueDelegate = _resourceDelegateDictionary[keyDelegate];
                _resourceDelegateDictionary.Remove(keyDelegate);

                // Return
                return valueDelegate;
            }
        }
    }
}
