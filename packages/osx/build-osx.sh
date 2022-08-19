APP_NAME="LacmusApp.Avalonia"
RID_LIST=("osx-x64")
PUB_PLATFORM_LIST=("osx-x64")
CONFIG="Release"
CERT_NAME="" # Name of certification to sign the application

echo "********** Start building $APP_NAME **********"

# Get application version
VERSION="0.7.0"
if [ "$?" != "0" ]; then
    echo "Unable to get version of $APP_NAME"
    exit
fi
echo "Version: $VERSION"

# Create output directory
mkdir -p bin/osx
cd src

# Build packages
for i in "${!RID_LIST[@]}"; do
    RID=${RID_LIST[$i]}
    PUB_PLATFORM=${PUB_PLATFORM_LIST[$i]}

    echo " " 
    echo "[$PUB_PLATFORM ($RID)]"
    echo " "

    # clean
    rm -rf ./LacmusApp.Avalonia/bin/
    dotnet clean
    dotnet restore
    if [ "$?" != "0" ]; then
        exit
    fi
    
    # build
    dotnet msbuild LacmusApp.Avalonia -t:BundleApp -property:Configuration=$CONFIG -p:SelfContained=true -p:PublishSingleFile=false -p:PublishTrimmed=true -p:RuntimeIdentifier=$RID
    if [ "$?" != "0" ]; then
        exit
    fi

    # create output directory
    if [[ -d "../bin/osx/$PUB_PLATFORM" ]]; then
        rm -r "../bin/osx/$PUB_PLATFORM"
    fi
    echo "Create directory '../bin/osx/$PUB_PLATFORM'"
    mkdir "../bin/osx/$PUB_PLATFORM"
    if [ "$?" != "0" ]; then
        exit
    fi

    # copy .app directory to output directoty
    echo "Creating app bundle..."
    mv ./LacmusApp.Avalonia/bin/$CONFIG/net6.0/$RID/publish/Lacmus.app ../bin/osx/$PUB_PLATFORM/Lacmus.app
    if [ "$?" != "0" ]; then
        exit
    fi

    # copy application icon and remove unnecessary files
    cp ../packages/osx/LacmusApp.icns ../bin/osx/$PUB_PLATFORM/Lacmus.app/Contents/Resources/LacmusApp.icns
    if [ "$?" != "0" ]; then
        exit
    fi

    # sign application
    #find "../bin/osx/$PUB_PLATFORM/Lacmus.app/Contents/MacOS/" | while read FILE_NAME; do
    #    if [[ -f $FILE_NAME ]]; then
    #        if [[ "$FILE_NAME" != "../bin/osx/$PUB_PLATFORM/Lacmus.app/Contents/MacOS//$APP_NAME" ]]; then
    #            echo "Signing $FILE_NAME"
    #            codesign -f -o runtime --timestamp --entitlements "../packages/osx/LacmusApp.entitlements" -s "$CERT_NAME" "$FILE_NAME"
    #            if [ "$?" != "0" ]; then
    #                exit
    #            fi
    #        fi
    #    fi
    #done
    #codesign -f -o runtime --timestamp --entitlements "../packages/osx/LacmusApp.entitlements" -s "$CERT_NAME" "../bin/osx/$PUB_PLATFORM/Lacmus.app/Contents/MacOS/$APP_NAME"
    #codesign -f -o runtime --timestamp --entitlements "../packages/osx/LacmusApp.entitlements" -s "$CERT_NAME" "../bin/osx/$PUB_PLATFORM/Lacmus.app"
    #
    # zip .app directory
    ditto -c -k --sequesterRsrc --keepParent "../bin/osx/$PUB_PLATFORM/Lacmus.app" "../bin/osx/LacmusApp.Avalonia-$VERSION-$PUB_PLATFORM.zip"
    rm -rf "../bin/osx/$PUB_PLATFORM/Lacmus.app"

done
