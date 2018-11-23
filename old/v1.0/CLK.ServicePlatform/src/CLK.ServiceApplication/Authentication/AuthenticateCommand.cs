using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceApplication.Authentication
{
    internal sealed class AuthenticateCommand
    {
        // Fields
        private Func<Task<AuthenticateResult>> _executeDelegate = null;


        // Constructors
        public AuthenticateCommand(Func<Task<AuthenticateResult>> executeDelegate)
        {
            #region Contracts

            if (executeDelegate == null) throw new ArgumentException();

            #endregion

            // Default
            _executeDelegate = executeDelegate;
        }


        // Methods
        public Task<AuthenticateResult> ExecuteAsync()
        {
            // Execute
            var result = _executeDelegate();
            if (result == null) throw new InvalidOperationException();

            // Return
            return result;
        }
    }
}
