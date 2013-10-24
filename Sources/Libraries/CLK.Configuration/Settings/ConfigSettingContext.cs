using CLK.Settings;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Configuration.Settings
{
    public sealed class ConfigSettingContext : SettingContext
    {
        // Constructors
        public ConfigSettingContext()
        {
            // Configuration
            System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (configuration == null) throw new InvalidOperationException();

            // Initialize
            this.Initialize(configuration);
        }

        public ConfigSettingContext(string configFilename)
        {
            #region Contracts

            if (string.IsNullOrEmpty(configFilename) == true) throw new ArgumentNullException();
            
            #endregion

            // Require
            if (System.IO.File.Exists(configFilename) == false) throw new System.IO.FileNotFoundException(string.Format("Filename:{0}", configFilename));

            // ConfigurationFileMap
            ExeConfigurationFileMap configurationFileMap = new ExeConfigurationFileMap();
            configurationFileMap.ExeConfigFilename = configFilename;

            // Configuration
            System.Configuration.Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(configurationFileMap, ConfigurationUserLevel.None);
            if (configuration == null) throw new InvalidOperationException();

            // Initialize
            this.Initialize(configuration);
        }

        public ConfigSettingContext(System.Configuration.Configuration configuration)
        {
            #region Contracts

            if (configuration == null) throw new ArgumentNullException();

            #endregion

            // Initialize
            this.Initialize(configuration);
        }


        // Methods   
        private void Initialize(System.Configuration.Configuration configuration)
        {
            #region Contracts

            if (configuration == null) throw new ArgumentNullException();

            #endregion

            // AppSettingRepository
            ISettingRepository appSettingRepository = new ConfigAppSettingRepository(configuration);

            // ConnectionStringRepository
            ISettingRepository connectionStringRepository = new ConfigConnectionStringRepository(configuration);

            // Initialize
            this.Initialize(appSettingRepository, connectionStringRepository);
        }
    }
}
