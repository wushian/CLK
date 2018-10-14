using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CLK.Activities
{
    public class ShellModel : ViewModel
    {
        // Fields
        private readonly List<ActivityLauncher> _activityLauncherList = null;

        private readonly Uri _homeUri = null;

        private ActivityModel _activityModel = null;


        // Constructors
        public ShellModel(List<ActivityLauncher> activityLauncherList, string homeUri = null)
        {
            #region Contracts

            if (activityLauncherList == null) throw new ArgumentException();

            #endregion

            // Default
            _activityLauncherList = activityLauncherList;
            _homeUri = string.IsNullOrEmpty(homeUri) == false ? new Uri(homeUri) : null;
            _activityModel = null;

            // Commands
            this.GoHomeCommand = new RelayCommand(() => this.GoHome());
            this.NavigateCommand = new RelayCommand<Uri>((uri) => this.Navigate(uri));
        }

        public override void Start()
        {
            // Base
            base.Start();

            // Home
            this.GoHome();
        }

        public override void Dispose()
        {
            // Base
            base.Dispose();
        }


        // Properties
        public virtual IEnumerable<ActivityLauncher> ActivityLauncherList
        {
            get { return _activityLauncherList; }
        }

        public virtual Uri HomeUri
        {
            get { return _homeUri; }
        }

        public virtual ActivityModel ActivityModel
        {
            get { return _activityModel; }
            private set
            {
                // Detach
                if (_activityModel != null)
                {
                    this.DetachActivityModel(_activityModel);
                }

                // Attach
                if (value != null)
                {
                    this.AttachActivityModel(value);
                }

                // SetValue
                this.SetValue(ref _activityModel, value);
            }
        }
        

        // Commands
        public ICommand GoHomeCommand { get; private set; }

        public ICommand NavigateCommand { get; private set; }


        // Methods
        public void GoHome()
        {
            // HomeUri
            if (_homeUri != null)
            {
                // Navigate
                this.Navigate(_homeUri, null);
            }

            // First
            if (_homeUri == null && _activityLauncherList.Count > 0)
            {
                // Navigate
                this.Navigate(_activityLauncherList[0].ActivityUri, null);
            }
        }

        public void Navigate(Uri uri, Dictionary<string, object> bundle = null)
        {
            #region Contracts

            if (uri == null) throw new ArgumentException();

            #endregion

            // Navigate
            foreach (var activityLauncher in _activityLauncherList)
            {
                // Create
                var activityModel = activityLauncher.Create(uri, bundle);
                if (activityModel == null) continue;

                // Attach
                this.ActivityModel = activityModel;

                // Return
                return;
            }

            // NotFound
            this.ActivityModel = null;
        }
        

        protected virtual void AttachActivityModel(ActivityModel activityModel)
        {
            #region Contracts

            if (activityModel == null) throw new ArgumentException();

            #endregion

            // Attach
            activityModel.HomeNavigated += this.ActivityModel_HomeNavigated;
            activityModel.Navigated += this.ActivityModel_Navigated;
        }

        protected virtual void DetachActivityModel(ActivityModel activityModel)
        {
            #region Contracts

            if (activityModel == null) throw new ArgumentException();

            #endregion

            // Detach
            activityModel.HomeNavigated -= this.ActivityModel_HomeNavigated;
            activityModel.Navigated -= this.ActivityModel_Navigated;
        }


        // Handlers
        private void ActivityModel_HomeNavigated()
        {
            // Navigate
            this.GoHome();
        }

        private void ActivityModel_Navigated(Uri uri, Dictionary<string, object> bundle = null)
        {
            #region Contracts

            if (uri == null) throw new ArgumentException();

            #endregion

            // Navigate
            this.Navigate(uri, bundle);
        }
    }
}
