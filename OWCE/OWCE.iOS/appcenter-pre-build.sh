#!/usr/bin/env bash

../appcenter-pre-build-shared.sh

# Add entitlement for TestFlight distribution.
if [ "$APPCENTER_BRANCH" == "master" ];
then
    echo "Enabling TestFlight functionality."
    plutil -insert beta-reports-active -bool YES $APPCENTER_SOURCE_DIRECTORY/OWCE/OWCE.iOS/Info.plist
fi
