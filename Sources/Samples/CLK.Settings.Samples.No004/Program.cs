using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLK.Configuration.Settings;

namespace CLK.Settings.Samples.No004
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create
            SettingContext settingContext = new ConfigSettingContext();

            // Get
            Console.WriteLine("\nConnectionStrings");
            string connectionString = settingContext.ConnectionStrings["Database01"];
            Console.WriteLine(connectionString);

            Console.WriteLine("\nAppSettings");
            string argumentString = settingContext.AppSettings["Argument01"];
            Console.WriteLine(argumentString);

            // End
            Console.WriteLine("\n\nPress enter to end...");
            Console.ReadLine();
        }
    }
}
