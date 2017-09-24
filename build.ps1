# This file contains necesary code to be able to run FAKE, everything else is in build.fsx
Push-Location buildtools

if (-not (Test-Path "tmp")) {
    mkdir tmp
}

$nugetSource = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
$nugetPath = Join-Path (Get-Location) "tmp/nuget/nuget.exe"
if (-not (Test-Path $nugetPath)) {
    mkdir tmp/nuget
    (New-Object System.Net.WebClient).DownloadFile($nugetSource, $nugetPath)    
}

$fakePath = Join-Path (Get-Location) "tmp/FAKE/tools/FAKE.exe"
if (-not (Test-Path $fakePath)) {
    .\tmp\nuget\nuget.exe "install" "FAKE" "-ExcludeVersion" "-Version" "4.63.2" "-OutputDirectory" "tmp"
}

$target = "build"
if($args.Count -ge 1)
{
    $target = $args[0]
}

$target = "target=" + $target

& $fakePath "build.fsx" $target
$fakeExitCode = $LASTEXITCODE

Pop-Location

exit $fakeExitCode
