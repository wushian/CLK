using System;
using System.Collections.Generic;
using System.Text;
using CLK.Autofac;
using System.Linq;
using System.Linq.Expressions;

namespace CLK.AspNetCore
{
    public class WebHostOptions
    {
        // Constructors
        public WebHostOptions(AutofacScope autofacScope, IEnumerable<MiddlewareLauncher> middlewareLauncherList, string listenUrl = @"http://*:5000")
        {
            #region Contracts

            if (autofacScope == null) throw new ArgumentException(nameof(autofacScope));
            if (middlewareLauncherList == null) throw new ArgumentException(nameof(middlewareLauncherList));
            if (string.IsNullOrEmpty(listenUrl) == true) throw new ArgumentException(nameof(listenUrl));

            #endregion

            // Default
            this.ListenUrl = listenUrl;
            this.AutofacScope = autofacScope;
            this.MiddlewareLauncherList = middlewareLauncherList;
        }


        // Properties
        public string ListenUrl { get; private set; }

        public AutofacScope AutofacScope { get; private set; }

        public IEnumerable<MiddlewareLauncher> MiddlewareLauncherList { get; private set; }


        // Methods
        public TMiddlewareLauncher FindMiddlewareLauncher<TMiddlewareLauncher>() where TMiddlewareLauncher : MiddlewareLauncher
        {
            // Return
            return this.MiddlewareLauncherList.OfType<TMiddlewareLauncher>().FirstOrDefault();
        }
    }
}
