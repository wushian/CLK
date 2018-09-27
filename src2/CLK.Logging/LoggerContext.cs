using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Logging
{
    public partial class LoggerContext
    {
        // Singleton 
        private static LoggerContext _current;

        public static LoggerContext Current
        {
            get
            {
                // Require
                if (_current == null) throw new InvalidOperationException("_current=null");

                // Return
                return _current;
            }
        }

        public static void Initialize(LoggerContext context)
        {
            #region Contracts

            if (context == null) throw new ArgumentNullException();

            #endregion

            // Default
            _current = context;
        }
    }

    public partial class LoggerContext : IDisposable
    {
        // Fields
        private readonly LoggerFactory _loggerFactory = null;


        // Constructors
        public LoggerContext(LoggerFactory loggerFactory)
        {
            #region Contracts

            if (loggerFactory == null) throw new ArgumentException();

            #endregion

            // Default
            _loggerFactory = loggerFactory;
        }

        public void Dispose()
        {
            // LoggerFactory
            _loggerFactory.Dispose();
        }


        // Methods
        public Logger Create(Type target)
        {
            #region Contracts

            if (target == null) throw new ArgumentException();

            #endregion

            // Logger
            var logger = _loggerFactory.Create(target);
            if (logger == null) throw new InvalidOperationException("logger=null");

            // Return
            return logger;
        }
    }
}
