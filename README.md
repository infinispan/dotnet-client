# C# Hot Rod Client #

## Build ##
Build prerequisites:

* C++ HotRod Client (www.infinispan.org/hotrod-clients/)
* CMake 2.8 (www.cmake.org)
* C++03 plus shared_ptr TR1 support.
* SWIG 2.0.x (http://www.swig.org)
* .NET Framework 4.0 or Mono (http://www.mono-project.com)
* NLog 2.1.0 (http://nlog-project.org/)
* NUnit 2.6.3 (https://launchpad.net/nunitv2)

Documentation building requirements:
* Doxygen (http://doxygen.org)

Package building requirements:
* WiX (http://wixtoolset.org)

Build steps:
    set NLOG_DLL=/path/to/nlog/2.1.0/dll
    
    set HOTRODCPP32_HOME=/path/to/native/32bit/client
    set HOTRODCPP64_HOME=/path/to/native/64bit/client
    
    set HOTROD_SNK=/path/to/key/to/be/generated
    sn.exe -k %HOTROD_SNK%

    set JBOSS_HOME=/path/to/hotrod/standalone/server

    build.bat
