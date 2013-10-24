using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLK.Configuration.Settings;

namespace CLK.Settings.Samples.No003
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create
            SettingContext settingContext = new ConfigSettingContext();

            // List
            Console.WriteLine("\nConnectionStrings");
            foreach (string key in settingContext.ConnectionStrings.Keys)
            {
                string connectionString = settingContext.ConnectionStrings[key];
                Console.WriteLine(connectionString);
            }            

            Console.WriteLine("\nAppSettings");
            foreach (string key in settingContext.AppSettings.Keys)
            {
                string argumentString = settingContext.AppSettings[key];
                Console.WriteLine(argumentString);
            }     

            // End
            Console.WriteLine("\n\nPress enter to end...");
            Console.ReadLine();
        }
    }
}
