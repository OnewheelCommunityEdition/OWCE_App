#!/usr/bin/env bash
# App Center custom build scripts: https://aka.ms/docs/build/custom/scripts

echo "Deleting Android csproj file to avoid updating nugets."
rm -f $BUILD_REPOSITORY_LOCALPATH/OWCE/OWCE.Android/OWCE.Android.csproj
