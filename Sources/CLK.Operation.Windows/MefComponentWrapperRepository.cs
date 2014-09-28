using CLK.Operation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Operation
{
    public class MefComponentWrapperRepository<TBuilder> : IComponentWrapperRepository
        where TBuilder : class, IMefComponentWrapperBuilder
    {
        // Methods
        public IEnumerable<ComponentWrapper> GetAllComponentWrapper()
        {
            // Builder
            IEnumerable<TBuilder> builderCollection = this.GetAllExportedValue<TBuilder>();
            if (builderCollection == null) throw new InvalidOperationException();
            
            // Create
            List<ComponentWrapper> componentWrapperCollection = new List<ComponentWrapper>();
            foreach (IMefComponentWrapperBuilder builder in builderCollection)
            {
                ComponentWrapper componentWrapper = builder.Create();
                if (componentWrapper == null) throw new InvalidOperationException();
                componentWrapperCollection.Add(componentWrapper);
            }

            // Return
            return componentWrapperCollection;
        }

        private IEnumerable<T> GetAllExportedValue<T>(string contractName = null)
        {
            // Container
            CompositionContainer container = this.CreateContainer();
            if (container == null) throw new InvalidOperationException();

            // Create
            IEnumerable<T> exportedValueCollection = null;
            if (string.IsNullOrEmpty(contractName) == true)
            {
                exportedValueCollection = container.GetExportedValues<T>();
            }
            else
            {
                exportedValueCollection = container.GetExportedValues<T>(contractName);
            }
            if (exportedValueCollection == null) throw new InvalidOperationException();

            // Return
            return exportedValueCollection;
        } 

        private CompositionContainer CreateContainer()
        {
            // Catalog
            AggregateCatalog catalog = new AggregateCatalog();

            // 同目錄下的組件
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            catalog.Catalogs.Add(new DirectoryCatalog(path, "*.exe"));
            catalog.Catalogs.Add(new DirectoryCatalog(path, "*.dll"));

            // 同DLL內的組件
            catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));

            // Return
            return new CompositionContainer(catalog);
        }        
    }
}
