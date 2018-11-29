using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Transactions
{
    public class TransactionContext : IDisposable
    {
        // Fields
        private readonly IEnumerable<TransactionProvider> _transactionProviderList = null;


        // Constructors
        public TransactionContext(IEnumerable<TransactionProvider> transactionProviderList)
        {
            #region Contracts

            if (transactionProviderList == null) throw new ArgumentException();

            #endregion

            // Default
            _transactionProviderList = transactionProviderList;

            // TransactionFactory
            this.TransactionFactory = new CompositeTransactionFactory(transactionProviderList);
        }

        public void Start()
        {
            // TransactionProviderList
            foreach (var transactionProvider in _transactionProviderList)
            {
                transactionProvider.Start();
            }
        }

        public void Dispose()
        {
            // TransactionProviderList
            foreach (var transactionProvider in _transactionProviderList)
            {
                transactionProvider.Dispose();
            }
        }


        // Properties
        public TransactionFactory TransactionFactory { get; private set; }


        // Methods
        public Transaction BeginTransaction()
        {
            // Return
            return this.TransactionFactory.BeginTransaction();
        }
    }
}
