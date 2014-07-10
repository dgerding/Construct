using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AndroidVideoSensor
{
    [Activity(Label = "AndroidVideoSensorSettings")]
    public class settingsActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Settings);
            // Create your application here
            
            Button saveBtn = 
                FindViewById<Button>(Resource.Id.btnOne);
            Button resetBtn = 
                FindViewById<Button>(Resource.Id.btnTwo);
            Android.Widget.EditText ipAdress =
                        FindViewById<EditText>(Resource.Id.editTxtView);
            CheckBox debugChkBox =
                FindViewById<CheckBox>(Resource.Id.chkBoxOne);
            RadioGroup rdoGrp =
                FindViewById<RadioGroup>(Resource.Id.rdoGrp);

            saveBtn.Click += (sender, e) =>
                {
                    SaveSetting("ipAdress", ipAdress.Text);
                    Toast.MakeText(this, "Toast!", ToastLength.Short).Show();
                };
            resetBtn.Click += (sender, e) =>
                {
                    rdoGrp.ClearCheck();
                    debugChkBox.Checked = false;
                    ipAdress.Text = "";
                };
        }

        protected void SaveSetting(string address, string value)
        {
            var preferences = GetSharedPreferences("test", FileCreationMode.Private);
            var editor = preferences.Edit();
            editor.PutString(address, value);
            editor.Commit();
        }
    }
}