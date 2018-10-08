using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static log4net.Appender.RollingFileAppender;

namespace CLK.Logging.Log4net
{
    public class Log4netLoggerFactory : LoggerFactory
    {
        // Constructors
        public Log4netLoggerFactory(string configFilename = null)
        {
            // Setting
            log4net.MDC.Set("tab", "\t");

            // ConfigFileList
            List<FileInfo> configFileList = new List<FileInfo>();
            if (string.IsNullOrEmpty(configFilename) == false)
            {
                // GetAll
                configFileList = this.GetAllFileInfo(configFilename);
                if (configFileList == null) throw new InvalidOperationException("configFileList=null");
            }

            // Configure
            if (configFileList.Count > 0)
            {
                // Repository
                var repository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
                if (repository == null) throw new InvalidOperationException("repository=null");

                // XmlConfigurator
                foreach (var configFile in configFileList)
                {
                    XmlConfigurator.Configure(repository, configFile);
                }
            }
            else
            {
                // Repository
                var repository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
                if (repository == null) throw new InvalidOperationException("repository=null");

                // PatternLayout
                var patternLayout = new log4net.Layout.PatternLayout()
                {
                    ConversionPattern = @"%date{yyyy-MM-dd HH:mm:ss fff} %-5level [%thread] %logger.%property{method}() - %message%newline%X{tab}"
                };
                patternLayout.ActivateOptions();

                // ConsoleAppender
                var consoleAppender = new log4net.Appender.ConsoleAppender()
                {
                    Layout = patternLayout
                };
                consoleAppender.ActivateOptions();

                // FileAppender
                var fileAppender = new log4net.Appender.RollingFileAppender()
                {
                    Layout = patternLayout,
                    RollingStyle = RollingMode.Date,
                    File = string.Format("log/{0}", System.Reflection.Assembly.GetEntryAssembly().GetName().Name),
                    DatePattern = " yyyy-MM-dd'.log'",
                    StaticLogFileName = false,
                    AppendToFile = true
                };
                fileAppender.ActivateOptions();

                // BasicConfigurator
                BasicConfigurator.Configure(repository, 
                    consoleAppender, 
                    fileAppender
                );
            }
        }

        public void Dispose()
        {

        }


        // Methods
        public Logger Create(Type target)
        {
            #region Contracts

            if (target == null) throw new ArgumentException();

            #endregion

            // Logger
            var logger = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), target);
            if (logger == null) throw new InvalidOperationException("logger=null");

            // Return
            return new Log4netLogger(logger);
        }

        private List<FileInfo> GetAllFileInfo(string filename)
        {
            #region Contracts

            if (string.IsNullOrEmpty(filename) == true) throw new ArgumentException();

            #endregion

            // Result
            Dictionary<string, FileInfo> resultFileDictionary = new Dictionary<string, FileInfo>();

            // SearchPatternList
            var searchPatternList = filename.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (searchPatternList == null) throw new InvalidOperationException();
            if (searchPatternList.Length <= 0) throw new InvalidOperationException();

            // EntryAssembly             
            foreach (var searchPattern in searchPatternList)
            {
                // GetFiles 
                DirectoryInfo fileDirectory = new DirectoryInfo(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location));
                FileInfo[] fileList = fileDirectory?.GetFiles(searchPattern, SearchOption.AllDirectories);
                if (fileList == null) throw new InvalidOperationException();

                // Add
                foreach (var file in fileList)
                {
                    if (resultFileDictionary.ContainsKey(file.Name.ToLower()) == false)
                    {
                        resultFileDictionary.Add(file.Name.ToLower(), file);
                    }
                }
            }

            // CurrentDirectory         
            foreach (var searchPattern in searchPatternList)
            {
                // GetFiles 
                DirectoryInfo fileDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
                FileInfo[] fileList = fileDirectory?.GetFiles(searchPattern, SearchOption.AllDirectories);
                if (fileList == null) throw new InvalidOperationException();

                // Add
                foreach (var file in fileList)
                {
                    if (resultFileDictionary.ContainsKey(file.Name.ToLower()) == false)
                    {
                        resultFileDictionary.Add(file.Name.ToLower(), file);
                    }
                }
            }

            // Return
            return resultFileDictionary.Values.ToList();
        }
    }
}
