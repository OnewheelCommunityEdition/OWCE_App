#!/usr/bin/env bash
# App Center custom build scripts: https://aka.ms/docs/build/custom/scripts

echo "Deleting non-Android related csproj files to avoid updating nugets."
rm -f $BUILD_REPOSITORY_LOCALPATH/OWCE/OWCE.iOS/OWCE.iOS.csproj
rm -f $BUILD_REPOSITORY_LOCALPATH/OWCE/OWCE.MacOS/OWCE.MacOS.csproj
rm -f $BUILD_REPOSITORY_LOCALPATH/OWCE/OWCE.WatchOS/OWCE.WatchOS.WatchOSApp/OWCE.WatchOS.WatchOSApp.csproj
rm -f $BUILD_REPOSITORY_LOCALPATH/OWCE/OWCE.WatchOS/OWCE.WatchOS.WatchOSExtension/OWCE.WatchOS.WatchOSExtension.csproj