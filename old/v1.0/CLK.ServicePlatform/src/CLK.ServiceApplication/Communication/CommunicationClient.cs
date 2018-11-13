using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceApplication.Communication
{
    public abstract class CommunicationClient : IDisposable
    {
        // Constructors
        public CommunicationClient()
        {

        }

        public void Dispose()
        {
            // Notify
            this.OnDisposed(this);
        }

        internal protected abstract void Close();


        // Methods
        internal protected abstract void SignIn(CommunicationProperties properties);

        internal protected abstract void SignOut();


        // Events        
        internal event Action<CommunicationClient> Disposed;
        private void OnDisposed(CommunicationClient client)
        {
            #region Contracts

            if (client == null) throw new ArgumentException();

            #endregion

            // Raise
            var handler = this.Disposed;
            if (handler != null)
            {
                handler(client);
            }
        }

        public event Action Unauthorized;
        protected void OnUnauthorized()
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
