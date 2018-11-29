using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Transactions
{
    public sealed class DistributedTransactionProvider : TransactionProvider
    {
        // Constructors
        public DistributedTransactionProvider()
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
            System.Transactions.TransactionScope transactionScope = new System.Transactions.TransactionScope();

            // Transaction
            Transaction transaction = new DistributedTransaction(transactionScope);

            // Return
            return transaction;
        }
    }
}
