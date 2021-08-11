using System;

using Foundation;
using WatchConnectivity;
using WatchKit;

namespace OWCE.WatchOS.WatchOSExtension
{
    [Register("ExtensionDelegate")]
    public class ExtensionDelegate : WKExtensionDelegate
    {
        public ExtensionDelegate()
        {
        }
        public override void ApplicationDidFinishLaunching()
        {
            // Perform any final initialization of your application.
            WCSessionManager.SharedManager.StartSession();
        }
    }
}

