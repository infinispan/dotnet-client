#r @"tmp/FAKE/tools/FakeLib.dll"
#load "helper_functions.fsx"

open System
open System.IO
open Fake
open Fake.NuGet.Install
open Fake.DotNetCli

let cppClientVersion = "8.2.0.Alpha1"
let cppClientPackageVersion = "8.2.0-Alpha1" // nuget does not support string values after .
let swigVersion = "3.0.12"
let protobufVersion = "3.4.0" // if changing this, be sure to also update Google.Protobuf in src/Infinispan.HotRod/Infinispan.HotRod.csproj
let nunitToolsVersion = "2.6.1"
let infinispanServerVersion = "9.1.1.Final"

let generateDir = "../src/Infinispan.HotRod/generated"
let generateTestDir = "../test/Infinispan.HotRod.Tests/generated"

Target "Clean" (fun _ ->
    // git will not preserve empty folders, so ensure they exist because the tools expect them to exist and clean them
    [generateDir; generateTestDir]
        |> Seq.iter ensureDirectory
    CleanDirs [generateDir; generateTestDir]
)

Target "GenerateProto" (fun _ ->
    trace "running generation of proto files"
    let protocLocation = downloadProtocIfNonexist protobufVersion
    generateCSharpFromProtoFiles protocLocation "../protos" generateDir
    trace "proto files generated"
)

Target "GenerateProtoForTests" (fun _ ->
    trace "running generation of proto files for tests"
    let protocLocation = downloadProtocIfNonexist protobufVersion
    generateCSharpFromProtoFiles protocLocation "../test/resources/proto3" ("../../" + generateTestDir)
    trace "proto files for tests generated"
)

Target "GenerateSwig" (fun _ ->
    trace "running swig generation"
    let cppClientLocation = downloadCppClientIfNonexist cppClientVersion
    let swigToolPath = downloadSwigToolsIfNonexist swigVersion
    let cppClientInclude = @"../buildtools" @@ cppClientLocation @@ "include" // remember, it's gonna run from ../swig folder
    let sourceDir = "../swig"
    let _namespace = "Infinispan.HotRod.SWIGGen"
    generateCSharpFilesFromSwigTemplates swigToolPath cppClientInclude sourceDir _namespace generateDir
    trace "swig generated"
)

Target "BuildSwigWraper" (fun _ ->
    trace "running swig wraper build"
    let cppClientLocation = downloadCppClientIfNonexist cppClientVersion
    directoryCopy (cppClientLocation @@ "include") "../swig/include" true
    directoryCopy (cppClientLocation @@ "lib") "../swig/native_client/lib" true
    buildSwig ()
)

Target "Generate" (fun _ ->
    trace "proto files and swig files generated"
)

Target "Build" (fun _ ->
    Build (fun p -> { p with Project = "../Infinispan.HotRod.sln"})
    trace "solution built"
)

Target "ObtainInfinispan" (fun _ ->
    let infinispanLocation = downloadInfinispanIfNeeded infinispanServerVersion
    setEnvironVar "JBOSS_HOME" infinispanLocation
    trace "Infinispan obtained"
)

Target "UnitTest" (fun _ ->
    Test (fun p -> { p with Project = "../test/Infinispan.HotRod.Tests/Infinispan.HotRod.Tests.csproj"
                            AdditionalArgs = ["--logger \"trx;LogFileName=TestResults.trx\""; "--no-build"] } )
    trace "unit tests done"
)

Target "IntegrationTest" (fun _ ->
    trace "integration tests done"
)

Target "Test" (fun _ ->
    trace "tests done"
)

Target "Pack" (fun _ ->
    Pack (fun p -> { p with Project = "../Infinispan.HotRod.sln"})
    trace "packages created"
)

Target "Publish" (fun _ ->
    trace "published"
)

// CPP client targets

Target "CppPackage" (fun _ ->
    let packageRoot = "tmp/Infinispan.HotRod.Cpp-Client/"
    let binsPath = "tmp/Infinispan.HotRod.Cpp-Client/runtimes/win7-x64/native"
    ensureDirectory binsPath
    let cppClientLocation = downloadCppClientIfNonexist cppClientVersion
    directoryCopy "../swig/build/Release" binsPath false
    Copy binsPath (Directory.EnumerateFiles (sprintf "%s/lib" cppClientLocation))
    Copy packageRoot ["Infinispan.HotRod.Cpp-client.win7-x64.nuspec"]
    NuGetPack (fun p ->
                    { p with
                        OutputPath = "tmp"
                        WorkingDir = packageRoot
                        ToolPath = "tmp/nuget/nuget.exe"
                        Version = cppClientPackageVersion
                        Publish = false }) "Infinispan.HotRod.Cpp-client.win7-x64.nuspec"
    trace "cpp-client package created"
)

Target "CppPackagePublish" (fun _ ->
    trace "cpp-client package published"
)

// main targets chain
"Clean" ==> "GenerateProto" ==> "GenerateProtoForTests" ==> "GenerateSwig" ==> "BuildSwigWraper" ==> "Generate" ==> "Build"
    ==> "ObtainInfinispan" ==> "UnitTest" ==> "IntegrationTest" ==> "Test" ==> "Pack" ==> "Publish"

// CPP client chain - run with each new cpp-client release
"BuildSwigWraper" ==> "CppPackage" ==> "CppPackagePublish"

RunParameterTargetOrDefault "target" "Build"