using CLK.Threading.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CLK.ComponentModel.Observing
{
    public class ThreadSafeObservableCollection<T> : ThreadSafeEnumerable<T>, IObservableCollection<T>
    {
        // Fields         
        private readonly IObservableCollection<T> _component = null;

        private readonly bool _isAutomatic = true;


        private readonly object _syncRoot = new object();

        private readonly List<PropertyChangedEventArgs> _propertyChangedEventCollection = new List<PropertyChangedEventArgs>();

        private readonly List<NotifyCollectionChangedEventArgs> _collectionChangedEventCollection = new List<NotifyCollectionChangedEventArgs>();


        // Constructors
        public ThreadSafeObservableCollection(bool isAutomatic = true) : this(new NativeObservableCollection<T>(), isAutomatic) { }

        protected ThreadSafeObservableCollection(IObservableCollection<T> component, bool isAutomatic = true)
            : base(component)
        {
            #region Contracts

            if (component == null) throw new ArgumentNullException();

            #endregion

            // Component
            _component = component;

            // IsAutomatic
            _isAutomatic = isAutomatic;

            // Event
            ((INotifyPropertyChanged)_component).PropertyChanged += this.ObservableCollection_PropertyChanged;
            ((INotifyCollectionChanged)_component).CollectionChanged += this.ObservableCollection_CollectionChanged;     
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
                    return ((ICollection<T>)_component).IsReadOnly;
                }
                finally
                {
                    this.ExitReadLock();
                }
            }
        }

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
                    this.AutomaticRaiseEvent();
                }
            }
        }


        // Methods               
        public void RaiseEvent()
        {
            // Result
            IEnumerable<PropertyChangedEventArgs> propertyChangedEventCollection = null;
            IEnumerable<NotifyCollectionChangedEventArgs> collectionChangedEventCollection = null;

            // Get
            lock (_syncRoot)
            {
                // Require
                if (_propertyChangedEventCollection.Count == 0 && _collectionChangedEventCollection.Count == 0) return;

                // Detach
                propertyChangedEventCollection = _propertyChangedEventCollection.ToArray();
                collectionChangedEventCollection = _collectionChangedEventCollection.ToArray();
                _propertyChangedEventCollection.Clear();
                _collectionChangedEventCollection.Clear();
            }

            // Notify
            foreach (PropertyChangedEventArgs propertyChangedEventArgs in propertyChangedEventCollection)
            {
                this.OnPropertyChanged(this, propertyChangedEventArgs);
            }

            foreach (NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs in collectionChangedEventCollection)
            {
                this.OnCollectionChanged(this, notifyCollectionChangedEventArgs);
            }
        }

        private void AutomaticRaiseEvent()
        {
            // Require
            if (_isAutomatic == false) return;

            // RaiseEvent
            this.RaiseEvent();
        }        


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
                this.AutomaticRaiseEvent();
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
                this.AutomaticRaiseEvent();
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
                this.AutomaticRaiseEvent();
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
                this.AutomaticRaiseEvent();
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
                this.AutomaticRaiseEvent();
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


        // Handlers
        private void ObservableCollection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            #region Contracts

            if (sender == null) throw new ArgumentNullException();
            if (e == null) throw new ArgumentNullException();

            #endregion

            // Attach
            lock (_syncRoot)
            {
                // Add
                _propertyChangedEventCollection.Add(e);
            }
        }

        private void ObservableCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            #region Contracts

            if (sender == null) throw new ArgumentNullException();
            if (e == null) throw new ArgumentNullException();

            #endregion

            // Attach
            lock (_syncRoot)
            {
                // Add
                _collectionChangedEventCollection.Add(e);
            }
        }

        
        // Events
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            #region Contracts

            if (sender == null) throw new ArgumentNullException();
            if (e == null) throw new ArgumentNullException();

            #endregion

            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            #region Contracts

            if (sender == null) throw new ArgumentNullException();
            if (e == null) throw new ArgumentNullException();

            #endregion

            var handler = this.CollectionChanged;
            if (handler != null)
            {
                try
                {
                    handler(this, e);
                }
                catch (Exception)
                {
                    handler(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }
    }
}
