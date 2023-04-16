#!/usr/bin/env bash

export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true

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


dotnet tool install --global boots
boots --stable Mono

if [[ "$APPCENTER_XAMARIN_PROJECT" == *"OWCE.iOS.csproj"* ]]; then
  echo "iOS build detected"
  boots --stable Xamarin.iOS
fi

if [[ "$APPCENTER_XAMARIN_PROJECT" == *"OWCE.Android.csproj"* ]]; then
  echo "Android build detected"
  boots --stable Xamarin.Android
fi

