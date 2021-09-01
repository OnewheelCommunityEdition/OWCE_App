using System.Collections.Generic;
using OWCE.DependencyInterfaces;
using OWCE.PropertyChangeHandlers;
using WatchConnectivity;
using Xamarin.Forms;

[assembly: Dependency(typeof(OWCE.iOS.DependencyImplementations.Watch))]

namespace OWCE.iOS.DependencyImplementations
{
    // Implementation of IWatch on iOS.
    // Messages are received on InterfaceController.cs on the watch side.
    public class Watch : IWatch
    {
        private OWBoard _board;

        public void SendWatchMessages(Dictionary<string, object> messages)
        {
            WCSessionManager.SharedManager.SendMessage(messages);
        }

        void IWatch.ListenForWatchMessages(OWBoard board)
        {
            _board = board;
            WCSessionManager.SharedManager.MessageReceived += DidReceiveMessage;
        }

        void IWatch.StopListeningForWatchMessages()
        {
            WCSessionManager.SharedManager.MessageReceived -= DidReceiveMessage;
        }

        public void DidReceiveMessage(WCSession session, Dictionary<string, object> message)
        {
            WatchSyncEventHandler.HandleWatchMessage(message, _board);
        }
    }
}
