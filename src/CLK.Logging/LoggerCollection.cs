using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace CLK.Logging
{
    public class LoggerCollection<TCategory> : Logger<TCategory>
    {
        // Fields
        private readonly IEnumerable<Logger<TCategory>> _loggerList = null;


        // Constructors
        public LoggerCollection(LoggerContext loggerContext)
        {
            #region Contracts

            if (loggerContext == null) throw new ArgumentException();

            #endregion

            // LoggerList
            _loggerList = loggerContext.Create<TCategory>();
            if (_loggerList == null) throw new InvalidOperationException("_loggerList=null");
        }


        // Methods
        public void Debug(string message, Exception exception = null, [CallerMemberName]string methodName = "")
        {
            #region Contracts

            if (string.IsNullOrEmpty(message) == true) throw new ArgumentException();

            #endregion

            // Exception
            while (exception?.InnerException != null)
            {
                exception = exception.InnerException;
            }

            // Log
            foreach (var logger in _loggerList)
            {
                logger.Debug(message, exception, methodName);
            }
        }

        public void Info(string message, Exception exception = null, [CallerMemberName]string methodName = "")
        {
            #region Contracts

            if (string.IsNullOrEmpty(message) == true) throw new ArgumentException();

            #endregion

            // Exception
            while (exception?.InnerException != null)
            {
                exception = exception.InnerException;
            }

            // Log
            foreach (var logger in _loggerList)
            {
                logger.Info(message, exception, methodName);
            }
        }

        public void Warn(string message, Exception exception = null, [CallerMemberName]string methodName = "")
        {
            #region Contracts

            if (string.IsNullOrEmpty(message) == true) throw new ArgumentException();

            #endregion

            // Exception
            while (exception?.InnerException != null)
            {
                exception = exception.InnerException;
            }

            // Log
            foreach (var logger in _loggerList)
            {
                logger.Warn(message, exception, methodName);
            }
        }

        public void Error(string message, Exception exception = null, [CallerMemberName]string methodName = "")
        {
            #region Contracts

            if (string.IsNullOrEmpty(message) == true) throw new ArgumentException();

            #endregion

            // Exception
            while (exception?.InnerException != null)
            {
                exception = exception.InnerException;
            }

            // Log
            foreach (var logger in _loggerList)
            {
                logger.Error(message, exception, methodName);
            }
        }

        public void Fatal(string message, Exception exception = null, [CallerMemberName]string methodName = "")
        {
            #region Contracts

            if (string.IsNullOrEmpty(message) == true) throw new ArgumentException();

            #endregion

            // Exception
            while (exception?.InnerException != null)
            {
                exception = exception.InnerException;
            }

            // Log
            foreach (var logger in _loggerList)
            {
                logger.Fatal(message, exception, methodName);
            }
        }
    }
}
