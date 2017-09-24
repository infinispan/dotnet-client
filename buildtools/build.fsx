#r @"tmp/FAKE/tools/FakeLib.dll"
#load "helper_functions.fsx"

open System
open System.IO
open Fake
open Fake.NuGet.Install
open Fake.DotNetCli

let cppClientVersion = "8.2.0.Alpha1"
let swigVersion = "3.0.12"
let protobufVersion = "3.4.0" // if changing this, be sure to also update Google.Protobuf in src/Infinispan.HotRod/Infinispan.HotRod.csproj

let buildDir = "../build"
let generateDir = "../src/Infinispan.HotRod/generated"

Target "Clean" (fun _ ->
    CleanDirs [buildDir; generateDir]
)

Target "GenerateProto" (fun _ ->
    let protocLocation = downloadProtocIfNonexist protobufVersion
    DirectoryInfo("../protos").GetFiles "*.proto"
        |> Seq.map (fun protoFile ->
            ExecProcess (fun p ->
                p.FileName <- protocLocation
                p.Arguments <- sprintf "%s --csharp_out=%s" protoFile.Name generateDir
                p.WorkingDirectory <- "../protos") (TimeSpan.FromMinutes 5.0))
        |> Seq.iter (fun returnCode ->
            if returnCode <> 0 then failwith "could not process proto file")
    trace "proto files generated"
)

Target "GenerateSwig" (fun _ ->
    let cppClientLocation = downloadCppClientIfNonexist cppClientVersion
    let swigToolPath = downloadSwigToolsIfNonexist swigVersion
    let cppClientInclude = @"..\buildtools" @@ cppClientLocation @@ "include" // remember, it's gonna run from ../swig folder
    trace "running swig generation"
    let swigResult = ExecProcess (fun p ->
        p.FileName <- swigToolPath
        p.Arguments <- sprintf "-csharp -c++ -I%s -Iinclude -v -namespace Infinispan.HotRod.SWIGGen -outdir %s hotrodcs.i" cppClientInclude generateDir
        p.WorkingDirectory <- "../swig") (TimeSpan.FromMinutes 5.0)
    if swigResult <> 0 then failwith "could not process swig files"
    trace "swig generated"
)

Target "Generate" (fun _ ->
    trace "proto files and swig files generated"
)

Target "SetVersion" (fun _ ->
    trace "version set"
)

Target "Build" (fun _ ->
    Build (fun p -> { p with Project = "../src/Infinispan.HotRod/Infinispan.HotRod.csproj"})
    trace "solution built"
)

Target "UnitTest" (fun _ ->
    trace "unit tests done"
)

Target "IntegrationTest" (fun _ ->
    trace "integration tests done"
)

Target "Publish" (fun _ ->
    trace "published"
)

// targets chain
"Clean" ==> "GenerateProto" ==> "GenerateSwig" ==> "Generate" ==> "SetVersion" ==> "Build"
    ==> "UnitTest" ==> "IntegrationTest" ==> "Publish"

RunParameterTargetOrDefault "target" "Build"