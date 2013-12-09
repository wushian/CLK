using CLK.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CLK.ServiceModel
{
    public abstract class ChannelHost<TChannel>
        where TChannel : Channel
    {
        // Fields
        private readonly object _syncObject = new object();

        private readonly List<TChannel> _channelList = new List<TChannel>();

        private IEnumerable<TChannel> _channelCollection = null;


        // Constructors
        internal ChannelHost() { }


        // Properties
        private IEnumerable<TChannel> ChannelCollection
        {
            get
            {
                lock (_syncObject)
                {
                    if (_channelCollection == null)
                    {
                        _channelCollection = _channelList.ToArray();
                    }
                    return _channelCollection;
                }
            }
        }


        // Methods
        protected virtual void Attach(TChannel channel)
        {
            #region Contracts

            if (channel == null) throw new ArgumentNullException();

            #endregion

            lock (_syncObject)
            {
                // Refresh
                _channelCollection = null;

                // Add
                _channelList.Add(channel);
            }
        }

        protected virtual void Detach(TChannel channel)
        {
            #region Contracts

            if (channel == null) throw new ArgumentNullException();

            #endregion

            lock (_syncObject)
            {
                // Refresh
                _channelCollection = null;
                                
                // Remove
                _channelList.Remove(channel);
            }
        }


        public void ExecuteOne(Action<TChannel> executeDelegate)
        {
            #region Contracts

            if (executeDelegate == null) throw new ArgumentNullException();

            #endregion

            // Channel
            foreach (TChannel channel in this.ChannelCollection)
            {
                try
                {
                    // Execute
                    executeDelegate(channel);

                    // Return
                    return;
                }
                catch
                {
                    // Nothing

                }
            }

            // Throw
            throw new ExecuteIgnoredException();
        }

        public TResult ExecuteOne<TResult>(Func<TChannel, TResult> executeDelegate)
        {
            #region Contracts

            if (executeDelegate == null) throw new ArgumentNullException();

            #endregion

            // Channel
            foreach (TChannel channel in this.ChannelCollection)
            {
                try
                {
                    // Execute
                    TResult result = executeDelegate(channel);

                    // Return
                    return result;
                }
                catch
                {
                    // Nothing

                }
            }

            // Throw
            throw new ExecuteIgnoredException();
        }

        public TResult ExecuteOne<TResult>(Func<TChannel, TResult> executeDelegate, Func<TResult, bool> finishPredicate)
        {
            #region Contracts

            if (executeDelegate == null) throw new ArgumentNullException();
            if (finishPredicate == null) throw new ArgumentNullException();

            #endregion

            // Channel
            foreach (TChannel channel in this.ChannelCollection)
            {
                try
                {
                    // Execute
                    TResult result = executeDelegate(channel);

                    // Return
                    if (finishPredicate(result) == true) return result;
                }
                catch
                {
                    // Nothing

                }
            }

            // Throw
            throw new ExecuteIgnoredException();
        }


        public void ExecuteAll(Action<TChannel> executeDelegate)
        {
            #region Contracts

            if (executeDelegate == null) throw new ArgumentNullException();

            #endregion

            // Result
            bool executeIgnored = true;

            // Channel
            foreach (TChannel channel in this.ChannelCollection)
            {
                try
                {
                    // Execute
                    executeDelegate(channel);

                    // Count
                    executeIgnored = false;
                }
                catch
                {
                    // Throw
                    throw;
                }
            }

            // Throw
            if (executeIgnored == true) throw new ExecuteIgnoredException();
        }

        public IEnumerable<TResult> ExecuteAll<TResult>(Func<TChannel, TResult> executeDelegate)
        {
            #region Contracts

            if (executeDelegate == null) throw new ArgumentNullException();

            #endregion

            // Result
            bool executeIgnored = true;
            List<TResult> resultCollection = new List<TResult>();

            // Channel
            foreach (TChannel channel in this.ChannelCollection)
            {
                try
                {
                    // Execute
                    TResult result = executeDelegate(channel);

                    // Add
                    resultCollection.Add(result);

                    // Count
                    executeIgnored = false;
                }
                catch
                {
                    // Throw
                    throw;
                }
            }

            // Throw
            if (executeIgnored == true) throw new ExecuteIgnoredException();

            // Return
            return resultCollection;
        }
    }
}