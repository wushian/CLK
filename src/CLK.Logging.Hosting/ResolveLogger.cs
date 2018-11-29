using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace CLK.Logging.Hosting
{
    public class ResolveLogger<TCategory> : Logger<TCategory>
    {
        // Fields
        private readonly Logger<TCategory> _logger = null;


        // Constructors
        public ResolveLogger(LoggerContext loggerContext)
        {
            #region Contracts

            if (loggerContext == null) throw new ArgumentException(nameof(loggerContext));

            #endregion

            // Logger
            _logger = loggerContext.Create<TCategory>();
            if (_logger == null) throw new InvalidOperationException("_logger=null");
        }


        // Methods
        public void Debug(string message, Exception exception = null, [CallerMemberName]string methodName = "")
        {
            #region Contracts

            if (string.IsNullOrEmpty(message) == true) throw new ArgumentException();

            #endregion

            // Log
            _logger.Debug(message, exception, methodName);
        }

        public void Info(string message, Exception exception = null, [CallerMemberName]string methodName = "")
        {
            #region Contracts

            if (string.IsNullOrEmpty(message) == true) throw new ArgumentException();

            #endregion

            // Log
            _logger.Info(message, exception, methodName);
        }

        public void Warn(string message, Exception exception = null, [CallerMemberName]string methodName = "")
        {
            #region Contracts

            if (string.IsNullOrEmpty(message) == true) throw new ArgumentException();

            #endregion

            // Log
            _logger.Warn(message, exception, methodName);
        }

        public void Error(string message, Exception exception = null, [CallerMemberName]string methodName = "")
        {
            #region Contracts

            if (string.IsNullOrEmpty(message) == true) throw new ArgumentException();

            #endregion

            // Log
            _logger.Error(message, exception, methodName);
        }

        public void Fatal(string message, Exception exception = null, [CallerMemberName]string methodName = "")
        {
            #region Contracts

            if (string.IsNullOrEmpty(message) == true) throw new ArgumentException();

            #endregion

            // Log
            _logger.Fatal(message, exception, methodName);
        }
    }
}
