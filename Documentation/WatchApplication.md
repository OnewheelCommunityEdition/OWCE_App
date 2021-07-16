# Watch Support for OWCE

This page discusses Watch support for OWCE. Currently, only WatchOS (Apple Watch) is supported.

## How the code works

The Watch App uses Dependency Interfaces to communicate between the core (platform independent) OWCE app and the native iOS app, and WatchConnectivity to communicate between iOS and WatchOS.

### When board is connected

In [BoardListPage.xaml.cs](../OWCE/OWCE/Pages/BoardListPage.xaml.cs), when the board is connected, `IWatch.ListenForWatchMessages` is called:
* iOS app registers as a WatchConnectivity listener to receive messages from the watch (eg when the watch wakes up) and sends the latest values to the watch
* In future, can also be used for the watch to send actions back to the phone, such as changing ride mode or start ride tracking

### When watch app wakes up

In [InterfaceController.cs](../OWCE/OWCE.WatchOS/OWCE.WatchOS.WatchOSExtension/InterfaceController.cs):
* WatchOS Extension registers itself as a WatchConnectivity listener for receiving messages
* Upon receiving a message from the phone, the app will update the necessary parts of the UI
* When the watch is about to be activated (ie about to appear to the wearer), the watch app sends a message to the WatchConnectivity session for the phone to respond with the latest up-to-date values

### How the phone updates the watch with new values

In [OWBoard.cs](../OWCE/OWCE/OWBoard.cs) under `OWBoard()`, `WatchSyncEventHandler` is registered as a `PropertyChanged` listener to keep the watch app in sync with the latest values (eg speed, pattery percent, distance).

[WatchSyncEventHandler](../OWCE/OWCE/PropertyChangeHandlers/WatchSyncEventHandler.cs) filters the list of property changes and sends the relevant updates to the [IWatch](../OWCE/OWCE/DependencyInterfaces/IWatch.cs) Dependency Interface, implemented by [Watch.cs](../OWCE/OWCE.iOS/DependencyImplementations/Watch.cs) on iOS.

[Watch.cs](../OWCE/OWCE.iOS/DependencyImplementations/Watch.cs) uses the WatchConnectivity session to send update messages to the watch.
