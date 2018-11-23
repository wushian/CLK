using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceApplication.Authentication
{
    public sealed class AuthenticationProperties
    {
        // Constants
        private const string ExpireTimeKey = ".expireTime";


        // Constructors
        public AuthenticationProperties() : this(new Dictionary<string, string>()) { }

        public AuthenticationProperties(IDictionary<string, string> items)
        {
            #region Contracts

            if (items == null) throw new ArgumentException();

            #endregion

            // Default
            this.Items = items;
        }


        // Properties
        public IDictionary<string, string> Items { get; private set; }

        public DateTime ExpireTime
        {
            get
            {
                return AuthenticationPropertiesHelper.GetExpireTime(this.Items);
            }
            set
            {
                AuthenticationPropertiesHelper.SetExpireTime(this.Items, value);
            }                       
        }
    }
}
