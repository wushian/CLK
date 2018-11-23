using CLK.FrameClient.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLK.Promises;

namespace CLK.FrameClient.Transport.IdentityExtensions
{
    public class UnauthorizedHandler<TUser> : TransportHandler, IdentityHandler
        where TUser : IdentityUser
    {
        // Constants
        private const string AllowAnonymousHeaderKey = "Allow-Anonymous";


        // Fields
        private IdentityContext<TUser> _identityContext = null;


        // Constructors
        public UnauthorizedHandler(IdentityContext<TUser> identityContext)
        {
            #region Contracts

            if (identityContext == null) throw new ArgumentNullException();

            #endregion

            // Default
            _identityContext = identityContext;

            // Attach
            _identityContext.Attach(this);
        }


        // Methods
        public override ResultPromise<TransportResponse> Send(TransportRequest request)
        {
            #region Contracts

            if (request == null) throw new ArgumentNullException();

            #endregion

            // Result
            ResultPromise<TransportResponse> promise = new ResultPromise<TransportResponse>();

            // IsAuthenticated
            if(_identityContext.CurrentUser.IsAuthenticated() == false)
            {
                if (this.IsAllowAnonymous(request) == false)
                {
                    // Reject
                    promise.Reject(new UnauthorizedException());

                    // Return
                    return promise;
                }
            }

            // Send
            base.Send(request)

            // Success
            .Then(delegate (TransportResponse response)
            {
                // Resolve
                promise.Resolve(response);
            })

            // Fail
            .Fail(delegate (Exception error)
            {                
                try
                {
                    // Require
                    if (error is UnauthorizedException)
                    {
                        if (this.IsAllowAnonymous(request) == true)
                        {
                            error = new Exception(error.Message);
                        }
                    }

                    // Reject
                    promise.Reject(error);

                    // Notify
                    if (error is UnauthorizedException)
                    {
                        this.OnUnauthorized();
                    }
                }
                catch(Exception ex)
                {
                    // Reject
                    promise.Reject(ex);
                }
            });

            // Return
            return promise;
        }

        private bool IsAllowAnonymous(TransportRequest request)
        {
            #region Contracts

            if (request == null) throw new ArgumentNullException();

            #endregion

            // Require
            if(request.Headers.ContainsKey(AllowAnonymousHeaderKey) == true)
            {
                if (request.Headers[AllowAnonymousHeaderKey] == "true")
                {
                    return true;
                }
            }

            // Return
            return false;
        }


        // Events  
        public event Action Unauthorized;
        private void OnUnauthorized()
        {
            var handler = this.Unauthorized;
            if (handler != null)
            {
                handler();
            }
        }
    }
}
