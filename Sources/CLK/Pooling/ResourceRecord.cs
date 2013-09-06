using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Pooling 
{
    internal sealed class ResourceRecord<TResourceKey, TResource>
    {
        // Fields
        private readonly object _syncRoot = new object();

        private readonly List<Guid> _consumerList = new List<Guid>();


        // Constructors
        public ResourceRecord(TResourceKey resourceKey, TResource resource)
        {
            #region Contracts

            if (resourceKey == null) throw new ArgumentNullException();
            if (resource == null) throw new ArgumentNullException();

            #endregion
            this.ResourceKey = resourceKey;
            this.Resource = resource;
        }


        // Properties
        public TResourceKey ResourceKey { get; private set; }

        public TResource Resource { get; private set; }

        public bool IsReleased
        {
            get
            {
                lock (_syncRoot)
                {
                    if (_consumerList.Count <= 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
        }


        // Methods   
        public void Register(Guid consumerId)
        {
            #region Contracts

            if (consumerId == Guid.Empty) throw new ArgumentException();

            #endregion

            lock (_syncRoot)
            {
                if (_consumerList.Contains(consumerId) == false)
                {
                    _consumerList.Add(consumerId);
                }
            }
        }

        public void Unregister(Guid consumerId)
        {
            #region Contracts

            if (consumerId == Guid.Empty) throw new ArgumentException();

            #endregion

            lock (_syncRoot)
            {
                if (_consumerList.Contains(consumerId) == true)
                {
                    _consumerList.Remove(consumerId);
                }
            }
        }
    }
}
