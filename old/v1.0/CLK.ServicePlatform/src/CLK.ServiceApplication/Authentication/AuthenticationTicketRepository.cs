using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceApplication.Authentication
{
    public interface AuthenticationTicketRepository
    {
        // Methods
        void SetTicket(AuthenticationTicket ticket);

        AuthenticationTicket GetTicket();

        void RemoveTicket();
    }
}
