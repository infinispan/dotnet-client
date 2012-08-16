echo "Using ISPN_HOME: " %ISPN_HOME%
echo off
copy testresources\*.xml %ISPN_HOME%\etc /Y
copy testresources\*.bat %ISPN_HOME%\bin /Y