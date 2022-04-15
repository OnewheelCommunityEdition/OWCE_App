Onewheel Community Edition (OWCE) App
===========

A cross-platform app for use with the [Onewheel](https://onewheel.com/) V1, Plus, XR and Pint boards from Future Motion.

Newer board firmware no longer send through voltage as this was removed by Future Motion. Those versions are:
- XR with firmware 4155 and higher
- Pint with firmware 5059 and higher

NOTE: Onewheel Community Edition app is not endorsed by or affiliated with Future Motion in any way.

Written in C# with [Xamarin](http://www.xamarin.com)

Open Source Project by [@beeradmoore](http://www.twitter.com/beeradmoore) 

## Available (soon) for free on:
* Android and WearOS: Available on Google Play
* iPhone and Apple Watch: Available on App Store

## How to build and run this project. 

Before you start you will need to have installed both Visual Studio and Xamarin. If you are using Windows you will want Visual Studio 2017 ([install guide here](https://docs.microsoft.com/en-us/xamarin/cross-platform/get-started/installation/windows)) or if you are on macOS you will want Visual Studio for Mac ([install guide here](https://docs.microsoft.com/en-us/visualstudio/mac/installation)).

Using your flavour of Visual Studio open OWCE.sln. From the platform dropdown choose OWCE.iOS or OWCE.Android depending what platform you wish to build for. Then deploy and debug your app as you would with any other project.

NOTE: Because the app is very dependent on the Onewheels low energy bluetooth it will not function correctly in a simulator/emulator. For best results deploy to a physical device. 



## Frequently (or not so frequently) asked Questions

### Why did you create this?

There is quite a number of members on the [Onewheel Owners facebook group](https://www.facebook.com/groups/onewheelownersgroup/) that for one reason or another don't like the stock app by Future Motion. I figured why not create an app with its development shaped by features that the community wants.

### Don't other third party apps already exist?

Yes. Such as [pOnewheel](https://play.google.com/store/apps/details?id=net.kwatts.powtools&hl=en) for Android and [Float Deck](https://itunes.apple.com/us/app/float-deck/id1332503706?mt=8) for iOS. But the problem is one is for Android and the other is for iOS. Wouldn't it be better if there was just 1 app with the exact same feature sets shared across both platforms?

### Does this change how my Onewheel performs?

No. This app uses the same bluetooth low energy (BLE) interface that the official Onewheel app uses to read and display various stats.

### What Onewheels are supported?

Currently v1, Plus, XR and Pint. Pint X has not been tested yet.

### Will using this app void my warranty?

Although things such as riding your board without a helmet can void your warranty, we don't believe that using third party apps will void your warranty.
