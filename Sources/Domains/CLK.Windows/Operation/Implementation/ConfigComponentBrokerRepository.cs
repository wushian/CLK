using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLK.Operation;

namespace CLK.Operation
{
    public class ConfigComponentBrokerRepository : IComponentBrokerRepository
    {
        // Fields
        private readonly string _configFilename = string.Empty;

        private readonly string _sectionName = string.Empty;


        // Constructors
        public ConfigComponentBrokerRepository(string configFilename, string sectionName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(configFilename) == true) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(sectionName) == true) throw new ArgumentNullException();

            #endregion

            // Arguments
            _configFilename = configFilename;
            _sectionName = sectionName;
        }


        // Methods
        public IEnumerable<ComponentBroker> GetAllComponentBroker()
        {
            // ReflectContext
            CLK.Reflection.ReflectContext reflectContext = new CLK.Reflection.ConfigReflectContext(_configFilename);

            // Create
            IEnumerable<ComponentBroker> componentBrokerCollection = reflectContext.CreateAllEntity<ComponentBroker>(_sectionName);
            if (componentBrokerCollection == null) throw new InvalidOperationException();

            // Return
            return componentBrokerCollection;
        }
    }
}
