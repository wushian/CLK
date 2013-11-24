using System;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
                _operateTimer = new PortableTimer(this.Timer_Ticked, 500);
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
