using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceApplication.Authentication
{
    public class AuthenticationContext
    {
        // Fields
        private List<AuthenticationProvider> _providerList = new List<AuthenticationProvider>();

        private AuthenticationTicketRepository _ticketRepository = null;

        
        // Constructors
        public AuthenticationContext(AuthenticationTicketRepository ticketRepository)
        {
            #region Contracts
            
            if (ticketRepository == null) throw new ArgumentException();

            #endregion

            // Default
            _ticketRepository = ticketRepository;
        }


        // Methods
        internal Task<AuthenticateResult> SignInAsync(AuthenticateCommand command)
        {
            #region Contracts

            if (command == null) throw new ArgumentException();

            #endregion

            // Execute
            return command.ExecuteAsync().ContinueWith<AuthenticateResult>((executeTask) => {

                // Result
                var executeResult = executeTask.Result;
                if (executeResult == null) throw new InvalidOperationException();
                if (executeResult.Succeeded == false) return executeResult;

                // Ticket
                var currentTicket = executeResult.Ticket;
                if (currentTicket == null) throw new InvalidOperationException();
                if (currentTicket.ExpireTime < DateTime.Now) throw new InvalidOperationException();

                // Save
                _ticketRepository.SetTicket(currentTicket);

                // Return
                return executeResult;
            });
        }

        public Task<AuthenticateResult> SignInAsync()
        {
            // Ticket
            var currentTicket = _ticketRepository.GetTicket();
            if (currentTicket == null) return Task.FromResult<AuthenticateResult>(AuthenticateResult.Fail(new Exception("Ticket not find.")));
            if (currentTicket.ExpireTime < DateTime.Now) return Task.FromResult<AuthenticateResult>(AuthenticateResult.Fail(new Exception("Ticket is expired.")));
            
            // Return
            var executeResult = AuthenticateResult.Success(currentTicket);
            return Task.FromResult<AuthenticateResult>(executeResult);
        }
                
        public void SignOut()
        {
            // Ticket
            var currentTicket = _ticketRepository.GetTicket();
            if (currentTicket == null) return;
            if (currentTicket.ExpireTime < DateTime.Now) return;

            // Remove
            _ticketRepository.RemoveTicket();
        }
                

        public void Attach(AuthenticationProvider provider)
        {
            #region Contracts

            if (provider == null) throw new ArgumentException();

            #endregion

            // Initialize
            provider.Initialize(this.SignInAsync);

            // Add
            _providerList.Add(provider);            
        }

        public TProvider Find<TProvider>()where TProvider: class
        {
            // Find
            foreach (var provider in _providerList)
            {
                var resultProvider = provider as TProvider;
                if (resultProvider != null)
                {
                    return resultProvider;
                }
            }

            // Exception
            throw new Exception("Provider not find.");
        }
    }
}
