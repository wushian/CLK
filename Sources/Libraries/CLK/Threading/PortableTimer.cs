using CLK.Diagnostics;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CLK.Threading
{
    public sealed class PortableTimer : IDisposable
    {
        // Fields
        private readonly ManualResetEvent _executeThreadEvent = new ManualResetEvent(false);

        private readonly Action _callback = null;

        private readonly int _interval = 0;


        // Constructors
        public PortableTimer(Action callback, int interval)
        {
            #region Contracts

            if (callback == null) throw new ArgumentNullException();

            #endregion

            // Require
            if (interval <= 0) throw new ArgumentException();

            // Arguments
            _callback = callback;
            _interval = interval;

            // Begin
            Task.Factory.StartNew(this.Execute);
        }

        public void Dispose()
        {
            // End
            _executeThreadEvent.Set();
        }


        // Methods
        private void Execute()
        {
            while (true)
            {
                // Wait
                if (_executeThreadEvent.WaitOne(_interval) == true)
                {
                    return;
                }

                // Execute
                try
                {
                    // Callback
                    _callback();
                }
                catch (Exception ex)
                {
                    // Fail
                    DebugContext.Current.Fail(string.Format("Action:{0}, State:{1}, Message:{2}", "Callback", "Exception", ex.Message));
                }
            }
        }
    }
}
