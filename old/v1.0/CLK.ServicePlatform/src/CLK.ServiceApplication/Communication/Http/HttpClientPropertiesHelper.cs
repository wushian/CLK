using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceApplication.Communication.Http
{
    public static class HttpClientPropertiesHelper
    {
        // Constants
        private const string ModulePrefix = ".Communication.Http";

        private const string TokenKey = ".Token";


        // Methods
        private static string GetTokenKey()
        {
            // Return
            return ModulePrefix + TokenKey;
        }

        public static string GetToken(IDictionary<string, string> items)
        {
            #region Contracts

            if (items == null) throw new ArgumentException();

            #endregion

            // Result
            string token = null;

            // TokenKey
            string tokenKey = HttpClientPropertiesHelper.GetTokenKey();

            // Get
            if (items.ContainsKey(tokenKey) == true)
            {
                // Token
                token = items[tokenKey];
            }

            // Return
            return token;
        }

        public static void SetToken(IDictionary<string, string> items, string token)
        {
            #region Contracts

            if (string.IsNullOrEmpty(token) == true) throw new ArgumentException();

            #endregion

            // TokenKey
            string tokenKey = HttpClientPropertiesHelper.GetTokenKey();

            // Set
            items[tokenKey] = token;
        }
    }
}
