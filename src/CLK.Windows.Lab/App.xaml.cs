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
            // Setting
            this.ShutdownMode = ShutdownMode.OnLastWindowClose;

            // Events
            this.Startup += this.App_Startup;
        }


        // Handlers
        private void App_Startup(object sender, StartupEventArgs eventArgs)
        {
            // AppSettings
            var appSettings = SettingsHelper.GetAllAppSettings();
            if (appSettings == null) throw new InvalidOperationException("appSettings=null");

            // Variables
            var appId = appSettings["appId"];
            var appName = appSettings["appName"];
            var appVersion = appSettings["appVersion"];
            var baseUrl = @"http://*:5000";
            var hostingFilename = @"*.Hosting.json";
            var servicesFilename = @"*.Services.dll";

            // Context
            var loggerContext = LoggerContext.Initialize(new Log4netLoggerFactory());
            var autofacContext = new AutofacContext(hostingFilename);
            var aspnetContext = new AspnetContext(baseUrl, servicesFilename, autofacContext);

            // Logger
            var logger = new Logger<App>();

            // Start              
            logger.Info("========================================");
            logger.Info(string.Format("Program started: appId={0}, appName={1}, appVersion={2}", appId, appName, appVersion));
            aspnetContext.Start();

            // End
            this.Exit += (s,e) =>
            {
                // Dispose
                aspnetContext.Dispose();
                autofacContext.Dispose();
                loggerContext.Dispose();                
                logger.Info("Program ended");
                logger.Info("========================================");
            };
        }
    }
}
