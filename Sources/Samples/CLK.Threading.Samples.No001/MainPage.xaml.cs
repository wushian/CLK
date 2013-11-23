using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace CLK.Threading.Samples.No001
{
    public sealed partial class MainPage : Page
    {
        // Fields
        private readonly object _syncRoot = new object();

        private readonly SynchronizationContext _syncContext = null;

        private PortableTimer _operateTimer = null;


        // Constructors
        public MainPage()
        {
            // Base
            this.InitializeComponent();

            // SyncContext
            _syncContext = SynchronizationContext.Current;
        }


        // Handlers
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            lock (_syncRoot)
            {
                // Require
                if (_operateTimer != null) return;

                // Begin
                _operateTimer = new PortableTimer(this.Timer_Ticked, 900);
            }
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            lock (_syncRoot)
            {
                // Require
                if (_operateTimer == null) return;

                // End
                _operateTimer.Dispose();
                _operateTimer = null;
            }
        }

        private void Timer_Ticked()
        {
            System.Threading.SendOrPostCallback methodDelegate = delegate(object state)
            {
                // Display            
                this.TextBlock001.Text = DateTime.Now.ToString();
            };
            _syncContext.Post(methodDelegate, null);
        }
    }
}
