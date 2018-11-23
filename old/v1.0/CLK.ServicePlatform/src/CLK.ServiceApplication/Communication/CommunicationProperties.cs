using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceApplication.Communication
{
    public class CommunicationProperties
    {
        // Constructors
        public CommunicationProperties() : this(new Dictionary<string, string>()) { }

        public CommunicationProperties(IDictionary<string, string> items)
        {
            #region Contracts

            if (items == null) throw new ArgumentException();

            #endregion

            // Default
            this.Items = items;
        }


        // Properties
        public IDictionary<string, string> Items { get; private set; }
    }
}
