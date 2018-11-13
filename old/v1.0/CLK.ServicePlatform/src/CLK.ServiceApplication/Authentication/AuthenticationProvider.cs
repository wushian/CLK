using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceApplication.Authentication
{
    public abstract class AuthenticationProvider
    {
        // Fields
        private Func<AuthenticateCommand, Task<AuthenticateResult>> _signInDelegate = null;


        // Constructors
        internal void Initialize(Func<AuthenticateCommand, Task<AuthenticateResult>> signInDelegate)
        {
            #region Contracts

            if (signInDelegate == null) throw new ArgumentException();

            #endregion

            // Default
            _signInDelegate = signInDelegate;
        }


        // Methods
        protected Task<AuthenticateResult> ExecuteAsync(Func<Task<AuthenticateResult>> executeDelegate)
        {
            #region Contracts

            if (executeDelegate == null) throw new ArgumentException();

            #endregion

            // Require
            if (_signInDelegate == null) throw new InvalidOperationException("SignInDelegate not find.");

            // Command
            var command = new AuthenticateCommand(executeDelegate);

            // SignIn
            var result = _signInDelegate(command);
            if (result == null) throw new InvalidOperationException();

            // Return
            return result;
        }
    }
}
