[<AutoOpen>]
module BuildHelpers

#r @"tmp/FAKE/tools/FakeLib.dll"
open System
open System.Net
open System.IO
open Fake
open Fake.NuGet.Install
open Fake.ArchiveHelper.Tar.GZip


///**Description**
/// Downloads file from internet
///**Parameters**
///  * `url` - parameter of type `string`
///  * `downloadLocation` - parameter of type `string`
///
///**Output Type**
///  * `unit`
///
///**Exceptions**
///
let downloadArtifact (url:string) (downloadLocation:string) =
    let client = new WebClient()
    client.DownloadFile(url, downloadLocation)


///**Description**
/// Downloads HotRod cpp-client release from jboss.org
///**Parameters**
///  * `cppClientVersion` - parameter of type `string`
///
///**Output Type**
///  * `string`
///
///**Exceptions**
///
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


///**Description**
/// Downloads swig tools from public nuget repository
///**Parameters**
///  * `swigVersion` - parameter of type `string`
///
///**Output Type**
///  * `string`
///
///**Exceptions**
///
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
    


///**Description**
/// downloads protoc compiler from public nuget repository
///**Parameters**
///  * `protocVersion` - parameter of type `string`
///
///**Output Type**
///  * `string`
///
///**Exceptions**
///
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


///**Description**
/// Generates C# files from proto files using protocLocation
///**Parameters**
///  * `protocLocation` - parameter of type `string`
///  * `sourceDir` - parameter of type `string`
///  * `targetDir` - parameter of type `string`
///
///**Output Type**
///  * `unit`
///
///**Exceptions**
///
let generateCSharpFromProtoFiles protocLocation sourceDir targetDir =
    trace (sprintf "running C# generation from proto files in %s to %s" sourceDir targetDir)
    DirectoryInfo(sourceDir).GetFiles "*.proto"
        |> Seq.map (fun protoFile ->
            ExecProcess (fun p ->
                p.FileName <- protocLocation
                p.Arguments <- sprintf "%s --csharp_out=%s" protoFile.Name targetDir
                p.WorkingDirectory <- sourceDir) (TimeSpan.FromMinutes 5.0))
        |> Seq.iter (fun returnCode ->
            if returnCode <> 0 then failwith "could not process proto file")


///**Description**
/// Generates C# files from swig templates using swigToolPath
///**Parameters**
///  * `swigToolPath` - parameter of type `string`
///  * `includePath` - parameter of type `string`
///  * `sourceDir` - parameter of type `string`
///  * `_namespace` - parameter of type `string`
///  * `targetDir` - parameter of type `string`
///
///**Output Type**
///  * `unit`
///
///**Exceptions**
///
let generateCSharpFilesFromSwigTemplates swigToolPath includePath sourceDir _namespace targetDir =
    let swigResult = ExecProcess (fun p ->
        p.FileName <- swigToolPath
        p.Arguments <- sprintf "-csharp -c++ -I%s -Iinclude -v -namespace %s -outdir %s hotrodcs.i" includePath _namespace targetDir
        p.WorkingDirectory <- sourceDir) (TimeSpan.FromMinutes 5.0)
    if swigResult <> 0 then failwith "could not process swig files"