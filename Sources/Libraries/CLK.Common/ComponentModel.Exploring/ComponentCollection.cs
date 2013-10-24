using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;
using CLK.Collections.Generic;

namespace CLK.ComponentModel.Exploring
{
    internal sealed class ComponentCollection<TComponent> : IEnumerable, IEnumerable<TComponent>
    {
        // Fields
        private readonly ThreadSafeComponentCollection _componentCollection = null;        


        // Constructors
        public ComponentCollection(Func<TComponent, TComponent, bool> equalComponentDelegate)
        {
            #region Contracts

            if (equalComponentDelegate == null) throw new ArgumentNullException();

            #endregion

            // ComponentCollection
            _componentCollection = new ThreadSafeComponentCollection(equalComponentDelegate);
        }
        

        // Methods
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _componentCollection.GetEnumerator();
        }

        public IEnumerator<TComponent> GetEnumerator()
        {
            return _componentCollection.GetEnumerator();
        }

        public TComponent Attach(TComponent item)
        {
            #region Contracts

            if (item == null) throw new ArgumentNullException();

            #endregion

            return _componentCollection.Attach(item);
        }

        public TComponent Detach(TComponent item)
        {
            #region Contracts

            if (item == null) throw new ArgumentNullException();

            #endregion

            return _componentCollection.Detach(item);
        }


        // Class
        private sealed class ThreadSafeComponentCollection : ThreadSafeCollection<TComponent>
        {
            // Fields
            private readonly Func<TComponent, TComponent, bool> _equalComponentDelegate = null;


            // Constructors
            public ThreadSafeComponentCollection(Func<TComponent, TComponent, bool> equalComponentDelegate)
            {
                #region Contracts

                if (equalComponentDelegate == null) throw new ArgumentNullException();

                #endregion

                // Arguments
                _equalComponentDelegate = equalComponentDelegate;
            }


            // Methods
            public TComponent Attach(TComponent item)
            {
                #region Contracts

                if (item == null) throw new ArgumentNullException();

                #endregion

                try
                {
                    // Enter
                    this.EnterWriteLock();

                    // Search 
                    TComponent attachComponent = item;
                    foreach (TComponent existComponent in this)
                    {
                        if (_equalComponentDelegate(item, existComponent)==true)
                        {
                            attachComponent = default(TComponent); 
                            break;
                        }
                    }
                    if (attachComponent == null) return default(TComponent);

                    // Add
                    this.Add(attachComponent);

                    // Return
                    return attachComponent;
                }
                finally
                {
                    // Exit
                    this.ExitWriteLock();
                }
            }

            public TComponent Detach(TComponent item)
            {
                #region Contracts

                if (item == null) throw new ArgumentNullException();

                #endregion

                try
                {
                    // Enter
                    this.EnterWriteLock();

                    // Search 
                    TComponent detachComponent = default(TComponent);
                    foreach (TComponent existComponent in this)
                    {
                        if (_equalComponentDelegate(item, existComponent) == true)
                        {
                            detachComponent = existComponent;
                            break;
                        }
                    }
                    if (detachComponent == null) return default(TComponent);

                    // Remove
                    this.Remove(detachComponent);

                    // Return
                    return detachComponent;
                }
                finally
                {
                    // Exit
                    this.ExitWriteLock();
                }
            }
        }
    }
}
