using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using CLK.Activities;
using CLK.Autofac;

namespace CLK.Windows
{
    public class WindowContext : IDisposable
    {
        // Fields
        private WindowBuilder _windowBuilder = null;

        private System.Windows.Window _window = null;


        // Constructors
        public WindowContext(AutofacContext autofacContext)
        {
            #region Contracts

            if (autofacContext == null) throw new ArgumentException();

            #endregion

            // WindowBuilder
            _windowBuilder = new WindowBuilder(autofacContext);
        }

        public void Start()
        {
            // Require
            if (_window != null) return;

            // Window
            _window = _windowBuilder.Create();
            if (_window == null) throw new InvalidOperationException("_window=null");

            // Start
            _window.Show();
        }

        public void Dispose()
        {
            // Require
            if (_window == null) return;

            // Dispose
            _window.Close();
        }


        // Class
        private class WindowBuilder 
        {
            // Fields
            private readonly AutofacContext _autofacContext = null;


            // Constructors
            public WindowBuilder(AutofacContext autofacContext)
            {
                #region Contracts

                if (autofacContext == null) throw new ArgumentException();

                #endregion

                // Default
                _autofacContext = autofacContext;
            }


            // Methods
            public System.Windows.Window Create()
            {
                // Require
                if (Application.Current == null) throw new InvalidOperationException("Application.Current=null");

                // Shell
                Shell shell = null;
                if (_autofacContext.Container.IsRegistered<Shell>() == true && _autofacContext.Container.IsRegistered<ShellModel>() == true)
                {
                    shell = _autofacContext.Container.Resolve<Shell>();
                    shell.DataContext = _autofacContext.Container.Resolve<ShellModel>();
                }

                // Window
                System.Windows.Window window = null;
                if (_autofacContext.Container.IsRegistered<CLK.Activities.Window>() == true && _autofacContext.Container.IsRegistered<WindowModel>() == true)
                {
                    window = _autofacContext.Container.Resolve<CLK.Activities.Window>();
                    window.DataContext = _autofacContext.Container.Resolve<WindowModel>();
                }
                if (window == null) window = new System.Windows.Window();

                // Setting
                window.Content = shell;

                // Return
                return window;
            }
        }
    }
}
