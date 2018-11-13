using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceApplication.Authentication
{
    public static class AuthenticationPropertiesHelper
    {
        // Constants
        private const string ModulePrefix = ".Authentication.";

        private const string ExpireTimeKey = ".ExpireTime";


        // Methods
        private static string GetExpireTimeKey()
        {
            // Return
            return ModulePrefix + ExpireTimeKey;
        }

        public static DateTime GetExpireTime(IDictionary<string, string> items)
        {
            #region Contracts

            if (items == null) throw new ArgumentException();

            #endregion

            // Result
            DateTime expireTime = DateTime.MaxValue;

            // ExpireTimeKey
            string expireTimeKey = AuthenticationPropertiesHelper.GetExpireTimeKey();

            // Get
            if (items.ContainsKey(expireTimeKey) == true)
            {
                // ExpireTimeString
                string expireTimeString = items[expireTimeKey];

                // ExpireTime
                if (DateTime.TryParseExact(expireTimeString, "yyyy/MM/dd HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, out expireTime) == false)
                {
                    expireTime = DateTime.MaxValue;
                }
            }

            // Return
            return expireTime;
        }

        public static void SetExpireTime(IDictionary<string, string> items, DateTime expireTime)
        {
            #region Contracts

            if (items == null) throw new ArgumentException();

            #endregion

            // ExpireTimeKey
            string expireTimeKey = AuthenticationPropertiesHelper.GetExpireTimeKey();

            // ExpireTimeString
            string expireTimeString = expireTime.ToString("yyyy/MM/dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

            // Set
            items[expireTimeKey] = expireTimeString;
        }
    }
}
