using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Transactions
{
    public partial class UnitOfWorkContext
    {
        // Singleton 
        private static UnitOfWorkContext _current;

        internal static UnitOfWorkContext Current
        {
            get
            {
                // Require
                if (_current == null) throw new InvalidOperationException("_current=null");

                // Return
                return _current;
            }
        }

        public static void Initialize(UnitOfWorkContext context)
        {
            #region Contracts

            if (context == null) throw new ArgumentNullException();

            #endregion

            // Default
            _current = context;
        }
    }

    public partial class UnitOfWorkContext
    {
        // Fields
        private readonly UnitOfWorkScopeFactory _unitOfWorkScopeFactory = null;


        // Constructors
        public UnitOfWorkContext(UnitOfWorkScopeFactory unitOfWorkScopeFactory)
        {
            #region Contracts

            if (unitOfWorkScopeFactory == null) throw new ArgumentNullException();

            #endregion

            // Default
            _unitOfWorkScopeFactory = unitOfWorkScopeFactory;
        }


        // Methods
        internal UnitOfWorkScopeProvider Create()
        {
            // UnitOfWorkScopeProvider
            var unitOfWorkScopeProvider = _unitOfWorkScopeFactory.Create();
            if (unitOfWorkScopeProvider == null) throw new InvalidOperationException("unitOfWorkScopeProvider=null");

            // Return
            return unitOfWorkScopeProvider;
        }
    }
}
