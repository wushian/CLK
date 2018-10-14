using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Activities
{
    public interface ActivityLauncher
    {
        // Properties
        Uri ActivityUri { get; }


        // Methods
        ActivityModel Create(Uri uri, Dictionary<string, object> bundle = null);
    }

    public abstract class ActivityLauncher<TActivityModel> : ActivityLauncher
        where TActivityModel : ActivityModel
    {
        // Constructors
        public ActivityLauncher(string activityUri = null)
        {
            // ActivityUri
            if(string.IsNullOrEmpty(activityUri)==true)
            {
                activityUri = string.Format("{0}://{1}/{2}", "shell", typeof(TActivityModel).Namespace, typeof(TActivityModel).Name);
            }
            this.ActivityUri = new Uri(activityUri.Replace("*", ""));
        }


        // Properties
        public Uri ActivityUri { get; private set; }


        // Methods
        public ActivityModel Create(Uri uri, Dictionary<string, object> bundle = null)
        {
            #region Contracts

            if (uri == null) throw new ArgumentException();

            #endregion

            // Scheme
            if (Uri.Compare(uri, this.ActivityUri, UriComponents.Scheme, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) != 0) return null;

            // HostAndPort
            if (string.IsNullOrEmpty(this.ActivityUri.Host) == false)
            {
                if (Uri.Compare(uri, this.ActivityUri, UriComponents.HostAndPort, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) != 0) return null;
            }

            // Path
            if (string.IsNullOrEmpty(this.ActivityUri.LocalPath) == false && this.ActivityUri.LocalPath != @"/" && this.ActivityUri.LocalPath != @"\")
            {
                if (uri.LocalPath.StartsWith(this.ActivityUri.LocalPath, StringComparison.OrdinalIgnoreCase) == false) return null;
            }

            // CreateModel
            return this.CreateModel(uri, bundle);
        }

        protected abstract TActivityModel CreateModel(Uri uri, Dictionary<string, object> bundle = null);
    }
}
