using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CLK.Threading;

namespace CLK.Collections.Concurrent
{
    public class ThreadSafeEnumerable<T> : IEnumerable<T>, IDisposable
    {
        // Fields         
        private readonly PortableReaderWriterLock _readerWriterLock = new PortableReaderWriterLock();

        private readonly IEnumerable<T> _component = null;       


        private readonly object _syncRoot = new object();

        private bool _disposed = false;


        // Constructors
        protected ThreadSafeEnumerable(IEnumerable<T> component)
        {
            #region Contracts
           
            if (component == null) throw new ArgumentNullException();

            #endregion

            // Component
            _component = component;            
        }

        public void Dispose()
        {
            // Require
            lock (_syncRoot)
            {
                if (_disposed == true) return;
                _disposed = true;
            }

            // Component
            IDisposable disposable = _component as IDisposable;
            if (disposable != null) disposable.Dispose();
        }


        // Methods
        protected void EnterReadLock()
        {
            _readerWriterLock.EnterReadLock();
        }

        protected void ExitReadLock()
        {
            _readerWriterLock.ExitReadLock();
        }

        protected void EnterWriteLock()
        {
            _readerWriterLock.EnterWriteLock();
        }

        protected void ExitWriteLock()
        {
            _readerWriterLock.ExitWriteLock();
        }


        public IEnumerator<T> GetEnumerator()
        {
            return new ThreadSafeEnumerator<T>(_readerWriterLock, _component.GetEnumerator);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
