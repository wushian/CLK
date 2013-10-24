using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLK.Configuration.Settings;

namespace CLK.Settings.Samples.No001
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create
            SettingContext settingContext = new ConfigSettingContext();

            // End
            Console.WriteLine("\n\nPress enter to end...");
            Console.ReadLine();
        }
    }
}
