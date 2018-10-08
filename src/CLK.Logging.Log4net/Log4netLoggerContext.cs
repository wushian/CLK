using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Logging.Log4net
{
    public class Log4netLoggerContext : LoggerContext
    {
        // Constructors
        public Log4netLoggerContext(string configFilename = null) : base(new Log4netLoggerFactory(configFilename))
        {

        }
    }
}
