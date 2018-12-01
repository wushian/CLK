using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CLK.Logging;
using CLK.Services;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CLK.AspNetCore
{
    public class ActionLogFilter : IActionFilter
    {
        // Fields
        private readonly static string _executingTimeKey = "__" + typeof(ActionLogFilter).ToString() + "__";

        private readonly LoggerFactory _loggerFactory = null;


        // Constructors
        public ActionLogFilter(LoggerFactory loggerFactory)
        {
            #region Contracts

            if (loggerFactory == null) throw new ArgumentException();

            #endregion

            // Default
            _loggerFactory = loggerFactory;
        }


        // Methods
        public void OnActionExecuting(ActionExecutingContext context)
        {
            #region Contracts

            if (context == null) throw new ArgumentException();

            #endregion

            // Duration 
            var executingTime = DateTime.Now;
            context.HttpContext.Items.Add(_executingTimeKey, executingTime);

            // Descriptor
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (descriptor == null) throw new InvalidOperationException();

            // Logger
            var logger = this._loggerFactory.Create(descriptor.ControllerTypeInfo);
            if (logger == null) throw new InvalidOperationException();

            // Attribute
            var skipLogAttribute = descriptor.MethodInfo.GetCustomAttribute<SkipActionLogAttribute>();

            // Start
            if (skipLogAttribute == null)
            {
                logger.Debug("Action started", null, descriptor.MethodInfo.Name);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            #region Contracts

            if (context == null) throw new ArgumentException();

            #endregion

            // Duration 
            var executingTime = context.HttpContext.Items[_executingTimeKey] as DateTime?;
            var executedTime = DateTime.Now;
            var duration = executedTime - executingTime;

            // Descriptor
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (descriptor == null) throw new InvalidOperationException();

            // Logger
            var logger = _loggerFactory.Create(descriptor.ControllerTypeInfo);
            if (logger == null) throw new InvalidOperationException();

            // Attribute
            var skipLogAttribute = descriptor.MethodInfo.GetCustomAttribute<SkipActionLogAttribute>();

            // Exception
            var exception = context.Exception;
            while (exception?.InnerException != null)
            {
                exception = exception.InnerException;
            }

            // End
            if (exception == null && skipLogAttribute == null)
            {
                logger.Debug(string.Format("Action ended: duration={0}", duration?.Milliseconds), exception, descriptor.MethodInfo.Name);
            }

            // Error
            if (exception != null)
            {
                logger.Error(string.Format("Action error: duration={0}", duration?.Milliseconds), exception, descriptor.MethodInfo.Name);
            }
        }
    }
}
