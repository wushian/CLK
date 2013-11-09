using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Communication
{
    public sealed class DisconnectException : Exception
    {
        public DisconnectException() : base("Disconnect") { }

        public DisconnectException(string message) : base(message) { }

        public DisconnectException(string message, Exception innerException) : base(message, innerException) { }
    }
}
