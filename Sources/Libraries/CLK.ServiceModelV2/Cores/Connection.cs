using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceModel
{
    public abstract class Connection<TService>
        where TService : class, IConnectionService
    {
        // Constructors
        internal Connection() { }


        // Properties    
        public abstract TService Service { get; }
    }

    public abstract class Connection<TService, TCallback>
        where TService : class, IConnectionService
        where TCallback : class
    {
        // Constructors
        internal Connection() { }


        // Properties    
        public abstract TService Service { get; }

        public abstract TCallback Callback { get; }
    }
}
