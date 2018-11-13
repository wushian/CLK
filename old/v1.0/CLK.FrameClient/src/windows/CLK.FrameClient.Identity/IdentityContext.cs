using CLK.Promises;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.FrameClient.Identity
{
    public abstract class IdentityContext<TUser> where TUser : IdentityUser
    {
        // Fields
        private IdentityRepository<TUser> _repository = null;

        private List<IdentityHandler> _handlers = null;

        private TUser _currentUser = null;

        private TUser _defaultUser = null;


        // Constructors
        public IdentityContext(IdentityRepository<TUser> repository)
        {
            #region Contracts

            if (repository == null) throw new ArgumentNullException();

            #endregion

            // Default
            _repository = repository;
            _handlers = new List<IdentityHandler>();
        }

        public IdentityContext(IdentityRepository<TUser> repository, IdentityHandler handler)
        {
            #region Contracts

            if (repository == null) throw new ArgumentNullException();
            if (handler == null) throw new ArgumentNullException();

            #endregion

            // Default
            _repository = repository;
            _handlers = new List<IdentityHandler>();

            // Attach
            this.Attach(handler);
        }


        // Properties
        public TUser CurrentUser
        {
            get
            {
                if (_currentUser == null)
                {
                    return this.DefaultUser;
                }
                return _currentUser;
            }
        }

        private TUser DefaultUser
        {
            get
            {
                if (_defaultUser == null)
                {
                    _defaultUser = this.CreateDefaultUser();
                    if (_defaultUser == null) throw new InvalidOperationException();
                }
                return _defaultUser;
            }
        }


        // Methods
        public void Attach(IdentityHandler handler)
        {
            #region Contracts

            if (handler == null) throw new ArgumentNullException();

            #endregion

            // Require
            if (_handlers.Contains(handler) == true) return;

            // Handlers
            handler.Unauthorized += this.IdentityHandler_Unauthorized;

            // Attach
            _handlers.Add(handler);
        }

        public void Detach(IdentityHandler handler)
        {
            #region Contracts

            if (handler == null) throw new ArgumentNullException();

            #endregion

            // Require
            if (_handlers.Contains(handler) == false) return;

            // Handlers
            handler.Unauthorized -= this.IdentityHandler_Unauthorized;

            // Detach
            _handlers.Remove(handler);
        }


        public Promise SignInAsync()
        {
            // Require
            if (_currentUser != null)
            {
                Promise promise = new Promise();
                promise.Resolve();
                return promise;
            }

            // Repository
            return _repository.GetUserAsync()

            // Sign
            .ThenPromise(delegate (TUser user)
            {
                // IsAuthenticated
                if (user != null)
                {
                    if (user.IsAuthenticated() == false)
                    {
                        user = null;
                    }
                }

                // Sign
                if (user != null)
                {
                    // SignIn
                    return this.SignInAsync(user);
                }
                else
                {
                    // SignOut
                    return this.SignOutAsync();
                }
            });
        }

        public Promise SignInAsync(TUser user)
        {
            #region Contracts

            if (user == null) throw new ArgumentNullException();

            #endregion

            // Default
            _currentUser = user;

            // Repository
            return _repository.SetUserAsync(user)

            // Notify
            .Then(delegate ()
            {
                this.OnAuthorized(user);
            });
        }

        public Promise SignOutAsync()
        {
            // Default
            _currentUser = null;

            // Repository
            return _repository.RemoveUserAsync()

            // Notify
            .Then(delegate ()
            {
                this.OnUnauthorized();
            });            
        }


        protected abstract TUser CreateDefaultUser();


        // Handlers
        private void IdentityHandler_Unauthorized()
        {
            // SignOut
            this.SignOutAsync();
        }


        // Events  
        public event Action<TUser> Authorized;
        private void OnAuthorized(TUser user)
        {
            #region Contracts

            if (user == null) throw new ArgumentNullException();

            #endregion

            var handler = this.Authorized;
            if (handler != null)
            {
                handler(user);
            }
        }

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
