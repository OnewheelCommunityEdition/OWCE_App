#!/usr/bin/env bash

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

# For local dev.
if [ -z "$APPCENTER_SOURCE_DIRECTORY" ]
then
    APPCENTER_SOURCE_DIRECTORY=$(pwd)
fi

APP_CONSTANT_FILE=$APPCENTER_SOURCE_DIRECTORY/OWCE/OWCE/AppConstants.cs

if [ -e "$APP_CONSTANT_FILE" ]
then
    echo "Updating SyncfusionLicense to $OWCE_APPCENTER_IOS in AppConstant.cs"
    sed -i '' 's#AppCenteriOS = "[a-z:./]*"#AppCenteriOS = "'$OWCE_APPCENTER_IOS'"#' $APP_CONSTANT_FILE

    echo "Updating SyncfusionLicense to $OWCE_APPCENTER_ANDROID in AppConstant.cs"
    sed -i '' 's#AppCenterAndroid = "[a-z:./]*"#AppCenterAndroid = "'$OWCE_APPCENTER_ANDROID'"#' $APP_CONSTANT_FILE
fi
