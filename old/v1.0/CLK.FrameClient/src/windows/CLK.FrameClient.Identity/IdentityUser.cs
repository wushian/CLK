using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.FrameClient.Identity
{
    public class IdentityUser
    {
        // Constructors
        public IdentityUser(string name, DateTime expires)
        {
            #region Contracts

            if (string.IsNullOrEmpty(name) == true) throw new ArgumentNullException();

            #endregion

            // Default
            this.Name = name;
            this.Expires = expires;
        }


        // Properties
        public string Name { get; }

        public DateTime Expires { get; }


        // Methods
        public bool IsAuthenticated()
        {
            // Expires
            if (DateTime.Now > this.Expires)
            {
                return false;
            }
            return true;
        }
    }
}
