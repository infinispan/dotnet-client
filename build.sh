# cd buildscripts
# create tmp
# obtain nuget
# install fake
# pass params to fake script
# return exit code fake

cd buildtools

if [ ! -d tmp ]; then
  mkdir tmp
fi

NUGET_SOURCE="https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
NUGET_PATH="tmp/nuget/nuget.exe"
if [ ! -f $NUGET_PATH ]; then
  mkdir tmp/nuget
  wget -O tmp/nuget/nuget.exe $NUGET_SOURCE
fi

FAKE_PATH="tmp/FAKE/tools/FAKE.exe"
if [ ! -f $FAKE_PATH ]; then
  mono $NUGET_PATH "install" "FAKE" "-ExcludeVersion" "-Version" "4.63.2" "-OutputDirectory" "tmp"
fi

TARGET="target=build"
if [ "$#" -gt 0 ]; then
  TARGET="target=$1"
fi

mono $FAKE_PATH "build.fsx" $TARGET

cd ..