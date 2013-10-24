using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CLK.Threading
{
    public class STASynchronizationContext : SynchronizationContext, IDisposable
    {
        // Fields
        private readonly STAThread _staThread = null;


        // Constructors
        public STASynchronizationContext()
        {
            // STAThread
            _staThread = new STAThread();
            _staThread.Start();
        }

        public STASynchronizationContext(STAThread staThread)
        {
            #region Contracts

            if (staThread == null) throw new ArgumentNullException();

            #endregion

            // STAThread
            _staThread = staThread;
        }

        public void Dispose()
        {
            // STAThread
            _staThread.Stop();
        }


        // Methods
        public override void Post(SendOrPostCallback callback, object state)
        {
            #region Contracts

            if (callback == null) throw new ArgumentNullException();

            #endregion

            // STAThread
            _staThread.Post(callback, state);
        }

        public override void Send(SendOrPostCallback callback, object state)
        {
            #region Contracts

            if (callback == null) throw new ArgumentNullException();

            #endregion

            // STAThread
            _staThread.Send(callback, state);
        }

        public override SynchronizationContext CreateCopy()
        {
            return this;
        }
    }
}
