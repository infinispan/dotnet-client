rmdir /s /q build_windows
mkdir build_windows
cd build_windows


set HOTRODCPP32_HOME=%checkoutDir%/cpp-client/build_win32/_CPack_Packages/WIN-i686/ZIP/infinispan-hotrod-cpp-%cppTag%-WIN-i686
set HOTRODCPP64_HOME=%checkoutDir%/cpp-client/build_win64/_CPack_Packages/WIN-x86_64/ZIP/infinispan-hotrod-cpp-%cppTag%-WIN-x86_64

set full_generator="%generator:"=% Win64"

cmake -G %full_generator% -DHOTRODCPP32_HOME=%HOTRODCPP32_HOME% -DHOTRODCPP64_HOME=%HOTRODCPP64_HOME% ..
if %errorlevel% neq 0 goto fail

cmake --build . --config RelWithDebInfo
if %errorlevel% neq 0 goto fail

if  not "%buildTest%"=="skip" ( 
ctest -V -C RelWithDebInfo
if %errorlevel% neq 0 goto fail
)

cpack -G ZIP --config CPackSourceConfig.cmake
if %errorlevel% neq 0 goto fail

cpack -G WIX -C RelWithDebInfo
if %errorlevel% neq 0 goto fail

cmake %* -P ../wix-bundle.cmake
if %errorlevel% neq 0 goto fail
cd ..
goto eof

:fail
    cd ..
    exit /b 1
:eof
