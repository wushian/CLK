using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Settings
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

        public MemorySettingContext(ISettingRepository appSettingRepository, ISettingRepository connectionStringRepository)
        {
            #region Contracts

            if (appSettingRepository == null) throw new ArgumentNullException();
            if (connectionStringRepository == null) throw new ArgumentNullException();

            #endregion

            // Initialize
            this.Initialize(appSettingRepository, connectionStringRepository);
        }
    }
}
