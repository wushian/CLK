using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceModel
{
    public sealed class ExecuteIgnoredException : Exception
    {
        public ExecuteIgnoredException() : base("ExecuteIgnored") { }

        public ExecuteIgnoredException(string message) : base(message) { }

        public ExecuteIgnoredException(string message, Exception innerException) : base(message, innerException) { }
    }
}
