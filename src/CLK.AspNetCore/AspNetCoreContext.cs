using System;
using System.IO;
using Autofac.Extensions.DependencyInjection;
using CLK.Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace CLK.AspNetCore
{
    public class AspNetCoreContext : IDisposable
    {
        // Fields
        private WebHostBuilder _webHostBuilder = null;

        private IWebHost _webHost = null;


        // Constructors
        public AspNetCoreContext(WebHostOptions webHostOptions)
        {
            #region Contracts

            if (webHostOptions == null) throw new ArgumentException();

            #endregion

            // WebHostBuilder
            _webHostBuilder = new WebHostBuilder(webHostOptions);
        }

        public void Start()
        {
            // Require
            if (_webHost != null) return;

            // WebHost
            _webHost = _webHostBuilder.Build();
            if (_webHost == null) throw new InvalidOperationException("_webHost=null");

            // Start
            _webHost.Start();
        }

        public void Dispose()
        {
            // Require
            if (_webHost == null) return;

            // Dispose
            _webHost.Dispose();
        }
    }
}
