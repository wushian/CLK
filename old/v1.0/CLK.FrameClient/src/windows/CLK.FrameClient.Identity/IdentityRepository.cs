using CLK.Promises;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.FrameClient.Identity
{
    public interface IdentityRepository<TUser> where TUser : IdentityUser
    {
        // Methods
        ResultPromise<TUser> GetUserAsync();

        Promise SetUserAsync(TUser user);

        Promise RemoveUserAsync();
    }
}
