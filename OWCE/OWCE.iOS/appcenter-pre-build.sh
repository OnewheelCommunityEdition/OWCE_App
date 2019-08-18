#!/bin/bash

../appcenter-pre-build-shared.sh

# Add entitlement for TestFlight distribution.
if [ "$APPCENTER_BRANCH" == "master" ];
then
    plutil -insert beta-reports-active -bool YES $APPCENTER_SOURCE_DIRECTORY/OWCE/OWCE.iOS/Info.plist
fi
