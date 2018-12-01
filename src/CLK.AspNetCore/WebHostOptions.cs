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
        public WebHostOptions(AutofacScope autofacScope, string listenUrl = @"http://*:5000", IEnumerable<MiddlewareLauncher> middlewareLauncherList = null)
        {
            #region Contracts

            if (autofacScope == null) throw new ArgumentException(nameof(autofacScope));
            if (string.IsNullOrEmpty(listenUrl) == true) throw new ArgumentException(nameof(listenUrl));

            #endregion

            // Default
            this.AutofacScope = autofacScope;
            this.ListenUrl = listenUrl;

            // MiddlewareLauncherList
            if (middlewareLauncherList == null)
            {
                middlewareLauncherList = new List<MiddlewareLauncher>();
            }
            this.MiddlewareLauncherList = this.ConfigureMiddlewareLauncher(middlewareLauncherList.ToList());
        }


        // Properties
        public AutofacScope AutofacScope { get; private set; }

        public string ListenUrl { get; private set; }

        public List<MiddlewareLauncher> MiddlewareLauncherList { get; private set; }


        // Methods
        public TMiddlewareLauncher FindMiddlewareLauncher<TMiddlewareLauncher>() where TMiddlewareLauncher : MiddlewareLauncher
        {
            // Return
            return this.MiddlewareLauncherList.OfType<TMiddlewareLauncher>().FirstOrDefault();
        }

        protected virtual List<MiddlewareLauncher> ConfigureMiddlewareLauncher(List<MiddlewareLauncher> middlewareLauncherList)
        {
            #region Contracts

            if (middlewareLauncherList == null) throw new ArgumentException(nameof(middlewareLauncherList));

            #endregion

            // Return
            return middlewareLauncherList;
        }
    }
}
