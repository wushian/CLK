using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace CLK.Transactions
{
    internal class CompositeTransaction : Transaction
    {
        // Fields
        private readonly IEnumerable<Transaction> _transactionList = null;


        // Constructors
        public CompositeTransaction(IEnumerable<Transaction> transactionList)
        {
            #region Contracts

            if (transactionList == null) throw new ArgumentException(nameof(transactionList));

            #endregion

            // Default
            _transactionList = transactionList;
        }

        public void Dispose()
        {
            // Dispose
            foreach (var transaction in _transactionList)
            {
                transaction.Dispose();
            }
        }


        // Methods
        public void Complete()
        {
            // Complete
            foreach (var transaction in _transactionList)
            {
                transaction.Complete();
            }
        }
    }
}
