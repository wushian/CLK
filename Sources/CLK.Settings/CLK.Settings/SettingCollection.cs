using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLK.Collections;

namespace CLK.Settings
{
    public class SettingCollection : StoreDictionary<string, string>
    {        
        // Constructors
        public SettingCollection(ISettingRepository repository) : base(repository) { }
    }
}
