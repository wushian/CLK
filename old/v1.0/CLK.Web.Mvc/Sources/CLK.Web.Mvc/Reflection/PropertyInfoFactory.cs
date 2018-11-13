using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Web.Mvc
{
    internal sealed partial class PropertyInfoFactory
    {
        // Singleton
        private static object _syncRoot = new object();

        private static PropertyInfoFactory _instance = null;

        public static PropertyInfoFactory Current
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
                            _instance = new PropertyInfoFactory();
                        }
                    }
                }

                // Return
                return _instance;
            }
        }
    }

    internal sealed partial class PropertyInfoFactory
    {
        // Fields
        private static Dictionary<Type, PropertyInfo[]> _propertyInfoArrayCache = new Dictionary<Type, PropertyInfo[]>();


        // Constructors
        private PropertyInfoFactory()
        {

        }


        // Methods
        public PropertyInfo[] GetAll(Type type)
        {
            #region Contracts

            if (type == null) throw new ArgumentNullException();

            #endregion

            // Cache
            if (_propertyInfoArrayCache.ContainsKey(type) == true)
            {
                return _propertyInfoArrayCache[type];
            }

            // Create
            lock (_syncRoot)
            {
                if (_propertyInfoArrayCache.ContainsKey(type) == false)
                {
                    // GetProperties
                    var propertyInfoArray = type.GetProperties();
                    if (propertyInfoArray == null) throw new InvalidOperationException();

                    // Cache
                    _propertyInfoArrayCache.Add(type, propertyInfoArray);
                }
            }

            // Return
            return _propertyInfoArrayCache[type];
        }
    }
}
