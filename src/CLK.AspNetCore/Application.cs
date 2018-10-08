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
        public static IWebHost Run(string baseUrl, IStartup startup, Action execute)
        {
            #region Contracts

            if (string.IsNullOrEmpty(baseUrl) == true) throw new ArgumentException();
            if (startup == null) throw new ArgumentException();
            if (execute == null) throw new ArgumentException();

            #endregion

            // Create
            IWebHost webHost = null;
            {
                // Builder
                webHost = new WebHostBuilder()

                // Services
                .ConfigureServices((services) =>
                {
                    // Add
                    services.AddSingleton<IStartup>(startup);
                })

                // Listen
                .UseUrls(baseUrl)

                // Build       
                .Build();
            }
            if (webHost == null) throw new InvalidOperationException("webHost=null");

            // Run 
            try
            {
                // Start
                webHost.Start();

                // Execute
                execute();
            }
            catch (Exception)
            {
                // Dispose
                webHost.Dispose();

                // Throw
                throw;
            }

            // Return
            return webHost;
        }
    }

    public partial class Application
    {
        // Methods
        public static void Run(string[] args)
        {
            // Main
            try
            {
                // AppSettings
                var appSettings = Application.GetAppSettings();
                if (appSettings == null) throw new InvalidOperationException();

                // Variables
                var appId = appSettings["appId"];
                var version = appSettings["version"];
                var baseUrl = appSettings["baseUrl"];

                // Setting
                Console.Title = string.Format("{0} ({1})", appId, version);
                Application.ShowConsole();
                Application.DisableCloseButton();
                Application.DisbleQuickEditMode();

                // Context
                using (var logContext = LoggerContext.Initialize(new Log4netLoggerFactory()))
                using (var autofacContext = new AutofacContext())                            
                {
                    // Logger
                    var logger = new Logger<Application>();
                    if (logger == null) throw new InvalidOperationException();

                    // Run
                    logger.Info("=========================================================");
                    logger.Info(string.Format("Application started: appId={0}, version={1}", appId, version));
                    using (CLK.AspNetCore.Application.Run(baseUrl, null, () =>
                    {
                        // Execute                    
                        var runEvent = new ManualResetEvent(false);
                        Console.WriteLine("Application started. Press Ctrl + C to shut down.");
                        Console.CancelKeyPress += (sender, eventArgs) => { runEvent.Set(); eventArgs.Cancel = true; };
                        runEvent.WaitOne();
                    })) { }
                    logger.Info("Application ended");
                    logger.Info("=========================================================");
                }
            }
            catch (Exception unknownException)
            {
                // Setting
                Application.ShowConsole();

                // InnerException
                while (unknownException.InnerException != null)
                {
                    unknownException = unknownException.InnerException;
                }
                Console.WriteLine(unknownException.Message);

                // Wait
                var runEvent = new ManualResetEvent(false);
                Console.WriteLine("Application ended. Press Ctrl + C to shut down.");
                Console.CancelKeyPress += (sender, eventArgs) => { runEvent.Set(); eventArgs.Cancel = true; };
                runEvent.WaitOne();
            }
        }

        private static IConfiguration GetAppSettings(string configFilename = @"appsettings.*")
        {
            #region Contracts

            if (string.IsNullOrEmpty(configFilename) == true) throw new ArgumentException();

            #endregion

            // ConfigFileList
            var configFileList = FileHelper.GetAllFile(configFilename);
            if (configFileList == null) throw new InvalidOperationException();
            
            // ConfigurationBuilder
            var configurationBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory());
            foreach (var configFile in configFileList)
            {
                // Require
                if (configFile.Extension.ToLower() != "json") continue;

                // Add
                configurationBuilder = configurationBuilder.AddJsonFile(configFile.Name, true, true);
            }

            // Configuration
            var configuration = configurationBuilder.Build();
            if (configuration == null) throw new InvalidOperationException();

            // Return
            return configuration;
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

        private static void DisbleQuickEditMode()
        {
            // Window
            IntPtr window = Application.GetStdHandle(STD_INPUT_HANDLE);
            if (window == IntPtr.Zero) throw new InvalidOperationException();

            // ConsoleMode
            uint consoleMode;
            Application.GetConsoleMode(window, out consoleMode);

            // Disble
            consoleMode &= ~ENABLE_QUICK_EDIT_MODE;
            Application.SetConsoleMode(window, consoleMode);
        }        
    }

    #endregion
}
