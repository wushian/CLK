using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Setting
{
    public sealed class MemorySettingContext : SettingContext
    {
        // Constructors
        public MemorySettingContext()
        {
            // AppSettingRepository
            ISettingRepository appSettingRepository = new MemorySettingRepository();

            // ConnectionStringRepository
            ISettingRepository connectionStringRepository = new MemorySettingRepository();

            // Initialize
            this.Initialize(appSettingRepository, connectionStringRepository);
        }
    }
}
