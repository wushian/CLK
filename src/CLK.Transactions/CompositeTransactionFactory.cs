using System;
using System.Collections.Generic;
using System.Text;

namespace CLK.Transactions
{
    internal class CompositeTransactionFactory : TransactionFactory
    {
        // Fields
        private readonly IEnumerable<TransactionProvider> _transactionProviderList = null;


        // Constructors
        public CompositeTransactionFactory(IEnumerable<TransactionProvider> transactionProviderList)
        {
            #region Contracts

            if (transactionProviderList == null) throw new ArgumentException();

            #endregion

            // Default
            _transactionProviderList = transactionProviderList;
        }


        // Methods
        public Transaction BeginTransaction()
        {
            // TransactionList
            var transactionList = new List<Transaction>();
            foreach (var transactionProvider in _transactionProviderList)
            {
                // Transaction
                var transaction = transactionProvider.Create();
                if (transaction == null) throw new InvalidOperationException("transaction=null");

                // Add
                transactionList.Add(transaction);
            }

            // Return
            return new CompositeTransaction(transactionList);
        }
    }
}
