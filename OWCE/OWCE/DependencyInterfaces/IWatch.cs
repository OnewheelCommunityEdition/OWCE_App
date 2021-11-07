using System;
using System.Collections.Generic;

namespace OWCE.DependencyInterfaces
{
    // Handles communication with the watch.
    // Platform-specific implementations are expected to implement
    // this interface via DependencyService
    public interface IWatch
    {
        void SendWatchMessages(Dictionary<WatchMessage, object> messages);
        void ListenForWatchMessages(OWBoard board);
        void StopListeningForWatchMessages();
    }
}
