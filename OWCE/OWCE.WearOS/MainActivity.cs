using System;

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.Wearable.Activity;

namespace OWCE.WearOS
{
    [Activity(Label = "OWCE", MainLauncher = true)]
    public class MainActivity : WearableActivity
    {
        TextView textView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_main);

            textView = FindViewById<TextView>(Resource.Id.text);
            SetAmbientEnabled();
        }
    }
}



