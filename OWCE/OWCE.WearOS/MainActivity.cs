using System;

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.Wearable.Activity;
using Android.Gms.Wearable;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

namespace OWCE.WearOS
{
    [Activity(Label = "OWCE", MainLauncher = true)]
    public class MainActivity : WearableActivity,
        MessageClient.IOnMessageReceivedListener,
        CapabilityClient.IOnCapabilityChangedListener
    {
        //TextView textView;

        ICollection<INode> nodes;

        TextView notConnectedWarningTextView;
        TextView speedTextView;
        TextView batteryPercentTextView;
        TextView voltageTextView;
        TextView distanceTextView;
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_main);

            notConnectedWarningTextView = FindViewById<TextView>(Resource.Id.not_connected_warning);
            speedTextView = FindViewById<TextView>(Resource.Id.speed);
            batteryPercentTextView = FindViewById<TextView>(Resource.Id.battery_percent);
            voltageTextView = FindViewById<TextView>(Resource.Id.voltage);
            distanceTextView = FindViewById<TextView>(Resource.Id.distance);
            


            SetAmbientEnabled();

            SetupCapability();
        }

        void SetupCapability()
        {
            WearableClass.GetMessageClient(this).AddListener(this);
            //await WearableClass.GetMessageClient(this).AddListenerAsync(this);

            //var capabilityInfo = await WearableClass.GetCapabilityClient(this).GetCapabilityAsync("owce", CapabilityClient.FilterReachable);
            //nodes = capabilityInfo.Nodes;

            WearableClass.GetCapabilityClient(this).AddListener(this, "owce");
        }


        #region CapabilityClient.IOnCapabilityChangedListener
        public void OnCapabilityChanged(ICapabilityInfo capabilityInfo)
        {
            nodes = capabilityInfo.Nodes;
            Debugger.Break();
        }
        #endregion

        #region MessageClient.IOnMessageReceivedListener
        public void OnMessageReceived(IMessageEvent messageEvent)
        {
            if (notConnectedWarningTextView.Visibility == Android.Views.ViewStates.Visible)
            {
                notConnectedWarningTextView.Visibility = Android.Views.ViewStates.Gone;
                speedTextView.Visibility = Android.Views.ViewStates.Visible;
                batteryPercentTextView.Visibility = Android.Views.ViewStates.Visible;
                voltageTextView.Visibility = Android.Views.ViewStates.Visible;
                distanceTextView.Visibility = Android.Views.ViewStates.Visible;
            }


            var data = messageEvent.GetData();

            if (messageEvent.Path == "/speed")
            {
                var value = BitConverter.ToInt32(data);
                speedTextView.Text = $"{value}";
                //Debugger.Break();
            }
            else if (messageEvent.Path == "/voltage")
            {
                var value = BitConverter.ToSingle(data);
                voltageTextView.Text = $"{value:2}V";
                //Debugger.Break();
            }
            else if (messageEvent.Path == "/battery_percent")
            {
                var value = BitConverter.ToInt32(data);
                batteryPercentTextView.Text = $"{value}%";
                //Debugger.Break();
            }
            else if (messageEvent.Path == "/distance")
            {
                var value = System.Text.Encoding.ASCII.GetString(data);
                distanceTextView.Text = value;
                //Debugger.Break();

            }
            else if (messageEvent.Path == "/awake")
            {

            }
            else if (messageEvent.Path == "/speed_units_label")
            {

            }
        }
        #endregion
    }
}



