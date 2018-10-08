using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CLK.Transactions
{
    public sealed class DistributedUnitOfWorkScopeFactory : UnitOfWorkScopeFactory
    {
        // Methods
        public UnitOfWorkScopeProvider Create()
        {            
            // TransactionScope
            TransactionScope transactionScope = new TransactionScope();

            // UnitOfWorkScopeProvider
            UnitOfWorkScopeProvider unitOfWorkScopeProvider = new DistributedUnitOfWorkScopeProvider(transactionScope);

            // Return
            return unitOfWorkScopeProvider;
        }
    }
}
