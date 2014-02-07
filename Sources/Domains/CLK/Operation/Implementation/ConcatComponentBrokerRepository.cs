using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Operation
{
    public sealed class ConcatComponentBrokerRepository : IComponentBrokerRepository
    {
        // Fields 
        private readonly IComponentBrokerRepository _first = null;

        private readonly IComponentBrokerRepository _second = null;


        // Constructors
        public ConcatComponentBrokerRepository(IComponentBrokerRepository first, IComponentBrokerRepository second)
        {
            #region Contracts

            if (first == null) throw new ArgumentNullException();
            if (second == null) throw new ArgumentNullException();

            #endregion

            // Arguments
            _first = first;
            _second = second;
        }


        // Methods
        public IEnumerable<ComponentBroker> GetAllComponentBroker()
        {
            // Create
            var firstResultCollection = _first.GetAllComponentBroker();
            if (firstResultCollection == null) throw new InvalidOperationException();

            var secondResultCollection = _second.GetAllComponentBroker();
            if (secondResultCollection == null) throw new InvalidOperationException();
            
            // Return
            return firstResultCollection.Concat(secondResultCollection).ToArray();
        }
    }

    public static class ConcatComponentBrokerRepositoryExtension
    {
        // Concat
        public static IComponentBrokerRepository Concat(this IComponentBrokerRepository first, IComponentBrokerRepository second)
        {
            #region Contracts

            if (first == null) throw new ArgumentNullException();
            if (second == null) throw new ArgumentNullException();

            #endregion

            // Return  
            return new ConcatComponentBrokerRepository(first, second);
        }
    }
}
