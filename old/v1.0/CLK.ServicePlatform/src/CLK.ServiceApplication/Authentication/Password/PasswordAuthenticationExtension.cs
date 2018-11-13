using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceApplication.Authentication.Password
{
    public static class PasswordAuthenticationExtension
    {
        // Methods
        public static Task<AuthenticateResult> SignInAsync(this AuthenticationContext context, string id, string password)
        {
            // SignInAsync
            return context.Find<PasswordAuthenticationProvider>().SignInAsync(id, password);
        }
    }
}
