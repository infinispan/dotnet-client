if [%generator%] == [""] set generator="Visual Studio 17 2022"
echo Using generator -G %generator%

set home_drive=%CD:~0,2%

rmdir /s /q build_windows
mkdir build_windows
cd build_windows

set "buildTest=%~1"

if [%CLIENT_VERSION%] neq [] set V1=%CLIENT_VERSION:*/=%

for /f "tokens=1,2,3,4 delims=." %%a in ("%V1%") do (
   set v_1major=%%a
   set v_2minor=%%b
   set v_3micro=%%c
   set v_4qualifier=%%d
)

rem If empty or not a numnber, set values to default
if [%v_1major%] equ [] set v_1major=0
if [%v_2minor%] equ [] set v_2minor=1
if [%v_3micro%] equ [] set v_3micro=0
if [%v_4qualifier%] equ [] set v_4qualifier=SNAPSHOT

if 1%v_1major% neq +1%v_1major% set v_1major=0
if 1%v_2minor% neq +1%v_2minor% set v_2minor=1
if 1%v_3micro% neq +1%v_3micro% set v_3micro=0

set package_name=%v_1major%.%v_2minor%.%v_3micro%.%v_4qualifier%
set nuget_package_name=%v_1major%.%v_2minor%.%v_3micro%-%v_4qualifier%

if [%HOTRODCPP_HOME%] == [] set HOTRODCPP_HOME=%checkoutDir%/cpp-client/build_win/_CPack_Packages/WIN-x86_64/ZIP/infinispan-hotrod-cpp-%package_name%-WIN-x86_64
echo Using HOTRODCPP_HOME=%HOTRODCPP_HOME%

if "%HOTROD_PREBUILT_LIB_DIR%" == "" (
  set HOTRODCSDLL=hotrodcs.dll
  set PROTOBUFDLL=%GOOGLE_PROTOBUF_NUPKG%\Google.Protobuf.3.8.0\lib\net45\Google.Protobuf.dll
) else (
  set HOTRODCSDLL=%HOTROD_PREBUILT_LIB_DIR%\hotrodcs.dll
  set PROTOBUFDLL=%HOTROD_PREBUILT_LIB_DIR%\Google.Protobuf.dll
)

call :unquote u_generator %generator%
cmake -G "%u_generator%" -DHOTRODCPP_HOME=%HOTRODCPP_HOME% -DHOTROD_VERSION_MAJOR=%v_1major% -DHOTROD_VERSION_MINOR=%v_2minor% -DHOTROD_VERSION_PATCH=%v_3micro% -DHOTROD_VERSION_LABEL=%v_4qualifier% -DSWIG_DIR=%SWIG_DIR% -DSWIG_EXECUTABLE=%SWIG_EXECUTABLE% -DPROTOBUF_PROTOC_EXECUTABLE_CS="%PROTOBUF_PROTOC_EXECUTABLE_CS%" -DGOOGLE_PROTOBUF_NUPKG="%GOOGLE_PROTOBUF_NUPKG%" -DPROTOBUF_INCLUDE_DIR=%PROTOBUF_INCLUDE_DIR% -DJBOSS_HOME=%JBOSS_HOME% -DIKVM_CUSTOM_BIN_PATH=%IKVM_CUSTOM_BIN_PATH% -DOPENSSL_ROOT_DIR=%OPENSSL_ROOT_DIR% -DCONFIGURATION=RelWithDebInfo -DENABLE_DOXYGEN=1 -DENABLE_JAVA_TESTING=FALSE  -DProtobuf_LIBRARIES=%PROTOBUFDLL% %~4 ..
if %errorlevel% neq 0 goto fail
if "%HOTROD_PREBUILT_LIB_DIR%" == "" (
  cmake --build . --config RelWithDebInfo
) else (
  cmake --build . --config RelWithDebInfo --target hotrodcs-test-bin
)
if %errorlevel% neq 0 goto fail

if  not "%buildTest%"=="skip" ( 
ctest -V -C RelWithDebInfo -I 5,7
)

if %errorlevel% neq 0 goto fail

if "%HOTROD_PREBUILT_LIB_DIR%" == "" (
  cpack -G ZIP --config CPackSourceConfig.cmake
  if %errorlevel% neq 0 goto fail

  cpack -G ZIP --config CPackConfig.cmake
  if %errorlevel% neq 0 goto fail

  cpack -G WIX -C RelWithDebInfo
  if %errorlevel% neq 0 goto fail

  cmake %* -P ../wix-bundle.cmake
  if  "%packNuget%"=="true" (
    cpack -G NuGet -R %nuget_package_name% -C RelWithDebInfo
    if %errorlevel% neq 0 goto fail
  )
)
if %errorlevel% neq 0 goto fail
endlocal
goto eof

:unquote
  set %1=%~2
  goto :EOF

:fail
    %home_drive%
    ()
    exit /b 1
:eof
%home_drive%
