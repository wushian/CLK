using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CLK.AspNetCore
{
    public class FileServerMiddlewareLauncher : MiddlewareLauncher
    {
        // Methods
        public void ConfigureMiddleware(IApplicationBuilder app)
        {
            #region Contracts

            if (app == null) throw new ArgumentException();

            #endregion

            // FileServer
            app.UseFileServer();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            #region Contracts

            if (services == null) throw new ArgumentException();

            #endregion

            // FileServer

        }
    }
}
