using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Settings
{
    public abstract class SettingContext
    {
        // Constructors
        protected void Initialize(ISettingRepository appSettingRepository, ISettingRepository connectionStringRepository)
        {
            #region Contracts

            if (appSettingRepository == null) throw new ArgumentNullException();
            if (connectionStringRepository == null) throw new ArgumentNullException();

            #endregion

            // Arguments
            this.AppSettings = new SettingDictionary(appSettingRepository);
            this.ConnectionStrings = new SettingDictionary(connectionStringRepository);
        }


        // Properties
        public SettingDictionary AppSettings { get; private set; }

        public SettingDictionary ConnectionStrings { get; private set; }        
    }
}
