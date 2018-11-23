using CLK.ServiceApplication.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceApplication.Authentication
{
    public class AuthenticationTicket
    {
        // Constructors
        public AuthenticationTicket(User user) : this(user, new AuthenticationProperties()) { }

        public AuthenticationTicket(User user, AuthenticationProperties properties)
        {
            #region Contracts

            if (user == null) throw new ArgumentException();
            if (properties == null) throw new ArgumentException();

            #endregion

            // Default
            this.User = user;
            this.Properties = properties;
        }


        // Properties
        public User User { get; private set; }

        public AuthenticationProperties Properties { get; private set; }


        public DateTime ExpireTime
        {
            get
            {
                return this.Properties.ExpireTime;
            }
        }
    }
}
