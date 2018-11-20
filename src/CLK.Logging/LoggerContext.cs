using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Logging
{
    public class LoggerContext : IDisposable
    {
        // Fields
        private readonly IEnumerable<LoggerFactory> _loggerFactoryList = null;


        // Constructors
        public LoggerContext(IEnumerable<LoggerFactory> loggerFactoryList)
        {
            #region Contracts

            if (loggerFactoryList == null) throw new ArgumentException();

            #endregion

            // Default
            _loggerFactoryList = loggerFactoryList;
        }

        public void Start()
        {
            // Start
            foreach (var loggerFactory in _loggerFactoryList)
            {
                loggerFactory.Start();
            }
        }

        public void Dispose()
        {
            // Dispose
            foreach (var loggerFactory in _loggerFactoryList)
            {
                loggerFactory.Dispose();
            }
        }


        // Methods
        internal IEnumerable<Logger<TCategory>> Create<TCategory>()
        {
            // Result
            List<Logger<TCategory>> loggerList = new List<Logger<TCategory>>();

            // LoggerFactory
            foreach (var loggerFactory in _loggerFactoryList)
            {
                // Create
                var logger = loggerFactory.Create<TCategory>();
                if (logger == null) throw new InvalidOperationException("logger=null");

                // Add
                loggerList.Add(logger);
            }           

            // Return
            return loggerList;
        }
    }
}
