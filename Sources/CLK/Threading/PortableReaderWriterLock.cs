using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CLK.Threading
{
    public sealed class PortableReaderWriterLock
    {
        // Fields
        private readonly object _readRoot = new object();

        private readonly object _writeRoot = new object();


        // Methods
        public void EnterReadLock()
        {
            Monitor.Enter(_readRoot);
        }

        public void ExitReadLock()
        {
            Monitor.Exit(_readRoot);
        }


        public void EnterWriteLock()
        {
            Monitor.Enter(_writeRoot);
            Monitor.Enter(_readRoot);
        }

        public void ExitWriteLock()
        {
            Monitor.Exit(_readRoot);
            Monitor.Exit(_writeRoot);
        }
    }
}
