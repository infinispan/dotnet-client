rmdir /s /q build_windows
mkdir build_windows
cd build_windows

cmake %* ..
if %errorlevel% neq 0 goto fail

cmake --build . --config RelWithDebInfo
if %errorlevel% neq 0 goto fail

ctest -V -C RelWithDebInfo
if %errorlevel% neq 0 goto fail

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
