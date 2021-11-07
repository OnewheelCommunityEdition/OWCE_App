using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Foundation;
using UIKit;
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

        private bool darkMode = true;

        public override void WillActivate()
        {
            // This method is called when the watch view controller is about to be visible to the user.

            // Register for messages, eg when the phone updates new
            // values for speed, distance etc
            WCSessionManager.SharedManager.MessageReceived += DidReceiveMessage;

            // Check whether session is active
            if (!WCSessionManager.SharedManager.IsReachable())
            {
                // If session is not reachable, we should display a message
                // reminding the user to connect the phone to the board.
                ShowConnectToBoardViaPhoneMessage();
            }

            // Send message to Phone to tell it to update with latest values
            WCSessionManager.SharedManager.SendMessage(new Dictionary<WatchMessage, object>() {
                { WatchMessage.Awake, 0 } });

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

            // Unsubscribe to messages, so that we don't inadvertently act on
            // phone messages while the watch app is inactive.
            WCSessionManager.SharedManager.MessageReceived -= DidReceiveMessage;
        }

        // Called when the phone has new values to update on the watch display.
        // Messages are mostly sent from Watch.cs on phone side
        public void DidReceiveMessage(WCSession session, Dictionary<WatchMessage, object> messages)
        {
            try
            {
                // Since we got a response from the phone, cancel the timer
                // task that would have prompted the user to connect to the board on the phone
                source.Cancel();

                // Make sure we hide the "Connect to board" message and show the
                // ride details.
                HideConnectToBoardViaPhoneMessage();

                // Now process the message itself
                foreach (var message in messages)
                {
                    /*
                    if (message.Key == WatchMessage.MessagePhone)
                    {
                        var messageString = message.Value as string;
                        if (messageString != null)
                        {
                            System.Diagnostics.Debug.WriteLine($"Application context update received : {messageString}");
                            this.myLabel.SetText(messageString);
                        }
                    }
                    else */
                    if (message.Key == WatchMessage.BatteryPercent)
                    {
                        this.batteryPercentageLabel.SetText($"{message.Value}%");
                    }
                    else if(message.Key == WatchMessage.Speed)
                    {
                        this.speedLabel.SetText(String.Format("{0:F0}", message.Value));
                    }
                    else if(message.Key == WatchMessage.Voltage)
                    {
                        this.voltageLabel.SetText(String.Format("{0:F2}V", message.Value));
                    }
                    else if(message.Key == WatchMessage.Distance)
                    {
                        this.tripDistanceLabel.SetText(message.Value as string);
                    }
                    else if(message.Key == WatchMessage.SpeedUnitsLabel)
                    {
                        // Update the speed units to mph or km/h
                        this.speedUnitsLabel.SetText(message.Value as string);
                    }
                }
                
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception Processing Message: {ex.Message}");
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

        private void toggleBatteryPressed()
        {
            if (darkMode)
            {
                // Set Battery Labels to Light Mode
                this.batteryLabelGroup.SetBackgroundColor(UIColor.Magenta);
                this.batteryPercentageLabel.SetTextColor(UIColor.White);
                this.voltageLabel.SetTextColor(UIColor.White);

                // Set Speed Labels to Light mode
                this.speedLabelGroup.SetBackgroundColor(UIColor.Yellow);
                this.speedLabel.SetTextColor(UIColor.Black);
                this.speedUnitsLabel.SetTextColor(UIColor.Black);

                darkMode = false;
            }
            else
            {
                // Set Battery Labels to Dark Mode
                this.batteryLabelGroup.SetBackgroundColor(UIColor.Black);
                // Note: the battery labels are a whiter version of
                // Magenta to make it more readable
                this.batteryPercentageLabel.SetTextColor(UIColor.FromRGB(255, 100, 255));
                this.voltageLabel.SetTextColor(UIColor.FromRGB(255, 100, 255));

                // Set Speed Labels to Dark Mode
                this.speedLabelGroup.SetBackgroundColor(UIColor.Black);
                this.speedLabel.SetTextColor(UIColor.Yellow);
                this.speedUnitsLabel.SetTextColor(UIColor.Yellow);

                darkMode = true;
            }
        }

        partial void darkModeTogglePressed()
        {
            toggleBatteryPressed();
        }

    }
}

