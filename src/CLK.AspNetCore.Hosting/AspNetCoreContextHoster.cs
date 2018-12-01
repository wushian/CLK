using System;
using System.Collections.Generic;
using System.Text;
using CLK.Platform;

namespace CLK.AspNetCore.Hosting
{
    public class AspNetCoreContextHoster : PlatformHoster
    {
        // Fields
        private readonly AspNetCoreContext _aspNetCoreContext = null;


        // Constructors
        public AspNetCoreContextHoster(AspNetCoreContext aspNetCoreContext)
        {
            #region Contracts

            if (aspNetCoreContext == null) throw new ArgumentException();

            #endregion

            // Default
            _aspNetCoreContext = aspNetCoreContext;
        }

        public void Start()
        {
            // Start
            _aspNetCoreContext.Start();
        }

        public void Dispose()
        {
            // Dispose
            _aspNetCoreContext.Dispose();
        }
    }
}
