using System;
using System.Collections.Generic;
using System.Text;

namespace CLK.Transactions
{
    public interface TransactionFactory
    {
        // Methods
        Transaction BeginTransaction();
    }
}
