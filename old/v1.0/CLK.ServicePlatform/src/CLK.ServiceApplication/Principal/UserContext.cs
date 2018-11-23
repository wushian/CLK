using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceApplication.Principal
{
    public class UserContext
    {
        // Fields
        private User _currentUser = null;


        // Constructors
        public UserContext()
        {
            // Default
            _currentUser = new AnonymouUser();
        }


        // Methods
        public User Current()
        {
            // Get
            return _currentUser;
        }

        public TUser Current<TUser>()where TUser : class, User
        {
            // Get
            return _currentUser as TUser;
        }


        public void SignIn(User user)
        {
            #region Contracts

            if (user == null) throw new ArgumentException();

            #endregion

            // SignIn
            _currentUser = user;
        }

        public void SignOut()
        {
            // Require
            if (_currentUser == null) return;

            // SignOut
            _currentUser = null;
        }
    }
}
