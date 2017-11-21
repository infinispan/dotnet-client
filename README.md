# C# Hot Rod Client

[![Build status](https://ci.appveyor.com/api/projects/status/u4epfsjoso2a45lt?svg=true)](https://ci.appveyor.com/project/jfojtl/dotnet-client)

Hot Rod client enables you to connect to [infinispan](http://infinispan.org).

## Usage

To use this client, simply add nuget package `Infinispan.HotRod`. Sample of such usage is part of this repo at `samples\Infinispan.HotRod.SimpleSample`. In order to use it, you need to create `.NET Core 2+` app or `.NET Framework 4.6.1+`.

## Build on windows

You need to have [.NET Core SDK](http://dot.net/core) installed. Tools other than that are downloaded from internet to `./buildtools`. It uses [FAKE](http://fake.build) internally, bootstrapped from `.\build.ps1`.

Once you run `.\build.ps1`, it will create a cache with all tools necessary, so that it is not downloaded all the time. If you want to wipe that, simply delete folder `buildtools/tmp`.

For running tests, you also need to have Java installed (JDK tested). You may install JAVA using [chocolatey](https://chocolatey.org) by running `choco install jdk8`.

## Build on Linux

The client can be consumed from any linux x64 where with [.NET Core SDK](http://dot.net/core) and [Mono](http://mono-project.com) (guide for CentOS 7 works OK on Fedora 26) installed. Build was tested on Fedora 26, but theoretically can run anywhere. It requires `swig`, `unzip` and `wget` installed.

To build make build.sh executable (chmod +x build.sh) and run `./build.sh Build` (any command mentioned here should work with `build.sh` instead of `build.ps1`).

### IDE

Unfortunatelly, if you want to open the `Infinispan.HotRod.sln` from your favorite IDE, you need to run at least once `.\build.ps1 Generate` to generate swig and protobuffers.

### Complete build

If you want to build all the things from console, run `.\build.ps1`. If you want to work with the project in IDE (VS/Rider/VSCode), run `build.ps1 Generate`. This will generate files required for build - swig and Google Protobuffers.

### SWIG

C# Hot Rod client is a binding to native C++ client. It uses [swig](http://swig.org) to generate .NET binding. In order for swig to know what to generate, folder `./swig` contains swig templates. Swig will generate C# files which **must not be pushed to this repo** to `src/Infinispan.HotRod/generated/`. If you want to generate just swig files, run `build.ps1 GenerateSwig`. 

### Google Protobuffers

Some functionality of the client also depends on Google Protobuffers. Source is at `./protos` and generated C# files will be at `src/Infinispan.HotRod/generated/`. If you want to generate just proto files, run `build.ps1 GenerateProto`.

### Tests

Tests require Java installed. Then to run tests, you may use Visual Studio (tested with VS2017 15.3) or run `.\build.ps1 Tests`

## Publishing

To publish nuget package to [nuget.org](https://nuget.org), run `.\build.ps1 Publish`.

## Cpp-client

This binding relies on having cpp-client avaialable via nuget package. This build is also part of this repo. When new version of cpp-client is released, publish it to [nuget.org](https://nuget.org) by running `.\build.ps1 CppPublish`.

## Reporting Issues

Infinispan uses JIRA for issue management, hosted on issues.jboss.org
(https://issues.jboss.org/browse/HRCPP). You can log in using your jboss.org
username and password.
