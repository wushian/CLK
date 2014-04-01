using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace CLK.Settings.Samples.No007
{
    [Activity(Label = "CLK.Settings.Samples.No007", MainLauncher = true, Icon = "@drawable/icon")]
    public class Activity1 : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            // Base
            base.OnCreate(bundle);

            // View
            SetContentView(Resource.Layout.Main);

            // Context
            SettingContext.Current = new IsolatedSettingContext();
        }
    }
}

