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
    public abstract class ConnectionHost<TConnection>
        where TConnection : Connection
    {
        // Fields
        private readonly object _syncObject = new object();

        private readonly List<TConnection> _connectionList = new List<TConnection>();

        private IEnumerable<TConnection> _connectionCollection = null;


        // Constructors
        internal ConnectionHost() { }


        // Properties
        private IEnumerable<TConnection> ConnectionCollection
        {
            get
            {
                lock (_syncObject)
                {
                    if (_connectionCollection == null)
                    {
                        _connectionCollection = _connectionList.ToArray();
                    }
                    return _connectionCollection;
                }
            }
        }


        // Methods
        protected virtual void Attach(TConnection connection)
        {
            #region Contracts

            if (connection == null) throw new ArgumentNullException();

            #endregion

            lock (_syncObject)
            {
                // Refresh
                _connectionCollection = null;

                // Add
                _connectionList.Add(connection);
            }
        }

        protected virtual void Detach(TConnection connection)
        {
            #region Contracts

            if (connection == null) throw new ArgumentNullException();

            #endregion

            lock (_syncObject)
            {
                // Refresh
                _connectionCollection = null;
                                
                // Remove
                _connectionList.Remove(connection);
            }
        }


        public void ExecuteOne(Action<TConnection> executeDelegate)
        {
            #region Contracts

            if (executeDelegate == null) throw new ArgumentNullException();

            #endregion

            // Connection
            foreach (TConnection connection in this.ConnectionCollection)
            {
                try
                {
                    // Execute
                    executeDelegate(connection);

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

        public TResult ExecuteOne<TResult>(Func<TConnection, TResult> executeDelegate)
        {
            #region Contracts

            if (executeDelegate == null) throw new ArgumentNullException();

            #endregion

            // Connection
            foreach (TConnection connection in this.ConnectionCollection)
            {
                try
                {
                    // Execute
                    TResult result = executeDelegate(connection);

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

        public TResult ExecuteOne<TResult>(Func<TConnection, TResult> executeDelegate, Func<TResult, bool> finishPredicate)
        {
            #region Contracts

            if (executeDelegate == null) throw new ArgumentNullException();
            if (finishPredicate == null) throw new ArgumentNullException();

            #endregion

            // Connection
            foreach (TConnection connection in this.ConnectionCollection)
            {
                try
                {
                    // Execute
                    TResult result = executeDelegate(connection);

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


        public void ExecuteAll(Action<TConnection> executeDelegate)
        {
            #region Contracts

            if (executeDelegate == null) throw new ArgumentNullException();

            #endregion

            // Result
            bool executeIgnored = true;

            // Connection
            foreach (TConnection connection in this.ConnectionCollection)
            {
                try
                {
                    // Execute
                    executeDelegate(connection);

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

        public IEnumerable<TResult> ExecuteAll<TResult>(Func<TConnection, TResult> executeDelegate)
        {
            #region Contracts

            if (executeDelegate == null) throw new ArgumentNullException();

            #endregion

            // Result
            bool executeIgnored = true;
            List<TResult> resultCollection = new List<TResult>();

            // Connection
            foreach (TConnection connection in this.ConnectionCollection)
            {
                try
                {
                    // Execute
                    TResult result = executeDelegate(connection);

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