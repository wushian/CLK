using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Settings
{
    public abstract partial class SettingContext
    {
        // Locator
        public static SettingContext Current { get; set; }
    }

    public abstract partial class SettingContext
    {
        // Properties
        public SettingCollection AppSettings { get; private set; }

        public SettingCollection ConnectionStrings { get; private set; }


        // Methods   
        protected void Initialize(ISettingRepository appSettingRepository, ISettingRepository connectionStringRepository)
        {
            #region Contracts

            if (appSettingRepository == null) throw new ArgumentNullException();
            if (connectionStringRepository == null) throw new ArgumentNullException();

            #endregion

            // Arguments
            this.AppSettings = new SettingCollection(appSettingRepository);
            this.ConnectionStrings = new SettingCollection(connectionStringRepository);
        }
    }
}
