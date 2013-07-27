using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Settings.Samples
{
    class Program
    {
        // Methods
        public static void Main(string[] args)
        {
            // Sample
            ConfigSettingsSample();
            MemorySettingsSample();

            // End
            Console.ReadLine();
        }
                

        private static void ConfigSettingsSample()
        {
            // Initialize
            SettingContext.Current = new CLK.Settings.Configuration.ConfigSettingContext();

            // Run
            Run("Config Settings Sample");
        }

        private static void MemorySettingsSample()
        {
            // Initialize
            SettingContext.Current = new CLK.Settings.MemorySettingContext();
            CLK.Settings.SettingContext.Current.AppSettings["AppSetting001"] = "AppSetting from memory";
            CLK.Settings.SettingContext.Current.ConnectionStrings["ConnectionString001"] = "ConnectionStrings from memory";

            // Run
            Run("Memory Settings Sample");
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

            // Create
            Entity entity = EntityFactory.Create();

            // Operate            
            Console.WriteLine("Property001 : " + entity.Property001);
            Console.WriteLine("Property002 : " + entity.Property002);

            // End
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
