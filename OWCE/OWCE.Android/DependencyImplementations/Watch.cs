using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Android.Gms.Wearable;
using OWCE.DependencyInterfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(OWCE.Droid.DependencyImplementations.Watch))]

namespace OWCE.Droid.DependencyImplementations
{
    public class Watch : Java.Lang.Object,
        IWatch,
        MessageClient.IOnMessageReceivedListener,
        CapabilityClient.IOnCapabilityChangedListener
    {
        ICollection<INode> connectedDevices;
        MessageClient messageClient;
        Dictionary<WatchMessage, object> messageCache = new Dictionary<WatchMessage, object>();

        public void SendWatchMessages(Dictionary<WatchMessage, object> messages)
        {
            foreach (var message in messages)
            {
                messageCache[message.Key] = message.Value;
            }

            // If there are no nodes we have nothing to send.
            if (connectedDevices.Any() == false)
            {
                return;
            }

            // Implement when we have Android Watch support
            //Debugger.Break();

            if (messages.ContainsKey(WatchMessage.Voltage))
            {
                if (messages[WatchMessage.Voltage] is float value)
                {
                    var data = BitConverter.GetBytes(value);
                    foreach (var connectedDevice in connectedDevices)
                    {
                        //System.Diagnostics.Debug.WriteLine($"Phone: Attempting to send to {node.DisplayName}");
                        messageClient.SendMessage(connectedDevice.Id, "/voltage", data);
                    }
                }

                // float
                //Debugger.Break();
            }
            else if (messages.ContainsKey(WatchMessage.Speed))
            {
                if (messages[WatchMessage.Speed] is int value)
                {
                    var data = BitConverter.GetBytes(value);
                    foreach (var connectedDevice in connectedDevices)
                    {
                        //System.Diagnostics.Debug.WriteLine($"Phone: Attempting to send to {node.DisplayName}");
                        messageClient.SendMessage(connectedDevice.Id, "/speed", data);
                    }
                }

                // int
                // Debugger.Break();
            }
            else if (messages.ContainsKey(WatchMessage.BatteryPercent))
            {
                if (messages[WatchMessage.BatteryPercent] is int value)
                {
                    var data = BitConverter.GetBytes(value);
                    foreach (var connectedDevice in connectedDevices)
                    {
                        //System.Diagnostics.Debug.WriteLine($"Phone: Attempting to send to {node.DisplayName}");
                        messageClient.SendMessage(connectedDevice.Id, "/battery_percent", data);
                    }
                }

                // int
                //Debugger.Break();
            }
            else if (messages.ContainsKey(WatchMessage.Distance))
            {
                if (messages[WatchMessage.Distance] is string value)
                {
                    var data = System.Text.Encoding.ASCII.GetBytes(value);
                    foreach (var connectedDevice in connectedDevices)
                    {
                        //System.Diagnostics.Debug.WriteLine($"Phone: Attempting to send to {node.DisplayName}");
                        messageClient.SendMessage(connectedDevice.Id, "/distance", data);
                    }
                }

                // string
                //Debugger.Break();
            }
            else if (messages.ContainsKey(WatchMessage.SpeedUnitsLabel))
            {
                Debugger.Break();
            }
            else if (messages.ContainsKey(WatchMessage.Awake))
            {
                Debugger.Break();
            }
        }

        void IWatch.ListenForWatchMessages(OWBoard board)
        {
            // Implement when we have Android Watch support
            //Debugger.Break();
            SetupCapability();
        }

        void IWatch.StopListeningForWatchMessages()
        {
            // Implement when we have Android Watch support
            Debugger.Break();
        }

        async void SetupCapability()
        {
            messageClient = WearableClass.GetMessageClient(Xamarin.Essentials.Platform.CurrentActivity);
            messageClient.AddListener(this);

            var capabilityInfo = await WearableClass.GetCapabilityClient(Xamarin.Essentials.Platform.CurrentActivity).GetCapabilityAsync("owce", CapabilityClient.FilterReachable);
            connectedDevices = capabilityInfo.Nodes;

            WearableClass.GetCapabilityClient(Xamarin.Essentials.Platform.CurrentActivity).AddListener(this, "owce");
        }


        #region CapabilityClient.IOnCapabilityChangedListener 
        public void OnCapabilityChanged(ICapabilityInfo capabilityInfo)
        {
            connectedDevices = capabilityInfo.Nodes;

            // TODO: Send messageCache
        }
        #endregion

        #region MessageClient.IOnMessageReceivedListener
        public void OnMessageReceived(IMessageEvent messageEvent)
        {
            System.Diagnostics.Debug.WriteLine("Phone: OnMessageReceived");
        }
        #endregion
    }
}
