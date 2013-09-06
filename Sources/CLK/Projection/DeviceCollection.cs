using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;
using CLK.Collections.Concurrent;

namespace CLK.Projection
{
    internal sealed class DeviceCollection<TDevice> : IEnumerable, IEnumerable<TDevice>
    {
        // Fields
        private readonly ThreadSafeDeviceCollection _deviceCollection = null;        


        // Constructors
        public DeviceCollection(Func<TDevice, TDevice, bool> equalDeviceDelegate)
        {
            #region Contracts

            if (equalDeviceDelegate == null) throw new ArgumentNullException();

            #endregion

            // DeviceCollection
            _deviceCollection = new ThreadSafeDeviceCollection(equalDeviceDelegate);
        }
        

        // Methods
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _deviceCollection.GetEnumerator();
        }

        public IEnumerator<TDevice> GetEnumerator()
        {
            return _deviceCollection.GetEnumerator();
        }

        public TDevice Attach(TDevice item)
        {
            #region Contracts

            if (item == null) throw new ArgumentNullException();

            #endregion

            return _deviceCollection.Attach(item);
        }

        public TDevice Detach(TDevice item)
        {
            #region Contracts

            if (item == null) throw new ArgumentNullException();

            #endregion

            return _deviceCollection.Detach(item);
        }


        // Class
        private sealed class ThreadSafeDeviceCollection : ThreadSafeCollection<TDevice>
        {
            // Fields
            private readonly Func<TDevice, TDevice, bool> _equalDeviceDelegate = null;


            // Constructors
            public ThreadSafeDeviceCollection(Func<TDevice, TDevice, bool> equalDeviceDelegate)
            {
                #region Contracts

                if (equalDeviceDelegate == null) throw new ArgumentNullException();

                #endregion

                // Arguments
                _equalDeviceDelegate = equalDeviceDelegate;
            }


            // Methods
            public TDevice Attach(TDevice item)
            {
                #region Contracts

                if (item == null) throw new ArgumentNullException();

                #endregion

                try
                {
                    // Enter
                    this.EnterWriteLock();

                    // Search 
                    TDevice attachDevice = item;
                    foreach (TDevice existDevice in this)
                    {
                        if (_equalDeviceDelegate(item, existDevice)==true)
                        {
                            attachDevice = default(TDevice); 
                            break;
                        }
                    }
                    if (attachDevice == null) return default(TDevice);

                    // Add
                    this.Add(attachDevice);

                    // Return
                    return attachDevice;
                }
                finally
                {
                    // Exit
                    this.ExitWriteLock();
                }
            }

            public TDevice Detach(TDevice item)
            {
                #region Contracts

                if (item == null) throw new ArgumentNullException();

                #endregion

                try
                {
                    // Enter
                    this.EnterWriteLock();

                    // Search 
                    TDevice detachDevice = default(TDevice);
                    foreach (TDevice existDevice in this)
                    {
                        if (_equalDeviceDelegate(item, existDevice) == true)
                        {
                            detachDevice = existDevice;
                            break;
                        }
                    }
                    if (detachDevice == null) return default(TDevice);

                    // Remove
                    this.Remove(detachDevice);

                    // Return
                    return detachDevice;
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
