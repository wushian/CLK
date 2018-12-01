using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CLK.AspNetCore
{
    public class CorsMiddlewareLauncher : MiddlewareLauncher
    {
        // Methods
        public void ConfigureMiddleware(IApplicationBuilder app)
        {
            #region Contracts

            if (app == null) throw new ArgumentException();

            #endregion

            // Cors
            app.UseCors("Default");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            #region Contracts

            if (services == null) throw new ArgumentException();

            #endregion

            // Cors
            services.AddCors(options => options.AddPolicy("Default", corsPolicyBuilder => corsPolicyBuilder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
            ));
        }
    }
}
