using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CLK.Transactions
{
    internal sealed class LockUnitOfWorkScopeProvider : UnitOfWorkScopeProvider
    {
        // Fields
        private readonly object _syncRoot = null;

        private readonly UnitOfWorkScopeProvider _unitOfWorkScopeProvider = null;


        // Constructors
        public LockUnitOfWorkScopeProvider(object syncRoot, UnitOfWorkScopeFactory unitOfWorkScopeFactory)
        {
            #region Contracts

            if (syncRoot == null) throw new ArgumentNullException();
            if (unitOfWorkScopeFactory == null) throw new ArgumentNullException();

            #endregion

            // Default
            _syncRoot = syncRoot;

            // Execute
            try
            {
                // Monitor
                Monitor.Enter(_syncRoot);

                // UnitOfWorkScopeProvider
                _unitOfWorkScopeProvider = unitOfWorkScopeFactory.Create();
                if (_unitOfWorkScopeProvider == null) throw new InvalidOperationException("_unitOfWorkScopeProvider=null");
            }
            catch
            {
                // Dispose
                this.Dispose();

                // Throw
                throw;
            }
        }

        public void Dispose()
        {
            // Execute
            try
            {
                // Dispose
                _unitOfWorkScopeProvider.Dispose();
            }
            finally
            {
                // Monitor
                Monitor.Exit(_syncRoot);
            }
        }


        // Methods    
        public void Complete()
        {
            // Complete
            _unitOfWorkScopeProvider.Complete();
        }
    }
}
