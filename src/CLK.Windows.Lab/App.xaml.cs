using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CLK.AspNetCore;
using CLK.Autofac;
using CLK.Logging;
using CLK.Logging.Log4net;

namespace CLK.Windows.Lab
{
    public partial class App : Application
    {
        // Constructors
        public App()
        {
            // Run
            this.Startup += (s, e) => { Program.Run(); };
        }
    }

    public partial class Program
    {
        // Methods
        public static void Run()
        {
            // Loger
            var loggerContext = LoggerContext.Initialize(new Log4netLoggerFactory());
            var logger = new Logger<Program>();

            // Execute  
            try
            {
                // AppSettings
                var appSettings = SettingsHelper.GetAllAppSettings();
                if (appSettings == null) throw new InvalidOperationException("appSettings=null");

                // Variables
                var appName = appSettings["appName"];
                var appVersion = appSettings["appVersion"];
                var baseUrl = @"http://*:5000";
                var hostingFilename = @"*.Hosting.json";
                var servicesFilename = @"*.Services.dll";

                // Context
                var autofacContext = new AutofacContext(hostingFilename);
                var aspnetContext = new AspnetContext(baseUrl, servicesFilename, autofacContext);
                var windowContext = new WindowContext(autofacContext);
                Action startAction = () =>
                {
                    aspnetContext.Start();
                    windowContext.Start();
                };
                Action endAction = () =>
                {
                    windowContext?.Dispose();
                    aspnetContext?.Dispose();
                    autofacContext?.Dispose();
                    loggerContext?.Dispose();
                };

                // Run  
                logger.Info("========================================");
                logger.Info(string.Format("Application started: appName={0}, appVersion={1}", appName, appVersion));
                startAction?.Invoke();
                Application.Current.Exit += (s, e) =>
                {
                    endAction?.Invoke();
                    logger.Info("Application ended");
                    logger.Info("========================================");
                };
                Application.Current.MainWindow.Title = string.Format("{0} ({1})", appName, appVersion);
            }
            catch (Exception exception)
            {
                // Error
                while (exception?.InnerException != null)
                {
                    exception = exception.InnerException;
                }
                logger.Info(string.Format("Application error: exception={0}", exception?.Message), exception);
                logger.Info("========================================");

                // Notify
                MessageBox.Show(string.Format("Application error: exception={0}", exception?.Message), "Application error");
            }
        }
    }
}
