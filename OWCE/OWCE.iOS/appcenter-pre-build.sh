#!/usr/bin/env bash

echo "IOS"

# ../appcenter-pre-build-shared.sh

if [ -z "$OWCE_SYNCFUSION_LICENSE" ]
then
    echo "You need define the OWCE_SYNCFUSION_LICENSE variable in App Center"
    exit
fi

if [ -z "$OWCE_APPCENTER_IOS" ]
then
    echo "You need define the OWCE_APPCENTER_IOS variable in App Center"
    exit
fi

if [ -z "$OWCE_APPCENTER_ANDROID" ]
then
    echo "You need define the OWCE_APPCENTER_ANDROID variable in App Center"
    exit
fi

echo "All keys are defined.";
# For local dev.
echo "APPCENTER_SOURCE_DIRECTORY 1"
echo $APPCENTER_SOURCE_DIRECTORY
if [ -z "$APPCENTER_SOURCE_DIRECTORY" ]
then
    echo "reseting appcenter source directory"
    APPCENTER_SOURCE_DIRECTORY=$(pwd)
fi
echo "APPCENTER_SOURCE_DIRECTORY 2"
echo $APPCENTER_SOURCE_DIRECTORY

APP_CONSTANT_FILE=$APPCENTER_SOURCE_DIRECTORY/OWCE/AppConstants.cs

echo $APP_CONSTANT_FILE
if [ -e "$APP_CONSTANT_FILE" ]
then
    echo "Should try update file."
    echo "Updating SyncfusionLicense to $OWCE_SYNCFUSION_LICENSE in AppConstant.cs"
    sed -i '' 's#SyncfusionLicense = "[a-z:./]*"#SyncfusionLicense = "'$OWCE_SYNCFUSION_LICENSE'"#' $APP_CONSTANT_FILE

    echo "Updating SyncfusionLicense to $OWCE_APPCENTER_IOS in AppConstant.cs"
    sed -i '' 's#AppCenteriOS = "[a-z:./]*"#AppCenteriOS = "'$OWCE_APPCENTER_IOS'"#' $APP_CONSTANT_FILE

    echo "Updating SyncfusionLicense to $OWCE_APPCENTER_ANDROID in AppConstant.cs"
    sed -i '' 's#AppCenterAndroid = "[a-z:./]*"#AppCenterAndroid = "'$OWCE_APPCENTER_ANDROID'"#' $APP_CONSTANT_FILE
fi
echo "Finished updating file"

# Add entitlement for TestFlight distribution.
if [ "$APPCENTER_BRANCH" == "master" ];
then
    echo "Enabling TestFlight functionality."
    plutil -insert beta-reports-active -bool YES $APPCENTER_SOURCE_DIRECTORY/OWCE/OWCE.iOS/Info.plist
fi
