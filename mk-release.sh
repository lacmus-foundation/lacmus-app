#!/bin/bash
rm -rf ./bin/app/ && rm -rf ./src/bin/Release/
echo "restoring packeges"
dotnet restore
echo -n "building for linux"
dotnet publish --framework netcoreapp3.1 --runtime="linux-x64" -c Release -o ./bin/app/linux
echo -n "building for win10"
dotnet publish --framework netcoreapp3.1 --runtime="win10-x64" -c Release -o ./bin/app/win10
echo -n "building for osx"
dotnet publish --framework netcoreapp3.1 --runtime="osx-x64" -c Release -o ./bin/app/osx
cd ./bin/app/
zip -r -9 ./linux.zip ./linux/
zip -r -9 ./win10.zip ./win10/
zip -r -9 ./osx.zip ./osx/
