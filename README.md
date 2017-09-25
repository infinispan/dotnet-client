# C# Hot Rod Client

Hot Rod client enables you to connect to [infinispan](http://infinispan.org).

> Currently it builds only on windows x64

## Build

You need to have [.NET Core SDK](http://dot.net/core) installed. Tools other than that are downloaded from internet to `./buildtools`. It uses [FAKE](http://fake.build) internally, bootstrapped from `build.ps1`.

Once you run `build.ps1`, it will create a cache with all tools necessary, so that it is not downloaded all the time. If you want to wipe that, simply delete folder `buildtools/tmp`.

### IDE

Unfortunatelly, if you want to open the `Infinispan.HotRod.sln` from your favorite IDE, you need to run at least once `build.ps1 Generate` to generate swig and protobuffers.

### Complete build

If you want to build all the things from console, run `build.ps1`. If you want to work with the project in IDE (VS/Rider/VSCode), run `build.ps1 Generate`. This will generate files required for build - swig and Google Protobuffers.

### SWIG

C# Hot Rod client is a binding to native C++ client. It uses [swig](http://swig.org) to generate .NET binding. In order for swig to know what to generate, folder `./swig` contains swig templates. Swig will generate C# files which **must not be pushed to this repo** to `src/Infinispan.HotRod/generated/`. If you want to generate just swig files, run `build.ps1 GenerateSwig`. 

### Google Protobuffers

Some functionality of the client also depends on Google Protobuffers. Source is at `./protos` and generated C# files will be at `src/Infinispan.HotRod/generated/`. If you want to generate just proto files, run `build.ps1 GenerateProto`.

## Reporting Issues ##

Infinispan uses JIRA for issue management, hosted on issues.jboss.org
(https://issues.jboss.org/browse/HRCPP). You can log in using your jboss.org
username and password.
