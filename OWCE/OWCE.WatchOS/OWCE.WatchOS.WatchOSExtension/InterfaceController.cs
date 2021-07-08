using System;
using System.Collections.Generic;
using Foundation;
using WatchConnectivity;
using WatchKit;

namespace OWCE.WatchOS.WatchOSExtension
{
    public partial class InterfaceController : WKInterfaceController
    {
        protected InterfaceController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void Awake(NSObject context)
        {
            base.Awake(context);

            // Configure interface objects here.
            Console.WriteLine("{0} awake with context", this);

            // Register for notifications, eg when the phone updates new
            // values for speed, distance etc
            WCSessionManager.SharedManager.MessageReceived += DidReceiveMessage;
        }

        public override void WillActivate()
        {
            // This method is called when the watch view controller is about to be visible to the user.

            // Send message to Phone to tell it to update with latest values
            WCSessionManager.SharedManager.SendMessage(new Dictionary<string, object>() {
                { "WatchAppAwake", null } });
        }

        public override void DidDeactivate()
        {
            // This method is called when the watch view controller is no longer visible to the user.
        }

        // Called when the phone has new values to update on the watch display.
        // Messages are mostly sent from Watch.cs on phone side
        public void DidReceiveMessage(WCSession session, Dictionary<string, object> applicationContext)
        {   try
            {
                if (applicationContext.ContainsKey("MessagePhone"))
                {
                    var message = (string)applicationContext["MessagePhone"];
                    if (message != null)
                    {
                        Console.WriteLine($"Application context update received : {message}");
                        this.myLabel.SetText(message);
                    }
                }
                if (applicationContext.ContainsKey("BatteryPercent"))
                {
                    this.batteryPercentageLabel.SetText($"{applicationContext["BatteryPercent"]}%");
                }
                if (applicationContext.ContainsKey("Speed"))
                {
                    this.speedLabel.SetText(String.Format("{0:F0}", applicationContext["Speed"]));
                }
                if (applicationContext.ContainsKey("Voltage"))
                {
                    this.voltageLabel.SetText(String.Format("{0:F2}V battery", applicationContext["Voltage"]));
                }
                if (applicationContext.ContainsKey("Distance"))
                {
                    this.tripDistanceLabel.SetText((string)applicationContext["Distance"]);
                }
                if (applicationContext.ContainsKey("SpeedUnitsLabel"))
                {
                    // Update the speed units to mph or km/h
                    this.speedUnitsLabel.SetText((string)applicationContext["SpeedUnitsLabel"]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Processing Message: {ex.Message}");
                this.errorMessages.SetText($"Exception: {ex.Message}");
            }
        }
    }
}

