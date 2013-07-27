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

            // Create by Config
            entity = ReflectContext.Current.CreateEntity<Entity>("CLK.Reflection.Samples/entities");
            Print("by Default", entity);

            // Create by Name
            entity = ReflectContext.Current.CreateEntity<Entity>("CLK.Reflection.Samples/entities", "Entity001");
            Print("by Entity001", entity);

            entity = ReflectContext.Current.CreateEntity<Entity>("CLK.Reflection.Samples/entities", "Entity002");
            Print("by Entity002", entity);

            // End
            Console.WriteLine();
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
            Console.WriteLine("Property001 : " + entity.Property001);
            Console.WriteLine("Property002 : " + entity.Property002);
            Console.WriteLine();
        }
    }
}
