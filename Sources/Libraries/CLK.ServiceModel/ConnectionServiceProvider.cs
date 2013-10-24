using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceModel
{    
    public class ConnectionServiceProvider
    {
        // Methods
        public virtual TCallback GetCallbackChannel<TCallback>()
        {
            return OperationContext.Current.GetCallbackChannel<TCallback>();
        }
    }
}
