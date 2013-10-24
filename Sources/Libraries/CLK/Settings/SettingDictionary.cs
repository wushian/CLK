using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLK.Collections;

namespace CLK.Settings
{
    public sealed class SettingDictionary : StoreDictionary<string, string>
    {        
        // Constructors
        internal SettingDictionary(ISettingRepository repository) : base(repository) { }
    }
}
