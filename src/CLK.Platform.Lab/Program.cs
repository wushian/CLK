using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLK.Logging;
using CLK.Transactions;

namespace CLK.Platform.Lab
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var platformContext = new PlatformContext())
            {
                // Start
                platformContext.Start();

                // Logger
                var logger = platformContext.Resolve<Logger<Program>>();
                logger.Debug("Clark");

                // TransactionScope
                using (var transactionScope = new TransactionScope())
                {
                    transactionScope.Complete();
                }

                // End
                Console.ReadLine();
            }
        }
    }
}
