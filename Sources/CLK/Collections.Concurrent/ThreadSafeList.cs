using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Collections.Concurrent
{
    public class ThreadSafeList<T> : ThreadSafeCollection<T>, IList<T>
    {
        // Fields         
        private readonly IList<T> _component = null;


        // Constructors
        public ThreadSafeList() : this(new List<T>()) { }

        protected ThreadSafeList(IList<T> component)
            : base(component)
        {
            #region Contracts

            if (component == null) throw new ArgumentNullException();

            #endregion

            // Component
            _component = component;
        }


        // Properties
        public T this[int index]
        {
            get
            {
                try
                {
                    this.EnterReadLock();
                    return _component[index];
                }
                finally
                {
                    this.ExitReadLock();
                }
            }
            set
            {
                try
                {
                    this.EnterWriteLock();
                    _component[index] = value;
                }
                finally
                {
                    this.ExitWriteLock();
                }
            }
        }


        // Methods
        public void Insert(int index, T item)
        {
            #region Contracts

            if (item == null) throw new ArgumentNullException();

            #endregion

            try
            {
                this.EnterWriteLock();
                _component.Insert(index, item);
            }
            finally
            {
                this.ExitWriteLock();
            }
        }

        public void RemoveAt(int index)
        {
            try
            {
                this.EnterWriteLock();
                _component.RemoveAt(index);
            }
            finally
            {
                this.ExitWriteLock();
            }
        }


        public int IndexOf(T item)
        {
            #region Contracts

            if (item == null) throw new ArgumentNullException();

            #endregion

            try
            {
                this.EnterReadLock();
                return _component.IndexOf(item);
            }
            finally
            {
                this.ExitReadLock();
            }
        }
    }
}
