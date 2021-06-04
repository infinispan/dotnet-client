## Run testsuite against prebuilt package
Prerequisites:
* CMake 3.x (www.cmake.org)
* .NET Framework 4.5
* NLog 2.1.0 (http://nlog-project.org/)
* NUnit 3.8.0 (https://github.com/nunit/nunit)
* Hotrod .Net Client installed

Procedure:
1. Prepare the environment with the following settings:
```
set HOTROD_PREBUILT_LIB_DIR= path to the installed infinispan-hotrod-dotnet.smi libraries (ends with /lib)
set NLOG_DLL= your path to the NLog.2.1.0\lib\net45\NLog.dll
set NLOG_LICENSE=path to project\dotnet-client\license.txt
set JBOSS_HOME= path to infinispan server home dir
set PATH= path to NUnit.ConsoleRunner.3.7.0\tools;%PATH%
set NUNIT_DLL= path to NUnit.3.8.0\lib\net45\nunit.framework.dll
set generator="Visual Studio 14 2015 Win64" (update to your VS version)
```
2. run build.bat from the project home page. This will build the testsuite and run it against the library.
