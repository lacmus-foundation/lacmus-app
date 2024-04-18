#!/bin/bash
rm -rf ./bin/app/ && \
    rm -rf ./src/LacmusApp/bin/Release/ && \
    rm -rf ./src/LacmusApp.Avalonia/bin/Release/
echo "restoring packeges"
dotnet restore src/LacmusApp.sln
echo -n "building for linux"
dotnet publish --framework net8.0 --runtime="linux-x64" -c Release -o ./bin/app/linux src/LacmusApp.sln
mv ./bin/app/linux/LacmusApp.Avalonia ./bin/app/linux/LacmusApp
echo -n "building for win10"
dotnet publish --framework net8.0 --runtime="win10-x64" -c Release -o ./bin/app/win10 src/LacmusApp.sln
mv ./bin/app/win10/LacmusApp.Avalonia.exe ./bin/app/win10/LacmusApp.exe
echo -n "building for osx"
dotnet publish --framework net8.0 --runtime="osx-x64" -c Release -o ./bin/app/osx src/LacmusApp.sln
mv ./bin/app/osx/LacmusApp.Avalonia ./bin/app/osx/LacmusApp
cd ./bin/app/
zip -r -9 ./linux.zip ./linux/
zip -r -9 ./win10.zip ./win10/
zip -r -9 ./osx.zip ./osx/
