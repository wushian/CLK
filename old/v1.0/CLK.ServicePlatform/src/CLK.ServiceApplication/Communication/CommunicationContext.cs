using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceApplication.Communication
{
    public class CommunicationContext
    {
        // Fields
        private readonly object _syncRoot = new object();

        private List<CommunicationClient> _clientList = new List<CommunicationClient>();

        private CommunicationProperties _currentProperties = null;


        // Methods
        public void SignIn(CommunicationProperties currentProperties)
        {
            #region Contracts

            if (currentProperties == null) throw new ArgumentException();

            #endregion

            // Sync
            lock (_syncRoot)
            {
                // Properties
                _currentProperties = currentProperties;

                // Client
                foreach(var client in _clientList)
                {
                    client.SignIn(currentProperties);
                }
            }
        }

        public void SignOut()
        {
            // Sync
            lock (_syncRoot)
            {
                // Require
                if (_currentProperties == null) return;

                // Properties                
                _currentProperties = null;

                // Client
                foreach (var client in _clientList)
                {
                    client.SignOut();
                }
            }
        }
        

        public void Attach(CommunicationClient client)
        {
            #region Contracts

            if (client == null) throw new ArgumentException();

            #endregion

            // Sync
            lock (_syncRoot)
            {
                // Events
                client.Disposed += this.CommunicationClient_Disposed;
                client.Unauthorized += this.CommunicationClient_Unauthorized;

                // SignIn
                var currentProperties = _currentProperties;
                if (currentProperties != null)
                {
                    client.SignIn(currentProperties);
                }

                // Add
                _clientList.Add(client);
            }          
        }

        private void Detach(CommunicationClient client)
        {
            #region Contracts

            if (client == null) throw new ArgumentException();

            #endregion

            // Sync
            lock (_syncRoot)
            {
                // Remove
                if (_clientList.Remove(client) == false) return;

                // SignOut
                client.SignOut();

                // Events
                client.Disposed -= this.CommunicationClient_Disposed;

                // Close
                client.Close();
            }
        }


        // Handlers
        private void CommunicationClient_Disposed(CommunicationClient client)
        {
            #region Contracts

            if (client == null) throw new ArgumentException();

            #endregion

            // Detach
            this.Detach(client);
        }

        private void CommunicationClient_Unauthorized()
        {
            // Notify
            this.OnUnauthorized();
        }


        // Events
        public event Action Unauthorized;
        private void OnUnauthorized()
        {
            // Raise
            var handler = this.Unauthorized;
            if (handler != null)
            {
                handler();
            }
        }
    }
}
