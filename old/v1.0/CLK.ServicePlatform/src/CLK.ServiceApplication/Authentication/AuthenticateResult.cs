using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceApplication.Authentication
{
    public class AuthenticateResult
    {
        // Constructors
        private AuthenticateResult() { }


        // Properties
        public AuthenticationTicket Ticket { get; private set; }

        public Exception Failure { get; private set; }

        public bool Succeeded
        {
            get
            {
                return this.Ticket != null;
            }
        }


        // Methods
        public static AuthenticateResult Success(AuthenticationTicket ticket)
        {
            #region Contracts

            if (ticket == null) throw new ArgumentException();

            #endregion

            // Result
            var result = new AuthenticateResult();
            result.Ticket = ticket;
            result.Failure = null;

            // Return
            return result;
        }

        public static AuthenticateResult Fail(Exception failure)
        {
            #region Contracts

            if (failure == null) throw new ArgumentException();

            #endregion

            // Result
            var result = new AuthenticateResult();
            result.Ticket = null;
            result.Failure = failure;

            // Return
            return result;
        }

        public static AuthenticateResult Fail(string failureMessage)
        {
            #region Contracts

            if (string.IsNullOrEmpty(failureMessage) == true) throw new ArgumentException();

            #endregion

            // Result
            var result = new AuthenticateResult();
            result.Ticket = null;
            result.Failure = new Exception(failureMessage);

            // Return
            return result;
        }
    }
}
