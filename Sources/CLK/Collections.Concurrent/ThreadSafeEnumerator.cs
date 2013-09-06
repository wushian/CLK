using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLK.Threading;

namespace CLK.Collections.Concurrent
{
    internal sealed class ThreadSafeEnumerator<T> : IEnumerator<T>, IDisposable
    {
        // Fields         
        private readonly object _syncRoot = new object();

        private readonly PortableReaderWriterLock _readerWriterLock = null;

        private readonly IEnumerator<T> _component = null;                

        private bool _disposed = false;


        // Constructors
        public ThreadSafeEnumerator(PortableReaderWriterLock readerWriterLock, Func<IEnumerator<T>> getEnumeratorDelegate)
        {
            #region Contracts
                        
            if (readerWriterLock == null) throw new ArgumentNullException();
            if (getEnumeratorDelegate == null) throw new ArgumentNullException();

            #endregion

            // Lock
            _readerWriterLock = readerWriterLock;
            _readerWriterLock.EnterWriteLock();

            // Component
            _component = getEnumeratorDelegate();
            if (_component == null) throw new InvalidOperationException();
        }

        public void Dispose()
        {
            // Require
            lock (_syncRoot)
            {
                if (_disposed == true) return;
                _disposed = true;
            }

            // Dispose
            try
            {
                // Component
                _component.Dispose();
            }
            finally
            {
                // Lock            
                _readerWriterLock.ExitWriteLock();
            }
        }


        // Properties
        public T Current
        {
            get { return _component.Current; }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return this.Current; }
        }


        // Methods
        public bool MoveNext()
        {
            return _component.MoveNext();
        }

        public void Reset()
        {
            _component.Reset();
        }
    }
}
