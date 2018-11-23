using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Web.Mvc
{
    internal sealed partial class ParameterInfoFactory
    {
        // Singleton
        private static object _syncRoot = new object();

        private static ParameterInfoFactory _instance = null;

        public static ParameterInfoFactory Current
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
                            _instance = new ParameterInfoFactory();
                        }
                    }
                }

                // Return
                return _instance;
            }
        }
    }

    internal sealed partial class ParameterInfoFactory
    {
        // Fields
        private static Dictionary<Type, Dictionary<string, ParameterInfo[]>> _parameterInfoArrayCache = new Dictionary<Type, Dictionary<string, ParameterInfo[]>>();


        // Constructors
        private ParameterInfoFactory()
        {

        }


        // Methods
        public ParameterInfo[] GetAll(Type type, MethodInfo methodInfo)
        {
            #region Contracts

            if (type == null) throw new ArgumentNullException();
            if (methodInfo == null) throw new ArgumentNullException();

            #endregion

            // MethodInfoName
            var methodInfoName = methodInfo.ToString();
            if (string.IsNullOrEmpty(methodInfoName) == true) throw new InvalidOperationException();

            // Cache
            if (_parameterInfoArrayCache.ContainsKey(type) == true)
            {
                if (_parameterInfoArrayCache[type].ContainsKey(methodInfoName) == true)
                {
                    return _parameterInfoArrayCache[type][methodInfoName];
                }
            }

            // Create
            lock (_syncRoot)
            {
                if (_parameterInfoArrayCache.ContainsKey(type) == false)
                {
                    _parameterInfoArrayCache.Add(type, new Dictionary<string, ParameterInfo[]>());
                }

                if (_parameterInfoArrayCache[type].ContainsKey(methodInfoName) == false)
                {
                    // ParameterInfoArray
                    var parameterInfoArray = methodInfo.GetParameters();
                    if (parameterInfoArray == null) throw new InvalidOperationException();

                    // Cache
                    _parameterInfoArrayCache[type].Add(methodInfoName, parameterInfoArray);
                }
            }

            // Return
            return _parameterInfoArrayCache[type][methodInfoName];
        }
    }
}
