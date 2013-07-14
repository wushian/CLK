using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Configuration.Samples
{
    public sealed class EntityElementSection : ConfigurationSection
    {
        // Properties
        [ConfigurationProperty("", IsDefaultCollection = true, IsRequired = false)]
        public EntityElementCollection EntityElementCollection
        {
            get
            {
                return (EntityElementCollection)base[""];
            }
        }
    }
}