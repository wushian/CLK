using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Reflection.Configuration
{
    internal sealed class ConfigReflectRepository : IReflectRepository
    {
        // Fields
        private readonly System.Configuration.Configuration _configuration = null;


        // Constructors
        public ConfigReflectRepository(System.Configuration.Configuration configuration)
        {
            #region Contracts

            if (configuration == null) throw new ArgumentNullException();

            #endregion

            // Configuration
            _configuration = configuration;
        }


        // Methods
        private ReflectConfigurationSection GetSection(string sectionName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(sectionName) == true) throw new ArgumentNullException();

            #endregion

            // Section
            ReflectConfigurationSection section = _configuration.GetSection(sectionName) as ReflectConfigurationSection;

            // Return
            return section;
        }

        private ReflectConfigurationSection CreateSection(string sectionName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(sectionName) == true) throw new ArgumentNullException();

            #endregion

            // Section
            ReflectConfigurationSection section = _configuration.GetSection(sectionName) as ReflectConfigurationSection;
            if (section != null) return section;
            
            // Create
            string[] sectionNameChain = sectionName.Split('/');
            ConfigurationSectionGroup sectionGroup = null;
            for (int i = 0; i < sectionNameChain.Length; i++)
            {
                if (i != sectionNameChain.Length - 1)
                {
                    // SectionGroup
                    ConfigurationSectionGroup childSectionGroup = new ConfigurationSectionGroup();
                    if (sectionGroup == null)
                    {
                        _configuration.SectionGroups.Add(sectionNameChain[i], childSectionGroup);
                    }
                    else
                    {
                        sectionGroup.SectionGroups.Add(sectionNameChain[i], childSectionGroup);
                    }
                    sectionGroup = childSectionGroup;
                }
                else
                {
                    // Section
                    section = new ReflectConfigurationSection();
                    if (sectionGroup != null)
                    {
                        sectionGroup.Sections.Add(sectionNameChain[i], section);
                    }
                    else
                    {
                        _configuration.Sections.Add(sectionNameChain[i], section);
                    }
                }
            }

            // Save
            _configuration.Save();

            // Return
            return section;
        }


        public void AddSection(string sectionName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(sectionName) == true) throw new ArgumentNullException();

            #endregion

        }

        public void RemoveSection(string sectionName)
        {
            throw new NotImplementedException();
        }

        public bool ContainsSection(string sectionName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetAllSectionName()
        {
            throw new NotImplementedException();
        }


        public void AddDefaultEntityName(string sectionName, string entityName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(sectionName) == true) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(entityName) == true) throw new ArgumentNullException();

            #endregion

            // Section
            ReflectConfigurationSection section = this.GetSection(sectionName);
            if (section == null) throw new InvalidOperationException(string.Format("Fail to Get ReflectConfigurationSection:{0}", sectionName));

            // EntityName
            section.DefaultEntityName = entityName;

            // Save
            section.CurrentConfiguration.Save();
        }

        public void RemoveDefaultEntityName(string sectionName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(sectionName) == true) throw new ArgumentNullException();

            #endregion

            // Section
            ReflectConfigurationSection section = this.GetSection(sectionName);
            if (section == null) throw new InvalidOperationException(string.Format("Fail to Get ReflectConfigurationSection:{0}", sectionName));

            // EntityName
            section.DefaultEntityName = string.Empty;

            // Save
            section.CurrentConfiguration.Save();
        }

        public bool ContainsDefaultEntityName(string sectionName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(sectionName) == true) throw new ArgumentNullException();

            #endregion

            // Section
            ReflectConfigurationSection section = this.GetSection(sectionName);
            if (section == null) throw new InvalidOperationException(string.Format("Fail to Get ReflectConfigurationSection:{0}", sectionName));

            // Contains
            if (string.IsNullOrEmpty(section.DefaultEntityName) == false)
            {
                return true;
            }
            return false;
        }

        public string GetDefaultEntityName(string sectionName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(sectionName) == true) throw new ArgumentNullException();

            #endregion

            // Section
            ReflectConfigurationSection section = this.GetSection(sectionName);
            if (section == null) throw new InvalidOperationException(string.Format("Fail to Get ReflectConfigurationSection:{0}", sectionName));

            // EntityName
            if (string.IsNullOrEmpty(section.DefaultEntityName) == false)
            {
                return section.DefaultEntityName;
            }
            return string.Empty;
        }


        public void AddSetting(string sectionName, string entityName, ReflectSetting setting)
        {
            #region Contracts

            if (string.IsNullOrEmpty(sectionName) == true) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(entityName) == true) throw new ArgumentNullException();
            if (setting == null) throw new ArgumentNullException();

            #endregion

            // Section
            ReflectConfigurationSection section = this.GetSection(sectionName);
            if (section == null) throw new InvalidOperationException(string.Format("Fail to Get ReflectConfigurationSection:{0}", sectionName));

            // Element
            ReflectConfigurationElement element = new ReflectConfigurationElement();
            element.EntityName = entityName;
            element.BuilderType = setting.BuilderType;
            foreach (string parameterKey in setting.Parameters.Keys)
            {
                element.FreeAttributes.Add(parameterKey, setting.Parameters[parameterKey]);
            }

            // Save
            section.CurrentConfiguration.Save();
        }

        public void RemoveSetting(string sectionName, string entityName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(sectionName) == true) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(entityName) == true) throw new ArgumentNullException();

            #endregion

            // Section
            ReflectConfigurationSection section = this.GetSection(sectionName);
            if (section == null) throw new InvalidOperationException(string.Format("Fail to Get ReflectConfigurationSection:{0}", sectionName));

            // Remove
            section.SettingCollection.Remove(entityName);

            // Save
            section.CurrentConfiguration.Save();
        }

        public bool ContainsSetting(string sectionName, string entityName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(sectionName) == true) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(entityName) == true) throw new ArgumentNullException();

            #endregion

            // Section
            ReflectConfigurationSection section = this.GetSection(sectionName);
            if (section == null) throw new InvalidOperationException(string.Format("Fail to Get ReflectConfigurationSection:{0}", sectionName));

            // Element
            ReflectConfigurationElement element = section.SettingCollection.GetByName(entityName);
            if (element == null) return false;

            // Return
            return true;
        }

        public ReflectSetting GetSetting(string sectionName, string entityName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(sectionName) == true) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(entityName) == true) throw new ArgumentNullException();

            #endregion

            // Section
            ReflectConfigurationSection section = this.GetSection(sectionName);
            if (section == null) throw new InvalidOperationException(string.Format("Fail to Get ReflectConfigurationSection:{0}", sectionName));

            // Element
            ReflectConfigurationElement element = section.SettingCollection.GetByName(entityName);
            if (element == null) return null;

            // Setting
            ReflectSetting setting = new ReflectSetting(element.BuilderType);
            foreach (string parameterKey in element.FreeAttributes.Keys)
            {
                setting.Parameters.Add(parameterKey, element.FreeAttributes[parameterKey]);
            }

            // Return
            return setting;
        }

        public IEnumerable<string> GetAllEntityName(string sectionName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(sectionName) == true) throw new ArgumentNullException();

            #endregion

            // Section
            ReflectConfigurationSection section = this.GetSection(sectionName);
            if (section == null) throw new InvalidOperationException(string.Format("Fail to Get ReflectConfigurationSection:{0}", sectionName));
                        
            // Create
            List<string> entityNameList = new List<string>();
            foreach (ReflectConfigurationElement element in section.SettingCollection)
            {
                entityNameList.Add(element.EntityName);
            }
            return entityNameList;
        }
    }
}
