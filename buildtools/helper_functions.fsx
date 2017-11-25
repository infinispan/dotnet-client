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
    if isLinux then
        ExecProcess (fun p ->
            p.FileName <- "wget"
            p.Arguments <- sprintf "-O %s %s" downloadLocation url
            p.WorkingDirectory <- ".") (TimeSpan.FromMinutes 5.0)
        |> ignore
    else
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
/// Unzips file - using tar on linux, and 7zip on windows
///
///**Output Type**
///  * `int`
///
///**Exceptions**
///
let unzipFile file where =
    if isWindows then
        let zipPath = download7zipIfNonexist ()
        ExecProcess (fun p ->
            p.FileName <- zipPath
            p.Arguments <- sprintf "x %s" file
            p.WorkingDirectory <- where) (TimeSpan.FromMinutes 5.0)
    else
        ExecProcess (fun p ->
            p.FileName <- "unzip"
            p.Arguments <- file
            p.WorkingDirectory <- where) (TimeSpan.FromMinutes 5.0)

///**Description**
/// Unzips file - using tar on linux, and 7zip on windows
///
///**Output Type**
///  * `int`
///
///**Exceptions**
///
let unzipRpmFile file where folder =
    ExecProcess (fun p ->
        p.FileName <- "rpm2archive"
        p.Arguments <- file
        p.WorkingDirectory <- where) (TimeSpan.FromMinutes 5.0) |> ignore
    ExecProcess (fun p ->
        p.FileName <- "mkdir"
        p.Arguments <- folder
        p.WorkingDirectory <- where) (TimeSpan.FromMinutes 5.0)  |> ignore
    ExecProcess (fun p ->
        p.FileName <- "tar"
        p.Arguments <- sprintf "xvf %s.%s -C %s" file "tgz" folder
        p.WorkingDirectory <- where) (TimeSpan.FromMinutes 5.0) |> ignore
    

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
    let cppClientDirectory = sprintf "tmp/infinispan-hotrod-cpp-%s-WIN-x86_64" cppClientVersion
    if not (Directory.Exists cppClientDirectory) then
        let cppClientUrl = sprintf "http://downloads.jboss.org/infinispan/HotRodCPP/%s/infinispan-hotrod-cpp-%s-WIN-x86_64.zip" cppClientVersion cppClientVersion;
        trace (sprintf "downloading cpp-client version %s" cppClientVersion)
        downloadArtifact cppClientUrl "tmp/cpp-client.zip"
        trace "client downloaded, unziping"
        if unzipFile "cpp-client.zip" "tmp" <> 0 then failwith "cannot unzip cpp-client.zip"
        trace "client unziped"
    else
        trace "cpp client already downloaded, skipping"
    if isLinux then
        let cppLinuxClientDirectory = sprintf "tmp/cpp-linux-client"
        if not (Directory.Exists cppLinuxClientDirectory) then
            let cppClientUrl = sprintf "http://downloads.jboss.org/infinispan/HotRodCPP/%s/infinispan-hotrod-cpp-%s-RHEL-x86_64.rpm" cppClientVersion cppClientVersion;
            trace (sprintf "downloading cpp-linux-client version %s" cppClientVersion)
            downloadArtifact cppClientUrl "tmp/cpp-linux-client.rpm"
            trace "client downloaded, unziping"
            unzipRpmFile "cpp-linux-client.rpm" "tmp" "cpp-linux-client"
            trace "client unziped"
        else
            trace "cpp linux client already downloaded, skipping"
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
    let infinispanPath = sprintf "tmp/infinispan-server-%s" infinispanServerVersion
    if not (Directory.Exists infinispanPath) then
        let infinispanServerUrl = sprintf "http://downloads.jboss.org/infinispan/%s/infinispan-server-%s-bin.zip" infinispanServerVersion infinispanServerVersion
        trace (sprintf "downloading infinispan server version %s" infinispanServerVersion)
        downloadArtifact infinispanServerUrl "tmp/infinispan.zip"
        trace "infinispan downloaded, unziping"
        if unzipFile "infinispan.zip" "tmp" <> 0 then failwith "cannot unzip infinispan.zip"
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
    if isWindows then
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
    else
        "swig"
    
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
    let location = if isLinux then "tmp/Google.Protobuf.Tools/tools/linux_x64/protoc"
                   else if isMacOS then "tmp/Google.Protobuf.Tools/tools/macosx_x64/protoc"
                   else protocLocation
    DirectoryInfo(sourceDir).GetFiles "*.proto"
        |> Seq.map (fun protoFile ->
            ExecProcess (fun p ->
                p.FileName <- location
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
    let swigLocation = if isLinux then "swig" else swigToolPath
    let swigResult = ExecProcess (fun p ->
        p.FileName <- swigLocation
        p.Arguments <- sprintf "-csharp -c++ -dllimport hotrod_wrap -I%s -Iinclude -v -namespace %s -outdir %s hotrodcs.i" includePath _namespace targetDir
        p.WorkingDirectory <- sourceDir) (TimeSpan.FromMinutes 5.0)
    if swigResult <> 0 then failwith "could not process swig files"
 
let buildSwig () =
    if isWindows then
        build (fun p -> { p with Properties = [
                                                "Configuration", "Release"
                                                "Platform", "x64"
                                              ]}) "../swig/hotrod_wrap.vcxproj"
    else
        ExecProcess (fun p ->
            p.FileName <- "g++"
            p.Arguments <- "-std=c++11 -I include/ hotrodcs_wrap.cxx -shared -fPIC -o hotrodcs_wrap.so"
            p.WorkingDirectory <- "../swig") (TimeSpan.FromMinutes 5.0) |> ignore
        ExecProcess (fun p ->
            p.FileName <- "g++"
            p.Arguments <- "-std=c++11 -I include/ hotrodcs_wrap.cxx -c"
            p.WorkingDirectory <- "../swig") (TimeSpan.FromMinutes 5.0) |> ignore