using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Web.Mvc
{
    internal sealed partial class MethodInfoFactory
    {
        // Singleton
        private static object _syncRoot = new object();

        private static MethodInfoFactory _instance = null;

        public static MethodInfoFactory Current
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
                            _instance = new MethodInfoFactory();
                        }
                    }
                }

                // Return
                return _instance;
            }
        }
    }

    internal sealed partial class MethodInfoFactory
    {
        // Fields
        private static Dictionary<Type, MethodInfo[]> _methodInfoArrayCache = new Dictionary<Type, MethodInfo[]>();


        // Constructors
        private MethodInfoFactory()
        {

        }


        // Methods
        public MethodInfo[] GetAll(Type type)
        {
            #region Contracts

            if (type == null) throw new ArgumentNullException();

            #endregion

            // Cache
            if (_methodInfoArrayCache.ContainsKey(type) == true)
            {
                return _methodInfoArrayCache[type];
            }

            // Create
            lock (_syncRoot)
            {
                if (_methodInfoArrayCache.ContainsKey(type) == false)
                {
                    // GetProperties
                    var methodInfoArray = type.GetMethods(BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public);
                    if (methodInfoArray == null) throw new InvalidOperationException();

                    // Cache
                    _methodInfoArrayCache.Add(type, methodInfoArray);
                }
            }

            // Return
            return _methodInfoArrayCache[type];
        }
    }
}
