using CLK.Data.SqlClient;
using CLK.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Transactions.SqlClient
{
    public sealed class SqlTransactionProvider : TransactionProvider
    {
        // Constructors
        public SqlTransactionProvider()
        {

        }

        public void Start()
        {

        }

        public void Dispose()
        {

        }


        // Methods
        public Transaction Create()
        {            
            // TransactionScope
            var transactionScope = new SqlTransactionScope();

            // Transaction
            var transaction = new SqlTransaction(transactionScope);

            // Return
            return transaction;
        }
    }
}
