using System.Collections.Generic;
using OWCE.DependencyInterfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(OWCE.Droid.DependencyImplementations.Watch))]

namespace OWCE.Droid.DependencyImplementations
{
    public class Watch : IWatch
    {
        public void SendWatchMessages(Dictionary<WatchMessage, object> messages)
        {
            // Implement when we have Android Watch support
        }

        void IWatch.ListenForWatchMessages(OWBoard board)
        {
            // Implement when we have Android Watch support
        }

        void IWatch.StopListeningForWatchMessages()
        {
            // Implement when we have Android Watch support
        }
    }
}
