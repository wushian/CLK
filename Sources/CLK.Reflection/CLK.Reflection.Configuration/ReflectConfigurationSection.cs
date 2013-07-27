using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Reflection.Configuration
{
    public sealed class ReflectConfigurationSection : ConfigurationSection
    {
        // Properties
        [ConfigurationProperty("default", DefaultValue = "", IsRequired = false)]
        public string DefaultEntityName
        {
            get
            {
                return (string)base["default"];
            }
            set
            {
                base["default"] = value;
            }
        }

        [ConfigurationProperty("", IsDefaultCollection = true, IsRequired = false)]
        public ReflectConfigurationElementCollection SettingCollection
        {
            get
            {
                return (ReflectConfigurationElementCollection)base[""];
            }
        }
    }
}
