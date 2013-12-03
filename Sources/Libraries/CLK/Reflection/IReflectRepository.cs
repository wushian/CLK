using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Reflection
{
    public interface IReflectRepository
    {
        // Methods
        void AddSection(string sectionName);

        void RemoveSection(string sectionName);

        bool ContainsSection(string sectionName);

        IEnumerable<string> GetAllSectionName();
                        
                
        void AddSetting(string sectionName, string entityName, ReflectSetting setting);

        void RemoveSetting(string sectionName, string entityName);

        bool ContainsSetting(string sectionName, string entityName);                 

        IEnumerable<string> GetAllEntityName(string sectionName);

        ReflectSetting GetSetting(string sectionName, string entityName);     


        void AddDefaultEntityName(string sectionName, string entityName);

        void RemoveDefaultEntityName(string sectionName);

        bool ContainsDefaultEntityName(string sectionName);

        string GetDefaultEntityName(string sectionName);
    }
}
