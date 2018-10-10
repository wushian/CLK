using CLK.Autofac;
using CLK.Logging;
using CLK.Logging.Log4net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CLK.AspNetCore
{
    public partial class Application
    {
        // Methods
        public static void Run()
        {
            try
            {
                // AppSettings
                var appSettings = SettingsHelper.GetAllAppSettings();
                if (appSettings == null) throw new InvalidOperationException("appSettings=null");

                // Variables
                var appId = appSettings["appId"];
                var appName = appSettings["appName"];
                var appVersion = appSettings["appVersion"];
                var baseUrl = appSettings["baseUrl"];                                
                var hostingFilename = @"*.Hosting.json";
                var servicesFilename = @"*.Services.dll";

                // Setting
                Console.Title = string.Format("{0} ({1})", appName, appVersion);
                Application.ShowConsole();
                Application.DisableCloseButton();
                Application.DisableQuickEditMode();

                // Context
                using (var autofacContext = new AutofacContext(hostingFilename))
                using (var loggerContext = LoggerContext.Initialize(new Log4netLoggerFactory()))                                            
                {
                    // Logger
                    var logger = new Logger<Application>();
                    if (logger == null) throw new InvalidOperationException();

                    // Application
                    logger.Info("========================================");
                    logger.Info(string.Format("Application started: appId={0}, appName={1}, appVersion={2}", appId, appName, appVersion));
                    using (var aspNetCoreContext = new AspNetCoreContext(baseUrl, servicesFilename, autofacContext))
                    {
                        // Wait                
                        var waitEvent = new ManualResetEvent(false);
                        Console.WriteLine("Application started. Press Ctrl + C to shut down.");
                        Console.CancelKeyPress += (sender, eventArgs) => { waitEvent.Set(); eventArgs.Cancel = true; };
                        waitEvent.WaitOne();
                    }
                    logger.Info("Application ended");
                    logger.Info("========================================");
                }
            }
            catch (Exception exception)
            {
                // Setting
                Application.ShowConsole();

                // Exception
                while (exception?.InnerException != null)
                {
                    exception = exception.InnerException;
                }
                Console.WriteLine(exception.Message);

                // Wait
                var waitEvent = new ManualResetEvent(false);
                Console.WriteLine("Application ended. Press Ctrl + C to shut down.");
                Console.CancelKeyPress += (sender, eventArgs) => { waitEvent.Set(); eventArgs.Cancel = true; };
                waitEvent.WaitOne();
            }
        }
    }

    #region Win32 API

    public partial class Application
    {
        // Imports
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int hConsoleHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint mode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint mode);


        // Constants
        private const int MF_BYCOMMAND = 0x00000000;

        private const int SC_CLOSE = 0xF060;

        private const int STD_INPUT_HANDLE = -10;

        private const uint ENABLE_QUICK_EDIT_MODE = 0x0040;


        // Methods
        private static void ShowConsole()
        {
            // Window
            var window = Application.GetConsoleWindow();
            if (window == IntPtr.Zero) throw new InvalidOperationException();

            // Show
            Application.ShowWindow(window, 5);
        }

        private static void HideConsole()
        {
            // Window
            var window = Application.GetConsoleWindow();
            if (window == IntPtr.Zero) throw new InvalidOperationException();

            // Hide
            Application.ShowWindow(window, 0);
        }

        private static void DisableCloseButton()
        {
            // Window
            var window = Application.GetConsoleWindow();
            if (window == IntPtr.Zero) throw new InvalidOperationException();

            // Menu
            var menu = Application.GetSystemMenu(window, false);
            if (menu == IntPtr.Zero) throw new InvalidOperationException();

            // Disable
            Application.DeleteMenu(menu, SC_CLOSE, MF_BYCOMMAND);
        }

        private static void DisableQuickEditMode()
        {
            // Window
            IntPtr window = Application.GetStdHandle(STD_INPUT_HANDLE);
            if (window == IntPtr.Zero) throw new InvalidOperationException();

            // ConsoleMode
            uint consoleMode;
            Application.GetConsoleMode(window, out consoleMode);

            // Disable
            consoleMode &= ~ENABLE_QUICK_EDIT_MODE;
            Application.SetConsoleMode(window, consoleMode);
        }        
    }

    #endregion
}
