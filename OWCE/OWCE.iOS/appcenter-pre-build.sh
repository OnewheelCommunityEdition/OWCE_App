#!/usr/bin/env bash

../appcenter-pre-build-shared.sh

echo "BRANCH"
echo $APPCENTER_BRANCH

# Add entitlement for TestFlight distribution.
if [ "$APPCENTER_BRANCH" == "master" ];
then
    #plutil -insert beta-reports-active -bool YES $APPCENTER_SOURCE_DIRECTORY/OWCE/OWCE.iOS/Info.plist
fi