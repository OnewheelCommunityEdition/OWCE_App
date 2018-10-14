#!/bin/sh
rm -rf csharp
mkdir csharp
protoc -I=. --csharp_out=csharp/ *.proto
