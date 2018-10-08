using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CLK.AspNetCore
{
    internal static class ExceptionMiddlewareExtension
    {
        // Methods
        public static void UseExceptionMiddleware(this IApplicationBuilder builder)
        {
            #region Contracts

            if (builder == null) throw new ArgumentException();

            #endregion

            // UseExceptionHandler
            builder.UseExceptionHandler((app)=>
            {        
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

                    // Write
                    await context.Response.WriteAsync(
                        JsonConvert.SerializeObject(
                            new { message = exception.Message }
                        )
                    );
                });
            });
        }
    }
}
