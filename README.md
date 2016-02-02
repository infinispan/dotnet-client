# C# Hot Rod Client #

## Build ##
Build prerequisites:

* C++ HotRod Client (www.infinispan.org/hotrod-clients/)
* CMake 2.8 (www.cmake.org)
* C++03 plus shared_ptr TR1 support.
* SWIG 2.0.x (http://www.swig.org)
* .NET Framework 4.0
* NLog 2.1.0 (http://nlog-project.org/)
* NUnit 2.6.3 (https://launchpad.net/nunitv2)
* IKVM.NET 7.2.4630.5 (http://www.ikvm.net/)

Note: after unpacking IKVM please edit the .exe.config files in <ikvm-root>/bin
and comment-out the "<supportedRuntime version="v2.0.50727"/>" element from all
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
    
    set HOTRODCPP32_HOME=/path/to/native/32bit/client
    set HOTRODCPP64_HOME=/path/to/native/64bit/client
    
    set HOTROD_SNK=/path/to/key/to/be/generated
    sn.exe -k %HOTROD_SNK%

    set JBOSS_HOME=/path/to/hotrod/standalone/server

By default the build script will run the unit/integrations tests. If
you want to disable them pass ENABLE_{JAVA,CSHARP}_TESTING=false as flags
on the command line:

    build.bat [-DENABLE_JAVA_TESTING=false] [-DENABLE_CSHARP_TESTING=false]

Any additional build.bat arguments you might add will be passed on to cmake
during the build script generation phase.

After the script completes successfully you can find the .msi installer in
the build_windows/ subdirectory.

Support for building the client using Mono (http://www.mono-project.com) will
be coming soon.

## Reporting Issues ##
Infinispan uses JIRA for issue management, hosted on issues.jboss.org
(https://issues.jboss.org/browse/HRCPP). You can log in using your jboss.org
username and password.