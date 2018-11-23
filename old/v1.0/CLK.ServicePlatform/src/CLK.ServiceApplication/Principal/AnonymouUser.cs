using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceApplication.Principal
{
    internal class AnonymouUser : User
    {
        // Constructors
        public AnonymouUser()
        {
            // Default
            this.IsAuthenticated = false;
            this.Name = "";
        }


        // Properties
        public bool IsAuthenticated { get; private set; }

        public string Name { get; private set; }
    }
}
