using System;
using System.Collections.Generic;
using System.Text;

namespace CLK.AspNetCore
{
    public class AspNetCoreOptions
    {
        // Constructors
        public AspNetCoreOptions(string listenUrl = @"http://*:5000", string controllerFileName = @"*.Services.dll")
        {
            #region Contracts

            if (string.IsNullOrEmpty(listenUrl) == true) throw new ArgumentException();
            if (string.IsNullOrEmpty(controllerFileName) == true) throw new ArgumentException();

            #endregion

            // Default
            this.ListenUrl = listenUrl;
            this.ControllerFileName = controllerFileName;
        }


        // Properties
        public string ListenUrl { get; set; }

        public string ControllerFileName { get; set; }
    }
}
