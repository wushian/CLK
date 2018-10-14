using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CLK.Activities
{
    public abstract class ActivityModel : ViewModel
    {
        // Constructors
        public ActivityModel()
        {
            // Commands
            this.GoHomeCommand = new RelayCommand(() => this.GoHome());
            this.NavigateCommand = new RelayCommand<Uri>((uri) => this.Navigate(uri));
        }


        // Commands
        public ICommand GoHomeCommand { get; private set; }

        public ICommand NavigateCommand { get; private set; }


        // Methods
        public void GoHome()
        {
            // Notify
            this.OnHomeNavigated();
        }

        public void Navigate(Uri uri, Dictionary<string, object> bundle = null)
        {
            #region Contracts

            if (uri == null) throw new ArgumentException();

            #endregion

            // Notify
            this.OnNavigated(uri, bundle);
        }


        // Events
        public event Action HomeNavigated;
        private void OnHomeNavigated()
        {
            // Raise
            var handler = this.HomeNavigated;
            if (handler != null)
            {
                handler();
            }
        }

        public event Action<Uri, Dictionary<string, object>> Navigated;
        private void OnNavigated(Uri uri, Dictionary<string, object> bundle = null)
        {
            #region Contracts

            if (uri == null) throw new ArgumentException();

            #endregion

            // Raise
            var handler = this.Navigated;
            if (handler != null)
            {
                handler(uri, bundle);
            }
        }
    }
}
