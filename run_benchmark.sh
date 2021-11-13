#!/bin/bash
LOG_FILENAME="benchmark - $(date +"%Y-%m-%d %I-%M%p").log"
LOG_FILE="$(pwd)/$LOG_FILENAME"

BUILD_CONFIG=Debug
ANDROID_BUILD_ARGS="/p:Configuration=$BUILD_CONFIG"
IOS_BUILD_ARGS="/p:Configuration=$BUILD_CONFIG /p:Platform=iPhoneSimulator"

pushd OWCE/ > /dev/null

    pushd OWCE/ > /dev/null
        echo "Removing bin/obj folders and restoring nugets for OWCE.csproj"
        echo "Removing bin/obj folders and restoring nugets for OWCE.csproj" >> "$LOG_FILE"
        rm -rf bin/
        rm -rf obj/
        msbuild OWCE.csproj /p:Configuration=$BUILD_CONFIG /t:restore >> "$LOG_FILE"
        if [ $? -ne 0 ]; then
            echo "Unable to complete action. Please check your log file ($LOG_FILENAME) for more information."
            exit
        fi
        echo "" >> "$LOG_FILE"
    popd > /dev/null

    pushd OWCE.Android/ > /dev/null
        echo "Removing bin/obj folders and restoring nugets for OWCE.Android.csproj"
        echo "Removing bin/obj folders and restoring nugets for OWCE.Android.csproj" >> "$LOG_FILE"
        rm -rf bin/
        rm -rf obj/
        msbuild OWCE.Android.csproj /p:Configuration=$BUILD_CONFIG /t:restore >> "$LOG_FILE"
        if [ $? -ne 0 ]; then
            echo "Unable to complete action. Please check your log file ($LOG_FILENAME) for more information."
            exit
        fi
        echo "" >> "$LOG_FILE"
    popd > /dev/null

    pushd OWCE.iOS/ > /dev/null
        echo "Removing bin/obj folders and restoring nugets for OWCE.iOS.csproj"
        echo "Removing bin/obj folders and restoring nugets for OWCE.iOS.csproj" >> "$LOG_FILE"
        rm -rf bin/
        rm -rf obj/
        msbuild OWCE.iOS.csproj /p:Configuration=$BUILD_CONFIG /t:restore >> "$LOG_FILE"
        if [ $? -ne 0 ]; then
            echo "Unable to complete action. Please check your log file ($LOG_FILENAME) for more information."
            exit
        fi
        echo "" >> "$LOG_FILE"
    popd > /dev/null

    pushd OWCE.WatchOS/OWCE.WatchOS.WatchOSApp/ > /dev/null
        echo "Removing bin/obj folders and restoring nugets for OWCE.WatchOS.WatchOSApp.csproj"
        echo "Removing bin/obj folders and restoring nugets for OWCE.WatchOS.WatchOSApp.csproj" >> "$LOG_FILE"
        rm -rf bin/
        rm -rf obj/
        msbuild OWCE.WatchOS.WatchOSApp.csproj /p:Configuration=$BUILD_CONFIG /t:restore >> "$LOG_FILE"
        if [ $? -ne 0 ]; then
            echo "Unable to complete action. Please check your log file ($LOG_FILENAME) for more information."
            exit
        fi
        echo "" >> "$LOG_FILE"
    popd > /dev/null

    pushd OWCE.WatchOS/OWCE.WatchOS.WatchOSExtension/ > /dev/null
        echo "Removing bin/obj folders and restoring nugets for OWCE.WatchOS.WatchOSExtension.csproj"
        echo "Removing bin/obj folders and restoring nugets for OWCE.WatchOS.WatchOSExtension.csproj" >> "$LOG_FILE"
        rm -rf bin/
        rm -rf obj/
        msbuild OWCE.WatchOS.WatchOSExtension.csproj /p:Configuration=$BUILD_CONFIG /t:restore >> "$LOG_FILE"
        if [ $? -ne 0 ]; then
            echo "Unable to complete action. Please check your log file ($LOG_FILENAME) for more information."
            exit
        fi
        echo "" >> "$LOG_FILE"
    popd > /dev/null


    pushd OWCE.Android/ > /dev/null
        echo "Building OWCE.Android";

        BUILD_START_TIME=$(date +%s)
        msbuild OWCE.Android.csproj $ANDROID_BUILD_ARGS >> "$LOG_FILE"
        if [ $? -ne 0 ]; then
            echo "Unable to complete action. Please check your log file ($LOG_FILENAME) for more information."
            exit
        fi
        BUILD_END_TIME=$(date +%s)
        ANDROID_BUILD_TIME=$(($BUILD_END_TIME - $BUILD_START_TIME))
        
        echo "Modifying OWCE.Android and building again."
        echo "//modification" >> MainActivity.cs
        BUILD_START_TIME=$(date +%s)
        msbuild OWCE.Android.csproj $ANDROID_BUILD_ARGS >> "$LOG_FILE"
        if [ $? -ne 0 ]; then
            echo "Unable to complete action. Please check your log file ($LOG_FILENAME) for more information."
            exit
        fi
        BUILD_END_TIME=$(date +%s)
        ANDROID_REBUILD_TIME=$(($BUILD_END_TIME - $BUILD_START_TIME))

        echo "Modifying forms and building again."
        echo "//modification" >> ../OWCE/App.xaml.cs
        BUILD_START_TIME=$(date +%s)
        msbuild OWCE.Android.csproj $ANDROID_BUILD_ARGS >> "$LOG_FILE"
        if [ $? -ne 0 ]; then
            echo "Unable to complete action. Please check your log file ($LOG_FILENAME) for more information."
            exit
        fi
        BUILD_END_TIME=$(date +%s)
        ANDROID_FORMS_REBUILD_TIME=$(($BUILD_END_TIME - $BUILD_START_TIME))
    popd > /dev/null

    pushd OWCE.iOS/ > /dev/null
        echo "Building OWCE.iOS";

        BUILD_START_TIME=$(date +%s)
        msbuild OWCE.iOS.csproj $IOS_BUILD_ARGS >> "$LOG_FILE"
        if [ $? -ne 0 ]; then
            echo "Unable to complete action. Please check your log file ($LOG_FILENAME) for more information."
            exit
        fi
        BUILD_END_TIME=$(date +%s)
        IOS_BUILD_TIME=$(($BUILD_END_TIME - $BUILD_START_TIME))
        
        echo "Modifying OWCE.iOS and building again."
        echo "//modification" >> AppDelegate.cs
        BUILD_START_TIME=$(date +%s)
        msbuild OWCE.iOS.csproj $IOS_BUILD_ARGS >> "$LOG_FILE"
        if [ $? -ne 0 ]; then
            echo "Unable to complete action. Please check your log file ($LOG_FILENAME) for more information."
            exit
        fi
        BUILD_END_TIME=$(date +%s)
        IOS_REBUILD_TIME=$(($BUILD_END_TIME - $BUILD_START_TIME))

        echo "Modifying forms and building again."
        echo "//modification" >> ../OWCE/App.xaml.cs
        BUILD_START_TIME=$(date +%s)
        msbuild OWCE.iOS.csproj $IOS_BUILD_ARGS >> "$LOG_FILE"
        if [ $? -ne 0 ]; then
            echo "Unable to complete action. Please check your log file ($LOG_FILENAME) for more information."
            exit
        fi
        BUILD_END_TIME=$(date +%s)
        IOS_FORMS_REBUILD_TIME=$(($BUILD_END_TIME - $BUILD_START_TIME))
    popd > /dev/null

  
