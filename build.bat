

set HOTRODCPP32_HOME=%checkoutDir%/cpp-client/build_win32/_CPack_Packages/WIN-i686/ZIP/infinispan-hotrod-cpp-%cppTag%-WIN-i686
set HOTRODCPP64_HOME=%checkoutDir%/cpp-client/build_win64/_CPack_Packages/WIN-x86_64/ZIP/infinispan-hotrod-cpp-%cppTag%-WIN-x86_64


call:do_build %generator% 64
call:do_build %generator% 32 

goto eof

:do_build

setlocal
set "arch=%~2"
set "build_dir=build_win%arch%"

if "%arch%" == "32" (
  set "JAVA_HOME=%JAVA_HOME_32%"
  set "full_generator=%~1"
rmdir /s /q build_windows_32
mkdir build_windows_32
cd build_windows_32
)

if "%arch%" == "64" (
  set "JAVA_HOME=%JAVA_HOME_64%"
  set "full_generator=%~1 Win64"
rmdir /s /q build_windows_64
mkdir build_windows_64
cd build_windows_64
)



cmake -G "%full_generator%" -DHOTRODCPP32_HOME=%HOTRODCPP32_HOME% -DHOTRODCPP64_HOME=%HOTRODCPP64_HOME% ..
if %errorlevel% neq 0 goto fail

cmake --build . --config RelWithDebInfo
if %errorlevel% neq 0 goto fail

if  not "%buildTest%"=="skip" ( 
ctest -V -C RelWithDebInfo
if %errorlevel% neq 0 goto fail
)

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

:fail
    cd ..
    exit /b 1
:eof
