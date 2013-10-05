using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLK.Operation;

namespace CLK.Configuration.Operation
{
    public class ConfigComponentWrapperRepository : IComponentWrapperRepository
    {
        // Fields
        private readonly string _configFilename = string.Empty;

        private readonly string _sectionName = string.Empty;


        // Constructors
        public ConfigComponentWrapperRepository(string configFilename, string sectionName)
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
        public IEnumerable<ComponentWrapper> GetAllComponentWrapper()
        {
            // ReflectContext
            CLK.Reflection.ReflectContext reflectContext = new CLK.Configuration.Reflection.ConfigReflectContext(_configFilename);

            // Create
            IEnumerable<ComponentWrapper> componentWrapperCollection = reflectContext.CreateAllEntity<ComponentWrapper>(_sectionName);
            if (componentWrapperCollection == null) throw new InvalidOperationException();

            // Return
            return componentWrapperCollection;
        }
    }
}
