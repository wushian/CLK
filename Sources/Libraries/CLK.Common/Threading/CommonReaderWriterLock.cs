using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CLK.Threading
{
    public class CommonReaderWriterLock : IDisposable
    {
        // Fields
        private readonly ReaderWriterLockSlim _readerWriterLock = null;

        private readonly CountdownEvent _countdownEvent = new CountdownEvent(1);


        // Constructors
        public CommonReaderWriterLock()
        {
            // Lock
            _readerWriterLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        public CommonReaderWriterLock(LockRecursionPolicy lockRecursionPolicy)
        {
            // Lock
            _readerWriterLock = new ReaderWriterLockSlim(lockRecursionPolicy);
        }

        public void Dispose()
        {
            // Wait
            _countdownEvent.Signal();
            _countdownEvent.Wait();

            // Dispose
            _countdownEvent.Dispose();
            _readerWriterLock.Dispose();
        }


        // Methods
        public void EnterReadLock()
        {
            _countdownEvent.AddCount();
            _readerWriterLock.EnterReadLock();
        }

        public void ExitReadLock()
        {
            _readerWriterLock.ExitReadLock();
            _countdownEvent.Signal();
        }

        public void EnterWriteLock()
        {
            _countdownEvent.AddCount();
            _readerWriterLock.EnterWriteLock();
        }

        public void ExitWriteLock()
        {
            _readerWriterLock.ExitWriteLock();
            _countdownEvent.Signal();
        }
    }
}
