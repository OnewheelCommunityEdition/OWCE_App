using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Foundation;
using WatchConnectivity;
using WatchKit;

namespace OWCE.WatchOS.WatchOSExtension
{
    public partial class InterfaceController : WKInterfaceController
    {
        private CancellationTokenSource source = new CancellationTokenSource();

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

            // Check whether session is active
            if (!WCSessionManager.SharedManager.IsReachable())
            {
                // If session is not reachable, we should display a message
                // reminding the user to connect the phone to the board.
                ShowConnectToBoardViaPhoneMessage();
            }

            // Send message to Phone to tell it to update with latest values
            WCSessionManager.SharedManager.SendMessage(new Dictionary<string, object>() {
                { "WatchAppAwake", null } });

            // If we don't hear back from the phone within 3 secs, then assume
            // the phone is disconnected from the board.
            var oldSource = source;
            source = new CancellationTokenSource();
            oldSource.Dispose();
            var t = Task.Run(async delegate
                {
                    await Task.Delay(3000, source.Token);
                    ShowConnectToBoardViaPhoneMessage();
                });
        }

        public override void DidDeactivate()
        {
            // This method is called when the watch view controller is no longer visible to the user.
            source.Cancel();
        }

        // Called when the phone has new values to update on the watch display.
        // Messages are mostly sent from Watch.cs on phone side
        public void DidReceiveMessage(WCSession session, Dictionary<string, object> applicationContext)
        {   try
            {
                // Since we got a response from the phone, cancel the timer
                // task that would have prompted the user to connect to the board on the phone
                source.Cancel();

                // Make sure we hide the "Connect to board" message and show the
                // ride details.
                HideConnectToBoardViaPhoneMessage();

                // Now process the message itself
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

        // Displays the "Please connect the phone to the board" message
        // and hides the ride details (eg speed, battery, distance)
        private void ShowConnectToBoardViaPhoneMessage()
        {
            this.connectToBoardGroup.SetHidden(false);
            this.rideDetailsGroup.SetHidden(true);
        }

        // Does the opposite of ShowConnectToBoardViaPhoneMessage()
        private void HideConnectToBoardViaPhoneMessage()
        {
            this.connectToBoardGroup.SetHidden(true);
            this.rideDetailsGroup.SetHidden(false);

        }
    }
}

