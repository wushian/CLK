using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Reflection.Samples
{
    class Program
    {
        // Methods
        public static void Main(string[] args)
        {
            // Sample
            ConfigReflectionSample();

            // End
            Console.ReadLine();
        }


        private static void ConfigReflectionSample()
        {
            // Initialize
            ReflectContext.Current = new CLK.Reflection.Configuration.ConfigReflectContext();

            // Run
            Run("Config Settings Sample");
        }


        private static void Run(string title)
        {
            #region Contracts

            if (string.IsNullOrEmpty(title) == true) throw new ArgumentException();

            #endregion

            // Title
            Console.WriteLine("====================");
            Console.WriteLine(title);
            Console.WriteLine();

            // Result
            Entity entity = null;
            EntityBuilder builder = null;

            // List by Section
            Console.WriteLine("List from Section");
            foreach (string sectionName in ReflectContext.Current.ReflectSectionCollection.Keys)
            {
                ReflectSection section = ReflectContext.Current.ReflectSectionCollection[sectionName];
                Print("SectionName=" + sectionName, section);
            }

            // Create by EntityName
            entity = ReflectContext.Current.CreateEntity<Entity>("samples/entities", "Entity001");
            Print("CreateEntity by Entity001", entity);

            entity = ReflectContext.Current.CreateEntity<Entity>("samples/entities", "Entity002");
            Print("CreateEntity by Entity002", entity);

            // Create by Default
            entity = ReflectContext.Current.CreateEntity<Entity>("samples/entities");
            Print("CreateEntity by Default", entity);
                        
            // Add Entity
            builder = new EntityBuilder();
            builder.Property001 = "77777";
            builder.Property002 = "88888";
            ReflectContext.Current.ReflectSectionCollection["clarkApp/providers"].Add("P001", builder);

            builder = new EntityBuilder();
            builder.Property002 = "99999";
            ReflectContext.Current.ReflectSectionCollection["clarkApp/providers"].Add("P002", builder);
            
            // End
            Console.WriteLine();
        }

        static void Print(string title, ReflectSection section)
        {
            #region Contracts

            if (string.IsNullOrEmpty(title) == true) throw new ArgumentNullException();
            if (section == null) throw new ArgumentNullException();

            #endregion

            // Title
            Console.WriteLine(title);

            // Default
            Console.WriteLine("Default=" + section.DefaultEntityName);

            // EntityName
            Console.WriteLine("EntityName");
            foreach (string entityName in section.Keys)
            {
                Console.WriteLine("=" + entityName);
            }
            Console.WriteLine();
        }

        static void Print(string title, Entity entity)
        {
            #region Contracts

            if (string.IsNullOrEmpty(title) == true) throw new ArgumentNullException();
            if (entity == null) throw new ArgumentNullException();

            #endregion

            // Title
            Console.WriteLine(title);

            // Print
            Console.WriteLine("Property001=" + entity.Property001);
            Console.WriteLine("Property002=" + entity.Property002);
            Console.WriteLine();
        }
    }
}
