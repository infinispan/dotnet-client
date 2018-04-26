# C# Hot Rod Client #

Two build modes are available:

* Build mode 1 produces an .msi package to be used on Windows
* Build mode 2 produces a multiplatform .NET Core artifact

## Build mode 1
Build prerequisites:

* C++ HotRod Client (www.infinispan.org/hotrod-clients/)
* CMake 3.x (www.cmake.org)
* C++11
* SWIG 2.0.x (http://www.swig.org)
* .NET Framework 4.5
* NLog 2.1.0 (http://nlog-project.org/)
* Google.Protobuf 3.4.0 .NET assembly with protoc
* NUnit 3.8.0 (https://github.com/nunit/nunit)
* IKVM.NET 8.1.5717.0 (http://www.ikvm.net/)
* For running tests, you also need to have Java 8 installed (JDK tested).

Note: after unpacking IKVM please edit the .exe.config files in <ikvm-root>/bin
and comment-out the "\<supportedRuntime version="v2.0.50727"/\>" element from all
of them.

Documentation building requirements:
* Doxygen (http://doxygen.org)

Package building requirements:
* WiX (http://wixtoolset.org)

After you install the dependencies please update the PATH environment
variable to include the bin/ directories of CMake, SWIG, Maven, NUnit, IKVM.

Build steps:

    set NLOG_DLL=/path/to/nlog/2.1.0/dll
    set NUNIT_DLL=/path/to/nunit.framework.dll
    
    set HOTRODCPP_HOME=/path/to/native/64bit/client
    
    set HOTROD_SNK=/path/to/key/to/be/generated
    sn.exe -k %HOTROD_SNK%

    set JBOSS_HOME=/path/to/hotrod/standalone/server
    
    set PROTOBUF_PROTOC_EXECUTABLE_CS=/path/to/protoc.exe (typically C:\Users\%USERNAME%\google.grotobuf.install.dir\tools\protoc.exe}
    set GOOGLE_PROTOBUF_NUPKG=/parent/folder/of/google.protobuf.install.dir  (typically C:\Users\%USERNAME%)
    set OPENSSL_ROOT_DIR=/path/to/openssl/install/dir (typically C:\OpenSSL-Win64)

By default the build script will run the unit/integrations tests. If
you want to disable them pass ENABLE_{JAVA,CSHARP}_TESTING=false as flags
on the command line:

    build.bat [-DENABLE_JAVA_TESTING=false] [-DENABLE_CSHARP_TESTING=false]

Any additional build.bat arguments you might add will be passed on to cmake
during the build script generation phase.

After the script completes successfully you can find the .msi installer in
the build_windows/ subdirectory.

Optionally you can build a bundle to include the .NET and C++ runtimes which are
project dependencies.

Important: Make sure the runtimes to be bundled match the ones used for the actual build!

To build the bundle define:

    set HOTROD_VCREDIST_x86=<path to vcredist_x86.exe>
    set HOTROD_VCREDIST_x64=<path to vcredist_x64.exe>
    set HOTROD_DOTNET=<path to the .NET runtime standalone installer>

and then pass -DHOTROD_BUILD_BUNDLE=true on the command line as argument to build.bat. After
the build is complete you cand find the package in build_windows/ with a name ending in
"-bundle.exe".

Support for building the client using Mono (http://www.mono-project.com) will
be coming soon.

## Build mode 2

Requirements:

* .NET Core SDK and Runtime. Tools other than that are downloaded from internet to `./buildtools`. It uses [FAKE](http://fake.build) internally, bootstrapped from `.\build.ps1`.

* mono, unzip and wget on Linux.
* the C++ native client. This is automatically downloaded from the build script or a local version can be used pointing the env var HOTROD_PREBUILT_DIR to the root of the unpacked package (cmake/cpack usually puts these files under /build_dir/_CPack_Packages)

### On windows
Once you run `.\build.ps1`, it will create a cache with all tools necessary, so that it is not downloaded all the time. If you want to wipe that, simply delete folder `buildtools/tmp`.

### On Linux
To build make build.sh executable (chmod +x build.sh) and run `./build.sh Build` (any command mentioned here should work with `build.sh` instead of `build.ps1`).

#### Link the correct C++ core
The C++ client library must match the compiler version in use for the swig wrapper. If the libraries download by the script (from ci.infinispan.org) do not fit your needs just compile the C++ client from the source and point HOTROD_PREBUILT_DIR to the dist directory (the dir containing the unpacked .rpm), thus:

    HOTROD_PREBUILT_DIR=<path to unpacked rpm> ./build.sh
    & { $env:HOTROD_PREBUILT_DIR="<path to unpacked zip> ; .\build.ps1 }

### Packaging
To produce the .nupkg package just run the build with the *Pack* or *QuickPack* (if the project has already been built) argument.
The toolchain produces two types of assembly with or without the natives:

* if HOTROD_ARCH env var is not empty an Infinispan.HotRod.$HOTROD_ARCH.nupkg containing the natives dll is produced;
* if HOTROD_ARCH env var is empty (or undef) an Inifnispan.HotRod.nupkg containing the toolchaing to build the natives is produced.


### Building the native wrapper
Install the Infinispan.Hotrod.nupkg and set HOTROD_PREBUILT_DIR to the C++ client install path then run:

    build.[sh|ps1] BuildSwigWrapper

### Run the test suite
The system must be able to find the native libraries under the swig/native_client/lib folder so add it either to the LD_LIBRARY_PATH (linux) or to the PATH (win) variable. Define a JBOSS_HOME pointing to your Infinispan server location (in Windows use the Unix file separator '/') and then run the test suite with dotnet:
	dotnet test 

## Reporting Issues ##
Infinispan uses JIRA for issue management, hosted on issues.jboss.org
(https://issues.jboss.org/browse/HRCPP). You can log in using your jboss.org
username and password.
