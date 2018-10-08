using System;
using System.Collections.Generic;
using System.Reflection;
using CLK.Logging;
using CLK.Services;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CLK.AspNetCore
{
    internal static class ActionLoggerExtension
    {
        // Methods
        public static void AddActionLogger(this FilterCollection filters)
        {
            #region Contracts

            if (filters == null) throw new ArgumentException();

            #endregion

            // Attach
            filters.Add(new ActionLoggerFilter());
        }


        // Class
        private class ActionLoggerFilter : IActionFilter
        {
            // Fields
            private readonly object _syncRoot = new object();

            private readonly Dictionary<Type, Logger> _loggerDictionary = new Dictionary<Type, Logger>();


            // Methods
            public void OnActionExecuting(ActionExecutingContext context)
            {
                #region Contracts

                if (context == null) throw new ArgumentException();

                #endregion

                // ActionDescriptor
                var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
                if (actionDescriptor == null) throw new InvalidOperationException();

                // Logger
                var logger = this.GetLogger(actionDescriptor.ControllerTypeInfo);
                if (logger == null) throw new InvalidOperationException();

                // IgnoreLogAttribute
                var ignoreLogAttribute = actionDescriptor.MethodInfo.GetCustomAttribute<IgnoreLogAttribute>();
                if (ignoreLogAttribute == null)
                {
                    // Log
                    logger.Debug("Action started", null, actionDescriptor.MethodInfo.Name);
                }
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
                #region Contracts

                if (context == null) throw new ArgumentException();

                #endregion
                
                // ActionDescriptor
                var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
                if (actionDescriptor == null) throw new InvalidOperationException();

                // Logger
                var logger = this.GetLogger(actionDescriptor.ControllerTypeInfo);
                if (logger == null) throw new InvalidOperationException();                

                // IgnoreLogAttribute
                var ignoreLogAttribute = actionDescriptor.MethodInfo.GetCustomAttribute<IgnoreLogAttribute>();
                if (ignoreLogAttribute == null)
                {
                    // Log
                    if (context.Exception == null)
                    {
                        logger.Debug("Action ended", context.Exception, actionDescriptor.MethodInfo.Name);
                    }
                    else
                    {
                        logger.Error("Action error", context.Exception, actionDescriptor.MethodInfo.Name);
                    }
                }
                else
                {
                    // Log
                    if (context.Exception != null)
                    {
                        logger.Error("Action error", context.Exception, actionDescriptor.MethodInfo.Name);
                    }
                }
            }
            
            private Logger GetLogger(Type category)
            {
                #region Contracts

                if (category == null) throw new ArgumentException();

                #endregion

                // Sync
                lock (_syncRoot)
                {
                    // Create
                    if (_loggerDictionary.ContainsKey(category) == false)
                    {
                        // LoggerTpye
                        Type loggerTpye = null;
                        loggerTpye = typeof(Logger<>);
                        loggerTpye = loggerTpye.MakeGenericType(category);

                        // Logger
                        var logger = Activator.CreateInstance(loggerTpye) as Logger;
                        if (logger == null) throw new InvalidOperationException("logger=null");

                        // Add
                        _loggerDictionary.Add(category, logger);
                    }

                    // Return
                    return _loggerDictionary[category];
                }
            }
        }
    }
}
