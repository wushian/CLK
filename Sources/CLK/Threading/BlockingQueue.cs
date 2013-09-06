using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CLK.Threading
{
    public sealed class BlockingQueue<T>
    {
        // Fields        
        private readonly object _syncRoot = new object();

        private readonly Queue<T> _queue = null;

        private readonly ManualResetEvent _stuffEvent = null;

        private readonly ManualResetEvent _releaseEvent = null;

        private readonly WaitHandle[] _waitHandles = null;


        // Constructors
        public BlockingQueue()
        {
            // Default 
            _queue = new Queue<T>();
            _stuffEvent = new ManualResetEvent(false);
            _releaseEvent = new ManualResetEvent(false);
            _waitHandles = new WaitHandle[] { _stuffEvent, _releaseEvent };
        }


        // Methods
        public void Enqueue(T item)
        {
            // Enqueue
            lock (_syncRoot)
            {
                // Queue
                _queue.Enqueue(item);

                // Flag
                _stuffEvent.Set();
            }
        }

        public T Dequeue()
        {
            // Wait
            WaitHandle.WaitAny(_waitHandles);

            // Dequeue
            T item = default(T);
            lock (_syncRoot)
            {
                // Queue
                if (_queue.Count > 0) item = _queue.Dequeue();

                // Flag
                if (_queue.Count <= 0) _stuffEvent.Reset();
            }

            // Return
            return item;
        }


        public void Release()
        {
            // Release
            lock (_syncRoot)
            {
                // Flag
                _releaseEvent.Set();
            }
        }

        public void Reset()
        {
            // Reset
            lock (_syncRoot)
            {
                // Queue
                _queue.Clear();

                // Flag
                _stuffEvent.Reset();
                _releaseEvent.Reset();                
            }
        }
    }
}