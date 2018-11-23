using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceApplication.Authentication.Password
{
    public abstract class PasswordAuthenticationProvider : AuthenticationProvider
    {
        // Methods
        public Task<AuthenticateResult> SignInAsync(string id, string password)
        {
            #region Contracts

            if (string.IsNullOrEmpty(id) == true) throw new ArgumentException();
            if (string.IsNullOrEmpty(password) == true) throw new ArgumentException();

            #endregion
            
            // Execute
            return this.ExecuteAsync(() =>
            {
                // Authenticate
                return this.AuthenticateAsync(id, password);
            });
        }

        protected abstract Task<AuthenticateResult> AuthenticateAsync(string id, string password);
    }
}
