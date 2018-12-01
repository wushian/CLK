using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace CLK.AspNetCore
{
    public class UnhandledExceptionMiddlewareLauncher : MiddlewareLauncher
    {
        // Methods
        public void ConfigureMiddleware(IApplicationBuilder app)
        {
            #region Contracts

            if (app == null) throw new ArgumentException();

            #endregion

            // ExceptionHandler
            app.UseExceptionHandler(this.RegisterExceptionHandler);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            #region Contracts

            if (services == null) throw new ArgumentException();

            #endregion

            // ExceptionHandler

        }


        private void RegisterExceptionHandler(IApplicationBuilder app)
        {
            #region Contracts

            if (app == null) throw new ArgumentException();

            #endregion

            // Register
            app.Run(async context =>
            {
                // ExceptionHandlerFeature
                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (exceptionHandlerFeature == null) return;

                // Exception
                var exception = exceptionHandlerFeature.Error;
                while (exception?.InnerException != null)
                {
                    exception = exception.InnerException;
                }
                if (exception == null) return;

                // Response
                context.Response.Clear();
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                // Header
                await this.AddCorsHeader(context);

                // Write
                await context.Response.WriteAsync(
                    JsonConvert.SerializeObject(
                        new { error = exception.Message }
                    )
                );
            });
        }

        private async Task AddCorsHeader(HttpContext context)
        {
            #region Contracts

            if (context == null) throw new ArgumentException();

            #endregion

            // CorsService
            var corsService = context.RequestServices.GetService<ICorsService>();
            if (corsService == null) return;

            // CorsPolicy
            var corsPolicy = await context.RequestServices.GetService<ICorsPolicyProvider>()?.GetPolicyAsync(context, "Default");
            if (corsPolicy == null) return;

            // CorsResult
            var corsResult = corsService.EvaluatePolicy(context, corsPolicy);
            if (corsResult == null) return;

            // CorsResponse
            corsService.ApplyResult(
                corsResult,
                context.Response
            );
        }
    }
}
