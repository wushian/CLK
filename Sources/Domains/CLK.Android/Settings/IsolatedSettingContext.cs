using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Settings
{
    public sealed class IsolatedSettingContext : SettingContext
    {
        // Constructors
        public IsolatedSettingContext() : this(new IsolatedSettingRepository(), new IsolatedSettingRepository()) { }

        public IsolatedSettingContext(ISettingRepository appSettingRepository, ISettingRepository connectionStringRepository) : base(appSettingRepository, connectionStringRepository) { }
    }
}
