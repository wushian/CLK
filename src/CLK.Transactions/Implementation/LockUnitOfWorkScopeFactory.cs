using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Transactions
{
    public sealed class LockUnitOfWorkScopeFactory : UnitOfWorkScopeFactory
    {
        // Fields
        private readonly object _syncRoot = new object();

        private readonly UnitOfWorkScopeFactory _unitOfWorkScopeFactory = null;


        // Constructors
        public LockUnitOfWorkScopeFactory(UnitOfWorkScopeFactory unitOfWorkScopeFactory)
        {
            #region Contracts
                                    
            if (unitOfWorkScopeFactory == null) throw new ArgumentNullException();

            #endregion

            // Default
            _unitOfWorkScopeFactory = unitOfWorkScopeFactory;
        }


        // Methods
        public UnitOfWorkScopeProvider Create()
        {
            // Return
            return new LockUnitOfWorkScopeProvider(_syncRoot, _unitOfWorkScopeFactory);
        }
    }
}
