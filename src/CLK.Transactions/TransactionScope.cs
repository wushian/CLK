using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Transactions
{
    public partial class TransactionScope : IDisposable
    {
        // Constructors
        public TransactionScope() : this(TransactionContext.Current.Create()) { }
    }

    public partial class TransactionScope : IDisposable
    {
        // Fields
        private readonly TransactionScopeProvider _transactionScopeProvider = null;


        // Constructors
        private TransactionScope(TransactionScopeProvider transactionScopeProvider)
        {
            #region Contracts

            if (transactionScopeProvider == null) throw new ArgumentNullException();

            #endregion

            // Default
            _transactionScopeProvider = transactionScopeProvider;
        }

        public void Dispose()
        {
            // Dispose
            _transactionScopeProvider.Dispose();
        }


        // Methods    
        public void Complete()
        {
            // Complete
            _transactionScopeProvider.Complete();
        }
    }
}
