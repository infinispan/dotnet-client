echo "Using ISPN_HOME: " %ISPN_HOME%
rmdir /S /Q %ISPN_HOME%\bin\testconfigs
mkdir %ISPN_HOME%\bin\testconfigs
copy ..\testconfigs\*.* %ISPN_HOME%\bin\testconfigs