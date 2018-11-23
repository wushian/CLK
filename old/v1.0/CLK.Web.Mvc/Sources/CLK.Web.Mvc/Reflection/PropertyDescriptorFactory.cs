using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Web.Mvc
{
    internal sealed partial class PropertyDescriptorFactory
    {
        // Singleton
        private static object _syncRoot = new object();

        private static PropertyDescriptorFactory _instance = null;

        public static PropertyDescriptorFactory Current
        {
            get
            {
                // Create
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new PropertyDescriptorFactory();
                        }
                    }
                }

                // Return
                return _instance;
            }
        }
    }

    internal sealed partial class PropertyDescriptorFactory
    {
        // Fields
        private static Dictionary<Type, PropertyDescriptor[]> _propertyDescriptorArrayCache = new Dictionary<Type, PropertyDescriptor[]>();


        // Constructors
        private PropertyDescriptorFactory()
        {

        }


        // Methods
        public PropertyDescriptor[] GetAll(Type type)
        {
            #region Contracts

            if (type == null) throw new ArgumentNullException();

            #endregion

            // Cache
            if (_propertyDescriptorArrayCache.ContainsKey(type) == true)
            {
                return _propertyDescriptorArrayCache[type];
            }

            // Create
            lock (_syncRoot)
            {
                if (_propertyDescriptorArrayCache.ContainsKey(type) == false)
                {
                    // GetProperties
                    var propertyDescriptorArray = TypeDescriptor.GetProperties(type).OfType<PropertyDescriptor>().ToArray();
                    if (propertyDescriptorArray == null) throw new InvalidOperationException();

                    // Cache
                    _propertyDescriptorArrayCache.Add(type, propertyDescriptorArray);
                }
            }

            // Return
            return _propertyDescriptorArrayCache[type];
        }
    }
}
