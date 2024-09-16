Onewheel Community Edition (OWCE) App
===========

A cross-platform app for use with the [Onewheel](https://onewheel.com/) V1, Plus, XR, Pint, Pint X and GT.

NOTE: GT support requires patching with [Rewheel](https://github.com/rewheel-app/rewheel).
The newer board firmware no longer sends through voltage, as Future Motion removed it. Those versions are:
- XR with firmware 4155 and higher
- Pint with firmware 5059 and higher
_ Pint X, all firmware
- GT, all firmware

NOTE: Onewheel Community Edition app is not endorsed by or affiliated with Future Motion in any way.

Written in C# with [Xamarin](http://www.xamarin.com)

Open Source Project by [@beeradmoore](http://www.twitter.com/beeradmoore) 

## Available (soon) for free on:
* Android and WearOS: Available on Google Play
* iPhone and Apple Watch: Available on App Store

## How to build and run this project. 

Before you start, you will need to install both Visual Studio and Xamarin. If you are using Windows you will want Visual Studio 2017 ([install guide here](https://docs.microsoft.com/en-us/xamarin/cross-platform/get-started/installation/windows)) or if you are on macOS you will want Visual Studio for Mac ([install guide here](https://docs.microsoft.com/en-us/visualstudio/mac/installation)).

Using your flavor of Visual Studio, open OWCE.sln. From the platform dropdown, choose OWCE.iOS or OWCE.Android, depending on what platform you wish to build for. Then, deploy and debug your app like any other project.

NOTE: Because the app depends on the Onewheels low-energy Bluetooth, it will not function correctly in a simulator/emulator. For best results, deploy to a physical device. 



## Frequently (or not so frequently) Asked Questions

### Why did you create this?

There are quite a number of members on the [Onewheel Owners Facebook group](https://www.facebook.com/groups/onewheelownersgroup/) that, for one reason or another, don't like the stock app by Future Motion. I figured, why not create an app with its development shaped by features that the community wants?

### Do other third-party apps already exist?

Yes, but Future Motion's firmware lockdowns prohibit them from being used. Such as pOnewheel[https://github.com/ponewheel/android-ponewheel] (deprecated) for Android and Float Deck (obsolete) for iOS. However, the problem is that one is for Android, and the other is for iOS. Wouldn't it be better if there was just 1 app with the exact same feature sets shared across both platforms?

### Does this change how my Onewheel performs?

No. This app uses the same Bluetooth low energy (BLE) interface that the official Onewheel app uses to read and display various stats.

### What Onewheels are supported?

Currently, v1, Plus, XR, and Pint. Pint X and GT have not yet been thoroughly tested.

### Will using this app void my warranty?

Although things such as riding your board without a helmet can void your warranty, we don't believe that using third-party apps will void your warranty.
