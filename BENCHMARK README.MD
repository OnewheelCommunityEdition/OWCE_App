Onewheel Community Edition (OWCE) Compile Benchmark
===========

This repo bulds Onewheel Community Edition app (OWCE), a community built companion app for Onewheel (single wheel electric skateboard). The project itself is using cross platform Xamarin.Forms (C#/Mono) app.

Xamarin.Forms apps consist of the main shared codebase inside the Forms project (in this case OWCE.csproj), a Xamarin.Android project (OWCE.Android.csproj), and a Xamarin.iOS project (OWCE.iOS.csproj).

To start the benchmark with `./run_benchmark.sh`

The script will first delete bin/obj directories for projects about to be built, as well as restore nuget packages for those projects. 

It will then do an initial clean build of the Android project (this builds the Forms project as well) build, followed by modifying the Android project, and then finally modifying the Forms project. 

This simulates a clean build, working in Android specific code, working in the Forms project. Becuase Xamarin.Forms is a cross platform framework developers spend most of their time working in the Forms portain of their project.

This process is then repeated for iOS.

### Sample output:
```
Removing bin/obj folders and restoring nugets for OWCE.csproj
Removing bin/obj folders and restoring nugets for OWCE.Android.csproj
Building OWCE.Android
Modifying OWCE.Android and building again.
Modifying forms and building again.
Removing bin/obj folders and restoring nugets for OWCE.csproj
Removing bin/obj folders and restoring nugets for OWCE.iOS.csproj
Removing bin/obj folders and restoring nugets for OWCE.WatchOS.WatchOSApp.csproj
Removing bin/obj folders and restoring nugets for OWCE.WatchOS.WatchOSExtension.csproj
Building OWCE.iOS
Modifying OWCE.iOS and building again.
Modifying forms and building again.


Android build time: 34 seconds
Android re-build time: 9 seconds
Android forms re-build time: 11 seconds

iOS build time: 17 seconds
iOS re-build time: 4 seconds
iOS forms re-build time: 6 seconds

Total build time: 81 seconds

```

### Notes:
- This is not a build comparison of Xamarin.iOS vs Xamarin.Android.
- It is possible that the build configs are not using optimised for build time (although this should be irrelvant for comparing different machines).
- Apps are built in Debug mode. You can change to release mode by setting `BUILD_CONFIG=Release`
- Apps are built to be deployed to Android emulator/iOS simulator. Not for physical device builds. If you want to deploy to physical devcies you would need to create provisioning profiles to get the iOS app to build.
- This benchmark does not record time required to deploy to Android emulator/iOS simulator.

### Results
| Device | Android build | Android rebuild | Android Forms rebuild | iOS build | iOS rebuild | iOS Forms rebuild | Total |
|-------------|-------------|-------------|-------------|-------------|-------------|-------------|-------------|
| Mid 2014 MacBook Pro (i7, 2.8 GHz) | 34 | 9 | 11 | 17 | 4 |  6 | 81 |
