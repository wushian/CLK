using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Transactions
{
    public sealed class UnitOfWorkScope
    {
        // Fields
        private readonly UnitOfWorkScopeProvider _unitOfWorkScopeProvider = null;


        // Constructors
        public UnitOfWorkScope() : this(UnitOfWorkContext.Current.Create()) { }

        internal UnitOfWorkScope(UnitOfWorkScopeProvider unitOfWorkScopeProvider)
        {
            #region Contracts

            if (unitOfWorkScopeProvider == null) throw new ArgumentNullException();

            #endregion

            // Default
            _unitOfWorkScopeProvider = unitOfWorkScopeProvider;
        }

        public void Dispose()
        {
            // Dispose
            _unitOfWorkScopeProvider.Dispose();
        }


        // Methods    
        public void Complete()
        {
            // Complete
            _unitOfWorkScopeProvider.Complete();
        }
    }
}
