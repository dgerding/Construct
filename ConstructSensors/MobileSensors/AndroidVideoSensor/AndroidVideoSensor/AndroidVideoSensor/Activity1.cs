using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace AndroidVideoSensor
{
    [Activity(Label = "AndroidVideoSensor", MainLauncher = true, Icon = "@drawable/icon")]
    public class Activity1 : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            LinearLayout layout = FindViewById<LinearLayout>(Resource.Id.widget32);
            TextView aLabel = FindViewById<TextView>(Resource.Id.textViewMain);
            Button aButton = FindViewById<Button>(Resource.Id.button1);
            
            layout.Orientation = Orientation.Vertical;
            aLabel.Text = "asdfsdf";

            aButton.Click += (sender, e) =>
            {
                aLabel.Text = "I'm watchin' you! \n ashdbasd";
                StartActivity(typeof(CamerActivity));
            };
            SetContentView(layout);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            var item = menu.Add(0, 1, 1, Resource.String.Settings);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            StartActivity(typeof(settingsActivity));
            return true;
        }

    }
}

