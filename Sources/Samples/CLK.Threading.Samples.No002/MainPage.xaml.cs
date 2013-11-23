using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CLK.Threading.Samples.No002.Resources;
using System.Threading;

namespace CLK.Threading.Samples.No002
{
    public partial class MainPage : PhoneApplicationPage
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
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            lock (_syncRoot)
            {
                // Require
                if (_operateTimer != null) return;

                // Begin
                _operateTimer = new PortableTimer(this.Timer_Ticked, 900);
            }
        }

        private void PhoneApplicationPage_Unloaded(object sender, RoutedEventArgs e)
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