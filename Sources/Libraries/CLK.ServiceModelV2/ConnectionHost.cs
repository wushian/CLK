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
        where TConnection : class
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
        internal void Attach(TConnection connection)
        {
            lock (_syncObject)
            {
                // Require
                if (_connectionList.Contains(connection) == true) return;

                // Add
                _connectionList.Add(connection);

                // Attach
                this.AttachConnection(connection);

                // Refresh
                _connectionCollection = null;
            }
        }

        internal void Detach(TConnection connection)
        {
            lock (_syncObject)
            {
                // Require
                if (_connectionList.Contains(connection) == false) return;

                // Remove
                _connectionList.Remove(connection);

                // Detach
                this.DetachConnection(connection);

                // Refresh
                _connectionCollection = null;
            }
        }

        protected virtual void AttachConnection(TConnection connection) { }

        protected virtual void DetachConnection(TConnection connection) { }


        public void Execute(Action<TConnection> executeDelegate)
        {
            #region Contracts

            if (executeDelegate == null) throw new ArgumentNullException();

            #endregion

            // ConnectionCollection
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

        public TResult Execute<TResult>(Func<TConnection, TResult> executeDelegate)
        {
            #region Contracts

            if (executeDelegate == null) throw new ArgumentNullException();

            #endregion

            // ConnectionCollection
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

        public TResult Execute<TResult>(Func<TConnection, TResult> executeDelegate, Func<TResult, bool> finishPredicate)
        {
            #region Contracts

            if (executeDelegate == null) throw new ArgumentNullException();
            if (finishPredicate == null) throw new ArgumentNullException();

            #endregion

            // ConnectionCollection
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

            // ConnectionCollection
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

            // ConnectionCollection
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