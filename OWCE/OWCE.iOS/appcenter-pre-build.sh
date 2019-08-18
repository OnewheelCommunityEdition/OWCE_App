#!/usr/bin/env bash

echo "IOS"

# ../appcenter-pre-build-shared.sh

if [ ! -n "$OWCE_SYNCFUSION_LICENSE" ]
then
    echo "You need define the OWCE_SYNCFUSION_LICENSE variable in App Center"
    exit
fi

if [ ! -n "$OWCE_APPCENTER_IOS" ]
then
    echo "You need define the OWCE_APPCENTER_IOS variable in App Center"
    exit
fi

if [ ! -n "$OWCE_APPCENTER_ANDROID" ]
then
    echo "You need define the OWCE_APPCENTER_ANDROID variable in App Center"
    exit
fi

# For local dev.
if [ ! -n "$APPCENTER_SOURCE_DIRECTORY" ]
then
    APPCENTER_SOURCE_DIRECTORY=$(pwd)
fi

APP_CONSTANT_FILE=$APPCENTER_SOURCE_DIRECTORY/OWCE/AppConstants.cs

if [ -e "$APP_CONSTANT_FILE" ]
then
    echo "Updating SyncfusionLicense to $OWCE_SYNCFUSION_LICENSE in AppConstant.cs"
    sed -i '' 's#SyncfusionLicense = "[a-z:./]*"#SyncfusionLicense = "'$OWCE_SYNCFUSION_LICENSE'"#' $APP_CONSTANT_FILE

    echo "Updating SyncfusionLicense to $OWCE_APPCENTER_IOS in AppConstant.cs"
    sed -i '' 's#AppCenteriOS = "[a-z:./]*"#AppCenteriOS = "'$OWCE_APPCENTER_IOS'"#' $APP_CONSTANT_FILE

    echo "Updating SyncfusionLicense to $OWCE_APPCENTER_ANDROID in AppConstant.cs"
    sed -i '' 's#AppCenterAndroid = "[a-z:./]*"#AppCenterAndroid = "'$OWCE_APPCENTER_ANDROID'"#' $APP_CONSTANT_FILE
fi


# Add entitlement for TestFlight distribution.
if [ "$APPCENTER_BRANCH" == "master" ];
then
    plutil -insert beta-reports-active -bool YES $APPCENTER_SOURCE_DIRECTORY/OWCE/OWCE.iOS/Info.plist
fi