popd > /dev/null


TOTAL_BUILD_TIME=$(($ANDROID_BUILD_TIME+$ANDROID_REBUILD_TIME+$ANDROID_FORMS_REBUILD_TIME+$IOS_BUILD_TIME+$IOS_REBUILD_TIME+$IOS_FORMS_REBUILD_TIME))

echo "" >> "$LOG_FILE"
echo "" >> "$LOG_FILE"
echo "Android build time: $ANDROID_BUILD_TIME seconds" >> "$LOG_FILE"
echo "Android re-build time: $ANDROID_REBUILD_TIME seconds" >> "$LOG_FILE"
echo "Android forms re-build time: $ANDROID_FORMS_REBUILD_TIME seconds" >> "$LOG_FILE"
echo "" >> "$LOG_FILE"
echo "iOS build time: $IOS_BUILD_TIME seconds" >> "$LOG_FILE"
echo "iOS re-build time: $IOS_REBUILD_TIME seconds" >> "$LOG_FILE"
echo "iOS forms re-build time: $IOS_FORMS_REBUILD_TIME seconds" >> "$LOG_FILE"
echo "" >> "$LOG_FILE"
echo "Total build time: $TOTAL_BUILD_TIME seconds" >> "$LOG_FILE"
echo "" >> "$LOG_FILE"

echo ""
echo ""
echo "Android build time: $ANDROID_BUILD_TIME seconds"
echo "Android re-build time: $ANDROID_REBUILD_TIME seconds"
echo "Android forms re-build time: $ANDROID_FORMS_REBUILD_TIME seconds"
echo ""
echo "iOS build time: $IOS_BUILD_TIME seconds"
echo "iOS re-build time: $IOS_REBUILD_TIME seconds"
echo "iOS forms re-build time: $IOS_FORMS_REBUILD_TIME seconds"
echo ""
echo "Total build time: $TOTAL_BUILD_TIME seconds"
echo ""