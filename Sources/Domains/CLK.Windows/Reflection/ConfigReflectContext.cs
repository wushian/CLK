using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLK.Settings;

namespace CLK.Reflection
{
    public sealed class ConfigReflectContext : ReflectContext
    {
        // Constructors
        public ConfigReflectContext()
        {
            // Configuration
            System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (configuration == null) throw new InvalidOperationException();

            // Initialize
            this.Initialize(configuration);
        }

        public ConfigReflectContext(string configurationFilename)
        {
            #region Contracts

            if (string.IsNullOrEmpty(configurationFilename) == true) throw new ArgumentNullException();
            
            #endregion

            // Require
            if (System.IO.File.Exists(configurationFilename) == false) throw new System.IO.FileNotFoundException(string.Format("Filename:{0}", configurationFilename));

            // ConfigurationFileMap
            ExeConfigurationFileMap configurationFileMap = new ExeConfigurationFileMap();
            configurationFileMap.ExeConfigFilename = configurationFilename;

            // Configuration
            System.Configuration.Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(configurationFileMap, ConfigurationUserLevel.None);
            if (configuration == null) throw new InvalidOperationException();

            // Initialize
            this.Initialize(configuration);
        }


        // Methods   
        private void Initialize(System.Configuration.Configuration configuration)
        {
            #region Contracts

            if (configuration == null) throw new ArgumentNullException();

            #endregion

            // ReflectRepository
            IReflectRepository reflectRepository = new ConfigReflectRepository(configuration);
            
            // SettingContext
            SettingContext settingContext = new ConfigSettingContext(configuration);

            // Initialize
            base.Initialize(reflectRepository, settingContext);
        }
    }
}
