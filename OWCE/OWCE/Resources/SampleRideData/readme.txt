For development purposes.

Adding files into this folder is intended to replay previous rides through OWCE app in iOS simulator and Android emulator.

1. Add a recored .bin file into SampleRideData directory.
2. Change build action to embedded resource.
3. Launch in debug mode for iOS simulator or Android emulator.
4. Ride should show up as board with the board name being the filename of the ride (without the extension)
5. Clicking the recorded ride should replay all the data through OWCE in order to work on the UI without having to ride with a phone in one hand and laptop in the other.

.gitignore will try its best to not allow you to commit your ride data accidentally. Please make sure you don't commit your ride data.