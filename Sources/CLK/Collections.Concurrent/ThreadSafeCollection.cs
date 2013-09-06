using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;

namespace CLK.Collections.Concurrent
{
    public class ThreadSafeCollection<T> : ThreadSafeEnumerable<T>, ICollection<T>
    {
        // Fields         
        private readonly ICollection<T> _component = null;


        // Constructors
        public ThreadSafeCollection() : this(new List<T>()) { }

        protected ThreadSafeCollection(ICollection<T> component) : base(component)
        {
            #region Contracts
           
            if (component == null) throw new ArgumentNullException();

            #endregion
            
            // Component
            _component = component;            
        }

        
        // Properties
        public int Count
        {
            get
            {
                try
                {
                    this.EnterReadLock();
                    return _component.Count;
                }
                finally
                {
                    this.ExitReadLock();
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                try
                {
                    this.EnterReadLock();
                    return _component.IsReadOnly;
                }
                finally
                {
                    this.ExitReadLock();
                }
            }
        }


        // Methods
        public void Add(T item)
        {
            try
            {
                this.EnterWriteLock();
                _component.Add(item);
            }
            finally
            {
                this.ExitWriteLock();
            }
        }

        public bool Remove(T item)
        {
            try
            {
                this.EnterWriteLock();
                return _component.Remove(item);
            }
            finally
            {
                this.ExitWriteLock();
            }
        }

        public void Clear()
        {
            try
            {
                this.EnterWriteLock();
                _component.Clear();
            }
            finally
            {
                this.ExitWriteLock();
            }
        }       


        public bool Contains(T item)
        {
            try
            {
                this.EnterReadLock();
                return _component.Contains(item);
            }
            finally
            {
                this.ExitReadLock();
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            try
            {
                this.EnterReadLock();
                _component.CopyTo(array, arrayIndex);
            }
            finally
            {
                this.ExitReadLock();
            }
        }        
    }
}
