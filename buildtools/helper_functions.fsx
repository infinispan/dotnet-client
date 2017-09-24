[<AutoOpen>]
module BuildHelpers

#r @"tmp/FAKE/tools/FakeLib.dll"
open System
open System.Net
open System.IO
open Fake
open Fake.NuGet.Install
open Fake.ArchiveHelper.Tar.GZip

let downloadArtifact (url:string) (downloadLocation:string) =
    let client = new WebClient()
    client.DownloadFile(url, downloadLocation)

let downloadCppClientIfNonexist cppClientVersion =
    // FAKE uses ICSharpLibZip which is capable of extracting zips, but cpp-client.zip is corrupted for it
    // java is known to create invalid headers. 7zip can extract it without checking it
    // more info at http://community.sharpdevelop.net/forums/t/9055.aspx
    let zipPath = "tmp/7-zip.CommandLine/tools/7za.exe"
    if not (File.Exists zipPath) then
        NugetInstall (fun p ->
            {p with
                ToolPath = "./tmp/nuget/nuget.exe";
                OutputDirectory = "tmp";
                ExcludeVersion = true}) "7-Zip.CommandLine"
    let cppClientDirectory = sprintf "tmp/infinispan-hotrod-cpp-%s-WIN-x86_64" cppClientVersion
    if not (Directory.Exists cppClientDirectory) then
        let cppClientUrl = sprintf "http://downloads.jboss.org/infinispan/HotRodCPP/%s/infinispan-hotrod-cpp-%s-WIN-x86_64.zip" cppClientVersion cppClientVersion;
        trace (sprintf "downloading cpp-client version %s" cppClientVersion)
        downloadArtifact cppClientUrl "tmp/cpp-client.zip"
        trace "client downloaded, unziping"
        let unzip = ExecProcess (fun p ->
            p.FileName <- zipPath
            p.Arguments <- "x cpp-client.zip"
            p.WorkingDirectory <- "tmp") (TimeSpan.FromMinutes 5.0)
        trace "client unziped"
    else
        trace "cpp client already downloaded, skipping"
    cppClientDirectory

let downloadSwigToolsIfNonexist swigVersion =
    let swigLocation = sprintf "tmp/swigwintools/tools/swigwin-%s/swig.exe" swigVersion
    if not (File.Exists swigLocation) then
        NugetInstall (fun p ->
            {p with
                Version = swigVersion;
                ToolPath = "./tmp/nuget/nuget.exe";
                OutputDirectory = "tmp";
                ExcludeVersion = true}) "swigwintools"
    else
        trace "swig tools already exists, skipping"
    swigLocation
    

let downloadProtocIfNonexist protocVersion =
    let protocLocation = "tmp/Google.Protobuf.Tools/tools/windows_x64/protoc.exe"
    if not (File.Exists protocLocation) then
        NugetInstall (fun p ->
            {p with
                Version = protocVersion;
                ToolPath = "./tmp/nuget/nuget.exe";
                OutputDirectory = "tmp";
                ExcludeVersion = true}) "Google.Protobuf.Tools"
    else
        trace "protoc already exists, skipping"
    protocLocation