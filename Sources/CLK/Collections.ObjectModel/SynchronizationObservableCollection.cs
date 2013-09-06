using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace CLK.Collections.ObjectModel
{
    public class SynchronizationObservableCollection<T> : IObservableCollection<T>
    {
        // Fields         
        private readonly IObservableCollection<T> _component = null;

        private SynchronizationContext _syncContext = null;


        // Constructors
        public SynchronizationObservableCollection() : this(new ThreadSafeObservableCollection<T>()) { }

        public SynchronizationObservableCollection(IObservableCollection<T> component)
        {
            #region Contracts

            if (component == null) throw new ArgumentNullException();

            #endregion

            // Component
            _component = component;

            // Event
            ((INotifyPropertyChanged)_component).PropertyChanged += this.ObservableCollection_PropertyChanged;
            ((INotifyCollectionChanged)_component).CollectionChanged += this.ObservableCollection_CollectionChanged;
        }


        // Properties
        public int Count
        {
            get
            {
                return _component.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((ICollection<T>)_component).IsReadOnly;
            }
        }

        public T this[int index]
        {
            get
            {
                return _component[index];
            }
            set
            {
                _component[index] = value;
            }
        }


        // Methods
        public void SetSynchronizationContext(SynchronizationContext syncContext)
        {
            #region Contracts

            if (syncContext == null) throw new ArgumentNullException();

            #endregion

            // SyncContext
            _syncContext = syncContext;
        }


        public IEnumerator<T> GetEnumerator()
        {
            return _component.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _component.GetEnumerator();
        }


        public void Add(T item)
        {
            _component.Add(item);
        }

        public bool Remove(T item)
        {
            return _component.Remove(item);
        }

        public void Clear()
        {
            _component.Clear();
        }


        public bool Contains(T item)
        {
            return _component.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _component.CopyTo(array, arrayIndex);
        }


        public void Insert(int index, T item)
        {
            _component.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _component.RemoveAt(index);
        }


        public int IndexOf(T item)
        {
            return _component.IndexOf(item);
        }


        // Handlers
        private void ObservableCollection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            #region Contracts

            if (sender == null) throw new ArgumentNullException();
            if (e == null) throw new ArgumentNullException();

            #endregion

            // Notify
            this.OnPropertyChanged(e);
        }

        private void ObservableCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            #region Contracts

            if (sender == null) throw new ArgumentNullException();
            if (e == null) throw new ArgumentNullException();

            #endregion

            // Notify
            this.OnCollectionChanged(e);
        }


        // Events
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            #region Contracts

            if (e == null) throw new ArgumentNullException();

            #endregion

            var handler = this.PropertyChanged;            
            if (handler != null)
            {
                var syncContext = _syncContext;
                if (syncContext != null)
                {
                    SendOrPostCallback syncDelegate = delegate(object state)
                    {
                        handler(this, e);
                    };
                    _syncContext.Post(syncDelegate, null);
                }                
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            #region Contracts

            if (e == null) throw new ArgumentNullException();

            #endregion

            var handler = this.CollectionChanged;
            if (handler != null)
            {
                var syncContext = _syncContext;
                if (syncContext != null)
                {
                    SendOrPostCallback syncDelegate = delegate(object state)
                    {
                        try
                        {
                            handler(this, e);
                        }
                        catch (Exception)
                        {
                            handler(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                        }
                    };
                    _syncContext.Post(syncDelegate, null);
                }                       
            }
        }
    }
}
