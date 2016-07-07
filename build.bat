if [%HOTRODCPP_HOME%] == [] set HOTRODCPP_HOME=%checkoutDir%/cpp-client/build_win/_CPack_Packages/WIN-x86_64/ZIP/infinispan-hotrod-cpp-%cppTag%-WIN-x86_64
echo Using HOTRODCPP_HOME=%HOTRODCPP_HOME%
if [%generator%] == [""] set generator="Visual Studio 14 2015 Win64"
echo Using generator -G %generator%

rmdir /s /q build_windows
mkdir build_windows
cd build_windows

set "buildTest=%~1"

call :unquote u_generator %generator%
cmake -G "%u_generator%" -DHOTRODCPP_HOME=%HOTRODCPP_HOME% -DPROTOBUF_PROTOC_EXECUTABLE_CS="%PROTOBUF_PROTOC_EXECUTABLE_CS%" -DGOOGLE_PROTOBUF_NUPKG="%GOOGLE_PROTOBUF_NUPKG%" -DPROTOBUF_INCLUDE_DIR=%PROTOBUF_INCLUDE_DIR% -DJBOSS_HOME=%JBOSS_HOME% -DIKVM_CUSTOM_BIN_PATH=%IKVM_CUSTOM_BIN_PATH% -DOPENSSL_ROOT_DIR=%OPENSSL_ROOT_DIR% -DCONFIGURATION=RelWithDebInfo %~4 ..
if %errorlevel% neq 0 goto fail

cmake --build . --config RelWithDebInfo
if %errorlevel% neq 0 goto fail

if  not "%buildTest%"=="skip" ( 
ctest -V -C RelWithDebInfo
)
if %errorlevel% neq 0 goto fail

cpack -G ZIP --config CPackSourceConfig.cmake
if %errorlevel% neq 0 goto fail

cpack -G ZIP --config CPackConfig.cmake
if %errorlevel% neq 0 goto fail

cpack -G WIX -C RelWithDebInfo
if %errorlevel% neq 0 goto fail

cmake %* -P ../wix-bundle.cmake
if %errorlevel% neq 0 goto fail
cd ..
endlocal
goto eof

:unquote
  set %1=%~2
  goto :EOF

:fail
    cd ..
    exit /b 1
:eof
