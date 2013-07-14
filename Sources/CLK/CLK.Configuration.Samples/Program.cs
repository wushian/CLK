using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Configuration.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            // Sample
            ReadConfigSample();

            // End
            Console.ReadLine();
        }


        private static void ReadConfigSample()
        {
            // Title
            Console.WriteLine("====================");
            Console.WriteLine("Read Config Sample");
            Console.WriteLine();

            // Open
            System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            EntityElementSection entityElementSection = configuration.GetSection("entities") as EntityElementSection;
            if (entityElementSection == null) throw new InvalidOperationException();

            // Print
            foreach (EntityElement entityElement in entityElementSection.EntityElementCollection)
            {
                Print(entityElement);
            }

            // End
            Console.WriteLine();
        }

        static void Print(EntityElement entityElement)
        {
            #region Contracts

            if (entityElement == null) throw new ArgumentException();

            #endregion

            // Name
            Console.WriteLine("Name = " + entityElement.Name);

            // FixedAttribute
            Console.WriteLine("FixedAttribute = " + entityElement.FixedAttribute);

            // FreeAttributes
            foreach (string key in entityElement.FreeAttributes.Keys)
            {
                Console.WriteLine(key + " = " + entityElement.FreeAttributes[key]);
            }

            // End
            Console.WriteLine();
        }
    }
}
