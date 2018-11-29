using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Transactions
{
    public sealed class DistributedTransaction : Transaction
    {
        // Fields
        private readonly System.Transactions.TransactionScope _transactionScope = null;


        // Constructors
        public DistributedTransaction(System.Transactions.TransactionScope transactionScope)
        {
            #region Contracts

            if (transactionScope == null) throw new ArgumentNullException();

            #endregion

            // Arguments
            _transactionScope = transactionScope;
        }

        public void Dispose()
        {
            // Dispose
            _transactionScope.Dispose();
        }


        // Methods    
        public void Complete()
        {
            // Complete
            _transactionScope.Complete();
        }
    }
}
