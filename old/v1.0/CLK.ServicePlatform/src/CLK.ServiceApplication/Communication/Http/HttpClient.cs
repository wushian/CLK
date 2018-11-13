using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceApplication.Communication.Http
{
    public class HttpClient : CommunicationClient
    {
        // Fields
        private readonly object _syncRoot = new object();

        private string _currentToken = null;
        

        // Constructors
        internal HttpClient() 
        {

        }

        protected internal override void Close()
        {
            
        }


        // Methods
        internal protected override void SignIn(CommunicationProperties properties)
        {
            #region Contracts

            if (properties == null) throw new ArgumentException();

            #endregion

            // Properties
            string token = HttpClientPropertiesHelper.GetToken(properties.Items);

            // SignIn
            this.SignIn(token);
        }

        private void SignIn(string token)
        {
            #region Contracts

            if (string.IsNullOrEmpty(token) == true) throw new ArgumentException();

            #endregion

            // Sync
            lock (_syncRoot)
            {
                // SignIn
                _currentToken = token;
            }
        }

        internal protected override void SignOut()
        {
            // Sync
            lock (_syncRoot)
            {
                // Require
                if (_currentToken == null) return;

                // Properties                
                _currentToken = null;
            }
        }
    }
}
