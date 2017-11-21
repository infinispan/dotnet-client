[<AutoOpen>]
module BuildHelpers

#r @"tmp/FAKE/tools/FakeLib.dll"
open System
open System.Net
open System.IO
open Fake
open Fake.NuGet.Install
open Fake.ArchiveHelper.Tar.GZip

let nugetPath = "./tmp/nuget/nuget.exe"

// Copy directory function
let rec directoryCopy srcPath dstPath copySubDirs =

    if not <| System.IO.Directory.Exists(srcPath) then
        let msg = System.String.Format("Source directory does not exist or could not be found: {0}", srcPath)
        raise (System.IO.DirectoryNotFoundException(msg))

    if not <| System.IO.Directory.Exists(dstPath) then
        System.IO.Directory.CreateDirectory(dstPath) |> ignore

    let srcDir = new System.IO.DirectoryInfo(srcPath)

    for file in srcDir.GetFiles() do
        let temppath = System.IO.Path.Combine(dstPath, file.Name)
        file.CopyTo(temppath, true) |> ignore

    if copySubDirs then
        for subdir in srcDir.GetDirectories() do
            let dstSubDir = System.IO.Path.Combine(dstPath, subdir.Name)
            directoryCopy subdir.FullName dstSubDir copySubDirs

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
/// Downloads 7-zip from NuGet.org
///
///**Output Type**
///  * `unit`
///
///**Exceptions**
///
let download7zipIfNonexist () =
    let zipPath = "tmp/7-zip.CommandLine/tools/7za.exe"
    if not (File.Exists zipPath) then
        NugetInstall (fun p ->
            {p with
                ToolPath = nugetPath;
                OutputDirectory = "tmp";
                ExcludeVersion = true}) "7-Zip.CommandLine"
    zipPath

///**Description**
/// Downloads HotRod cpp-client release from jboss.org
///**Parameters**
///  * `cppClientVersion` - parameter of type `string`
///
///**Output Type**
///  * `string` - location of cpp-client
///
///**Exceptions**
///
let downloadCppClientIfNonexist cppClientVersion =
    // FAKE uses ICSharpLibZip which is capable of extracting zips, but cpp-client.zip is corrupted for it
    // java is known to create invalid headers. 7zip can extract it without checking it
    // more info at http://community.sharpdevelop.net/forums/t/9055.aspx
    let zipPath = download7zipIfNonexist ()
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
/// Downloads Infinispan server from internet
///**Parameters**
///  * `infinispanServerVersion` - parameter of type `string`
///
///**Output Type**
///  * `string` - location of infinispan server
///
///**Exceptions**
///
let downloadInfinispanIfNeeded infinispanServerVersion =
    let zipPath = download7zipIfNonexist ()
    let infinispanPath = sprintf "tmp/infinispan-server-%s" infinispanServerVersion
    if not (Directory.Exists infinispanPath) then
        let infinispanServerUrl = sprintf "http://downloads.jboss.org/infinispan/%s/infinispan-server-%s-bin.zip" infinispanServerVersion infinispanServerVersion
        trace (sprintf "downloading infinispan server version %s" infinispanServerVersion)
        downloadArtifact infinispanServerUrl "tmp/infinispan.zip"
        trace "infinispan downloaded, unziping"
        let unzip = ExecProcess (fun p ->
            p.FileName <- zipPath
            p.Arguments <- "x infinispan.zip"
            p.WorkingDirectory <- "tmp") (TimeSpan.FromMinutes 5.0)
        trace "infinispan unziped"
    infinispanPath

///**Description**
/// Downloads swig tools from public nuget repository
///**Parameters**
///  * `swigVersion` - parameter of type `string`
///
///**Output Type**
///  * `string` - location of swig tools
///
///**Exceptions**
///
let downloadSwigToolsIfNonexist swigVersion =
    let swigLocation = sprintf "tmp/swigwintools/tools/swigwin-%s/swig.exe" swigVersion
    if not (File.Exists swigLocation) then
        NugetInstall (fun p ->
            {p with
                Version = swigVersion;
                ToolPath = nugetPath;
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
///  * `string` - location of protoc
///
///**Exceptions**
///
let downloadProtocIfNonexist protocVersion =
    let protocLocation = "tmp/Google.Protobuf.Tools/tools/windows_x64/protoc.exe"
    if not (File.Exists protocLocation) then
        NugetInstall (fun p ->
            {p with
                Version = protocVersion;
                ToolPath = nugetPath;
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
        p.Arguments <- sprintf "-csharp -c++ -dllimport hotrod_wrap -I%s -Iinclude -v -namespace %s -outdir %s hotrodcs.i" includePath _namespace targetDir
        p.WorkingDirectory <- sourceDir) (TimeSpan.FromMinutes 5.0)
    if swigResult <> 0 then failwith "could not process swig files"
 