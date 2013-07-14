using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Configuration.Samples
{
    public sealed class EntityElement : FreeAttributeConfigurationElement
    {
        // Constructors
        public EntityElement()
        {
            // Default
            this.Name = string.Empty;
            this.FixedAttribute = string.Empty;
        }


        // Properties
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get
            {
                return Convert.ToString(base["name"]);
            }
            set
            {
                base["name"] = value;
            }
        }

        [ConfigurationProperty("fixedAttribute", IsRequired = true)]
        public string FixedAttribute
        {
            get
            {
                return Convert.ToString(base["fixedAttribute"]);
            }
            set
            {
                base["fixedAttribute"] = value;
            }
        }
    }
}
