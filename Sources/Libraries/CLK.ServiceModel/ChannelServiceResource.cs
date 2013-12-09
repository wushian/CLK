using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceModel
{
    internal sealed partial class ChannelServiceResource
    {
        // Singleton
        private static ChannelServiceResource _current = null;

        public static ChannelServiceResource Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new ChannelServiceResource();
                }
                return _current;
            }
        }
    }

    internal sealed partial class ChannelServiceResource
    {
        // Fields  
        private readonly object _syncRoot = new object();

        private readonly Dictionary<ServiceHostBase, List<object>> _resourceDictionary = new Dictionary<ServiceHostBase, List<object>>();


        // Methods
        public void AttachResource(ServiceHostBase serviceHost, object resource)
        {
            #region Contracts

            if (serviceHost == null) throw new ArgumentNullException();
            if (resource == null) throw new ArgumentNullException();

            #endregion

            lock (_syncRoot)
            {
                // ServiceHostBase
                if (_resourceDictionary.ContainsKey(serviceHost) == false)
                {
                    _resourceDictionary.Add(serviceHost, new List<object>());
                }

                // Resource
                List<object> resourceList = _resourceDictionary[serviceHost];
                if (resourceList.Contains(resource) == false)
                {
                    resourceList.Add(resource);
                }
            }
        }

        public void DetachResource(ServiceHostBase serviceHost, object resource)
        {
            #region Contracts

            if (serviceHost == null) throw new ArgumentNullException();
            if (resource == null) throw new ArgumentNullException();

            #endregion

            lock (_syncRoot)
            {
                // ServiceHostBase
                if (_resourceDictionary.ContainsKey(serviceHost) == false)
                {
                    return;
                }

                // Resource
                List<object> resourceList = _resourceDictionary[serviceHost];
                if (resourceList.Contains(resource) == false)
                {
                    resourceList.Remove(resource);
                }

                if (resourceList.Count == 0)
                {
                    _resourceDictionary.Remove(serviceHost);
                }
            }
        }

        public TResource GetResource<TResource>(ServiceHostBase serviceHost)
            where TResource : class
        {
            #region Contracts

            if (serviceHost == null) throw new ArgumentNullException();

            #endregion

            lock (_syncRoot)
            {
                // Result
                TResource resource = null;

                // Search 
                if (_resourceDictionary.ContainsKey(serviceHost) == true)
                {
                    foreach (object existResource in _resourceDictionary[serviceHost])
                    {
                        resource = existResource as TResource;
                        if (resource != null) break;
                    }
                }

                // Return
                return resource;
            }
        }
    }
}
