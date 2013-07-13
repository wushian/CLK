using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Settings.Samples.WinConsole
{
    class Program
    {
        // Methods
        public static void Main(string[] args)
        {
            // Sample
            ConfigSample();
            MemorySample();

            // End
            Console.ReadLine();
        }

        private static void ConfigSample()
        {
            // Title
            Console.WriteLine("Config Sample");

            // Initialize
            SettingContext.Current = new CLK.Settings.Configuration.ConfigSettingContext();

            // Operate
            Entity entity = EntityFactory.Create();
            Console.WriteLine("Property001 : " + entity.Property001);
            Console.WriteLine("Property002 : " + entity.Property002);

            // End
            Console.WriteLine();
        }

        private static void MemorySample()
        {
            // Title
            Console.WriteLine("Memory Sample");

            // Initialize
            SettingContext.Current = new CLK.Settings.MemorySettingContext();
            CLK.Settings.SettingContext.Current.AppSettings["AppSetting001"] = "AppSetting from memory";
            CLK.Settings.SettingContext.Current.ConnectionStrings["ConnectionString001"] = "ConnectionStrings from memory";

            // Operate
            Entity entity = EntityFactory.Create();
            Console.WriteLine("Property001 : " + entity.Property001);
            Console.WriteLine("Property002 : " + entity.Property002);

            // End
            Console.WriteLine();
        }
    }
}
