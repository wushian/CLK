using System;
using System.Collections.Generic;
using System.Text;

namespace CLK.AspNetCore
{
    public static class AspnetContextExtensionExtension
    {
        // Methods
        public static void Run(this AspnetContext context, Action execute)
        {
            #region Contracts

            if (context == null) throw new ArgumentException();
            if (execute == null) throw new ArgumentException();

            #endregion

            // Run 
            try
            {
                // Start
                context.Start();

                // Execute
                execute();
            }
            catch (Exception)
            {
                // Dispose
                context.Dispose();

                // Throw
                throw;
            }
        }
    }
}
