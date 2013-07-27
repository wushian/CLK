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
        public void AddSection(string sectionName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(sectionName) == true) throw new ArgumentNullException();

            #endregion
            
            // Section
            ReflectConfigurationSection existSection = _configuration.GetSection(sectionName) as ReflectConfigurationSection;
            if (existSection != null) return;

            // Variables
            string[] sectionNameChain = sectionName.Split('/');
            ConfigurationSectionGroup parentSectionGroup = _configuration.RootSectionGroup;

            // Add            
            for (int i = 0; i < sectionNameChain.Length; i++)
            {
                // SectionGroup
                if (i < sectionNameChain.Length - 1)
                {
                    ConfigurationSectionGroup sectionGroup = parentSectionGroup.SectionGroups.Get(sectionNameChain[i]);
                    if (sectionGroup == null)
                    {
                        sectionGroup = new ConfigurationSectionGroup();
                        parentSectionGroup.SectionGroups.Add(sectionNameChain[i], sectionGroup);
                    }
                    parentSectionGroup = sectionGroup;
                    continue;
                }

                // Section
                if (i >= sectionNameChain.Length - 1)                
                {
                    ConfigurationSection section = parentSectionGroup.Sections.Get(sectionNameChain[i]);
                    if (section == null)
                    {
                        section = new ReflectConfigurationSection();
                        parentSectionGroup.Sections.Add(sectionNameChain[i], section);
                    }
                    continue;
                }
            }

            // Save
            _configuration.Save();            
        }

        public void RemoveSection(string sectionName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(sectionName) == true) throw new ArgumentNullException();

            #endregion

            // Section
            ReflectConfigurationSection existSection = _configuration.GetSection(sectionName) as ReflectConfigurationSection;
            if (existSection == null) return;

            // Variables
            string[] sectionNameChain = sectionName.Split('/');
            ConfigurationSectionGroup parentSectionGroup = _configuration.RootSectionGroup;

            // Remove            
            for (int i = sectionNameChain.Length - 1; i >= 0; i--)
            {
                // TargetName
                string targetName = string.Join("/", sectionNameChain, 0, i + 1);
                string parentName = string.Join("/", sectionNameChain, 0, i);

                // ParentSectionGroup
                parentSectionGroup = _configuration.RootSectionGroup;
                if (string.IsNullOrEmpty(parentName) == false) parentSectionGroup = _configuration.GetSectionGroup(parentName);
                if (parentSectionGroup == null) throw new InvalidOperationException(string.Format("Fail to Get ParentSectionGroup:{0}", parentName));

                // SectionGroup
                if (i < sectionNameChain.Length - 1)
                {
                    ConfigurationSectionGroup sectionGroup = parentSectionGroup.SectionGroups.Get(sectionNameChain[i]);
                    if (sectionGroup != null)
                    {
                        if (sectionGroup.SectionGroups.Count == 0)
                        {
                            if (sectionGroup.Sections.Count == 0)
                            {
                                parentSectionGroup.SectionGroups.Remove(sectionNameChain[i]);
                            }
                        }
                    }
                    continue;
                }

                // Section
                if (i >= sectionNameChain.Length - 1)
                {
                    parentSectionGroup.Sections.Remove(sectionNameChain[i]);
                    continue;
                }
            }

            // Save
            _configuration.Save();     
        }

        public bool ContainsSection(string sectionName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(sectionName) == true) throw new ArgumentNullException();

            #endregion

            // Section
            ReflectConfigurationSection section = _configuration.GetSection(sectionName) as ReflectConfigurationSection;
            if (section == null) return false;
            
            // Return
            return true;
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
            ReflectConfigurationSection section = _configuration.GetSection(sectionName) as ReflectConfigurationSection;
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
            ReflectConfigurationSection section = _configuration.GetSection(sectionName) as ReflectConfigurationSection;
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
            ReflectConfigurationSection section = _configuration.GetSection(sectionName) as ReflectConfigurationSection;
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
            ReflectConfigurationSection section = _configuration.GetSection(sectionName) as ReflectConfigurationSection;
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
            ReflectConfigurationSection section = _configuration.GetSection(sectionName) as ReflectConfigurationSection;
            if (section == null) throw new InvalidOperationException(string.Format("Fail to Get ReflectConfigurationSection:{0}", sectionName));

            // Element
            ReflectConfigurationElement element = new ReflectConfigurationElement();
            element.EntityName = entityName;
            element.BuilderType = setting.BuilderType;
            foreach (string parameterKey in setting.Parameters.Keys)
            {
                element.FreeAttributes.Add(parameterKey, setting.Parameters[parameterKey]);
            }

            // Add
            section.SettingCollection.Add(element);

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
            ReflectConfigurationSection section = _configuration.GetSection(sectionName) as ReflectConfigurationSection;
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
            ReflectConfigurationSection section = _configuration.GetSection(sectionName) as ReflectConfigurationSection;
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
            ReflectConfigurationSection section = _configuration.GetSection(sectionName) as ReflectConfigurationSection;
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
            ReflectConfigurationSection section = _configuration.GetSection(sectionName) as ReflectConfigurationSection;
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
