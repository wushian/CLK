using CLK.Data.SqlClient;
using CLK.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Transactions.SqlClient
{
    public sealed class SqlUnitOfWorkScopeFactory : UnitOfWorkScopeFactory
    {
        // Methods
        public UnitOfWorkScopeProvider Create()
        {            
            // TransactionScope
            var transactionScope = new SqlTransactionScope();

            // UnitOfWorkScopeProvider
            var unitOfWorkScopeProvider = new SqlUnitOfWorkScopeProvider(transactionScope);

            // Return
            return unitOfWorkScopeProvider;
        }
    }
}
