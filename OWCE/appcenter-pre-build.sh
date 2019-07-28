#!/usr/bin/env bash
#
# For Xamarin, change some constants located in some class of the app.
# In this sample, suppose we have an AppConstant.cs class in shared folder with follow content:
#
# namespace Core
# {
#     public class AppConstant
#     {
#         public const string ApiUrl = "https://production.com/api";
#     }
# }
# 
# Suppose in our project exists two branches: master and develop. 
# We can release app for production API in master branch and app for test API in develop branch. 
# We just need configure this behaviour with environment variable in each branch :)
# 
# The same thing can be perform with any class of the app.
#
# AN IMPORTANT THING: FOR THIS SAMPLE YOU NEED DECLARE API_URL ENVIRONMENT VARIABLE IN APP CENTER BUILD CONFIGURATION.

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
    plutil -insert beta-reports-active -bool YES $APPCENTER_SOURCE_DIRECTORY/OWCE.iOS/Info.plist
fi