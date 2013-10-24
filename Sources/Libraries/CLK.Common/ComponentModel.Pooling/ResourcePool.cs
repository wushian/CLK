using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.ComponentModel.Pooling 
{
    public abstract class ResourcePool<TResourceKey, TResource>
    {
        // Fields
        private readonly object _syncRoot = new object();

        private readonly List<ResourceRecord<TResourceKey, TResource>> _resourceRecordCollection = new List<ResourceRecord<TResourceKey, TResource>>();


        // Methods   
        public TResource Create(Guid consumerId, TResourceKey resourceKey)
        {
            #region Contracts

            if (consumerId == Guid.Empty) throw new ArgumentException();
            if (resourceKey == null) throw new ArgumentNullException();

            #endregion     
      
            lock (_syncRoot)
            {
                // Search 
                ResourceRecord<TResourceKey, TResource> existResourceRecord = null;
                foreach (ResourceRecord<TResourceKey, TResource> resourceRecord in _resourceRecordCollection)
                {
                    if (this.CompareResourceKey(resourceKey, resourceRecord.ResourceKey) == true)
                    {
                        existResourceRecord = resourceRecord;
                        break;
                    }
                }

                if (existResourceRecord != null)
                {
                    existResourceRecord.Register(consumerId);
                    return existResourceRecord.Resource;
                }

                // Create
                TResource resource = this.CreateResource(resourceKey);
                if (resource == null) throw new InvalidOperationException("CreateResource failed.");
                                
                ResourceRecord<TResourceKey, TResource> newResourceRecord = null;
                newResourceRecord = new ResourceRecord<TResourceKey, TResource>(resourceKey, resource);
                newResourceRecord.Register(consumerId);

                // Attach                
                _resourceRecordCollection.Add(newResourceRecord);

                // Return
                return resource;
            }
        }

        public void Release(Guid consumerId, TResourceKey resourceKey)
        {
            #region Contracts

            if (consumerId == Guid.Empty) throw new ArgumentException();
            if (resourceKey == null) throw new ArgumentNullException();

            #endregion

            lock (_syncRoot)
            {
                // Search
                ResourceRecord<TResourceKey, TResource> existResourceRecord = null;
                foreach (ResourceRecord<TResourceKey, TResource> resourceRecord in _resourceRecordCollection)
                {
                    if (this.CompareResourceKey(resourceKey, resourceRecord.ResourceKey) == true)
                    {
                        existResourceRecord = resourceRecord;
                        break;
                    }
                }
                if (existResourceRecord == null) return;

                // Release
                existResourceRecord.Unregister(consumerId);
                if (existResourceRecord.IsReleased == false) return;

                // Detach
                _resourceRecordCollection.Remove(existResourceRecord);
                this.ReleaseResource(existResourceRecord.Resource);
            }
        }


        protected abstract TResource CreateResource(TResourceKey resourceKey);

        protected abstract void ReleaseResource(TResource resource);

        protected abstract bool CompareResourceKey(TResourceKey resourceKeyA, TResourceKey resourceKeyB);
    }
}
