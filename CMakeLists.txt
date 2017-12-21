cmake_minimum_required(VERSION 2.8)

project(infinispan-hotrod-dotnet)

if (NOT DEFINED HOTROD_VERSION_MAJOR OR "${HOTROD_VERSION_MAJOR}" STREQUAL "")
	set (HOTROD_VERSION_MAJOR "8")
endif (NOT DEFINED HOTROD_VERSION_MAJOR OR "${HOTROD_VERSION_MAJOR}" STREQUAL "")

if (NOT DEFINED HOTROD_VERSION_MINOR OR "${HOTROD_VERSION_MINOR}" STREQUAL "")
	set (HOTROD_VERSION_MINOR "1")
endif (NOT DEFINED HOTROD_VERSION_MINOR OR "${HOTROD_VERSION_MINOR}" STREQUAL "")

if (NOT DEFINED HOTROD_VERSION_PATCH OR "${HOTROD_VERSION_PATCH}" STREQUAL "")
	set (HOTROD_VERSION_PATCH "1")
endif (NOT DEFINED HOTROD_VERSION_PATCH OR "${HOTROD_VERSION_PATCH}" STREQUAL "")

if (NOT DEFINED HOTROD_VERSION_LABEL OR "${HOTROD_VERSION_LABEL}" STREQUAL "")
  set (HOTROD_VERSION_LABEL "SNAPSHOT")
endif (NOT DEFINED HOTROD_VERSION_LABEL OR "${HOTROD_VERSION_LABEL}" STREQUAL "")

if ("${CMAKE_BINARY_DIR}" STREQUAL "${CMAKE_CURRENT_SOURCE_DIR}")
  message (FATAL_ERROR "You are calling cmake from the source directory. Please create a separate build directory and call cmake from there. See README.md for details.")
endif ("${CMAKE_BINARY_DIR}" STREQUAL "${CMAKE_CURRENT_SOURCE_DIR}")

string(REGEX MATCH "Win64" ISWIN64 "${CMAKE_GENERATOR}")

if (ISWIN64)
	set(PLATFORM "x64")
else (ISWIN64)
	set(PLATFORM "x86")
endif (ISWIN64)

# Map from version label to assembly revision.
set (REVISION_Alpha1 "0")
set (REVISION_Alpha2 "1")
set (REVISION_Alpha3 "2")
set (REVISION_Alpha4 "3")
set (REVISION_Beta1 "10")
set (REVISION_Beta2 "11")
set (REVISION_Beta3 "12")
set (REVISION_Beta4 "13")
set (REVISION_CR1 "20")
set (REVISION_CR2 "21")
set (REVISION_CR3 "22")
set (REVISION_CR4 "23")
set (REVISION_Final "30")
string(TIMESTAMP REVISION_SNAPSHOT "%y%j" UTC)

# CPack version.
set (CPACK_PACKAGE_VERSION_MAJOR "${HOTROD_VERSION_MAJOR}")
set (CPACK_PACKAGE_VERSION_MINOR "${HOTROD_VERSION_MINOR}")
set (CPACK_PACKAGE_VERSION_PATCH "${HOTROD_VERSION_PATCH}.${HOTROD_VERSION_LABEL}")
set (HOTROD_VERSION "${CPACK_PACKAGE_VERSION_MAJOR}.${CPACK_PACKAGE_VERSION_MINOR}.${CPACK_PACKAGE_VERSION_PATCH}")

# Assembly version.
get_property (HOTROD_VERSION_REVISION VARIABLE PROPERTY "REVISION_${HOTROD_VERSION_LABEL}")
if ("${HOTROD_VERSION_REVISION}" STREQUAL "")
  message (FATAL_ERROR "No revision mapping defined for HOTROD_VERSION_LABEL '${HOTROD_VERSION_LABEL}'. ${HOTROD_VERSION_REVISION}")
endif ("${HOTROD_VERSION_REVISION}" STREQUAL "")
set (HOTROD_ASSEMBLY_VERSION "${HOTROD_VERSION_MAJOR}.${HOTROD_VERSION_MINOR}.${HOTROD_VERSION_PATCH}.${HOTROD_VERSION_REVISION}")
set (HOTROD_ASSEMBLY_PRODUCT_NAME "${CMAKE_PROJECT_NAME}")
set (HOTROD_ASSEMBLY_DESCRIPTION ".NET Hot Rod Client")
set (HOTROD_ASSEMBLY_TITLE "${HOTROD_ASSEMBLY_DESCRIPTION}")
set (HOTROD_ASSEMBLY_COPYRIGHT "Copyright 2009 - 2014, Red Hat Inc. and/or its affiliates.")

set (CPACK_PACKAGE_VENDOR "Infinispan")
set (CPACK_PACKAGE_FILE_NAME "${CMAKE_PROJECT_NAME}-${CPACK_PACKAGE_VERSION_MAJOR}.${CPACK_PACKAGE_VERSION_MINOR}.${CPACK_PACKAGE_VERSION_PATCH}")
set (CPACK_RESOURCE_FILE_LICENSE "${CMAKE_CURRENT_SOURCE_DIR}/License.txt")
set (CPACK_WIX_UPGRADE_GUID "7cc17657-80b5-417f-9209-710682f852f4") # This value should not change.

message ("HotRod Version: ${HOTROD_VERSION}")
message ("HotRod Assembly Version: ${HOTROD_ASSEMBLY_VERSION}")

message (STATUS "HOTRODCPP_HOME value is :" ${HOTRODCPP_HOME} )

find_package(Protobuf REQUIRED)

function(HR_PROTOBUF_GENERATE_CSHARP SRCS)
  if(NOT ARGN)
	  message(SEND_ERROR "Error: PROTOBUF_GENERATE_CSHARP() called without any proto files")
    return()
  endif()

  if(PROTOBUF_GENERATE_CPP_APPEND_PATH)
    # Create an include path for each file specified
    foreach(FIL ${ARGN})
      get_filename_component(ABS_FIL ${FIL} ABSOLUTE)
      get_filename_component(ABS_PATH ${ABS_FIL} PATH)
      list(FIND _protobuf_include_path ${ABS_PATH} _contains_already)
      if(${_contains_already} EQUAL -1)
          list(APPEND _protobuf_include_path -I ${ABS_PATH})
      endif()
    endforeach()
  else()
    set(_protobuf_include_path -I ${CMAKE_CURRENT_SOURCE_DIR})
  endif()

  if(DEFINED PROTOBUF_IMPORT_DIRS)
    foreach(DIR ${PROTOBUF_IMPORT_DIRS})
      get_filename_component(ABS_PATH ${DIR} ABSOLUTE)
      list(FIND _protobuf_include_path ${ABS_PATH} _contains_already)
      if(${_contains_already} EQUAL -1)
          list(APPEND _protobuf_include_path -I ${ABS_PATH})
      endif()
    endforeach()
  endif()

  set(${SRCS})
  foreach(FIL ${ARGN})
    get_filename_component(ABS_FIL ${FIL} ABSOLUTE)
    get_filename_component(FIL_WE ${FIL} NAME_WE)

    list(APPEND ${SRCS} "${CMAKE_CURRENT_BINARY_DIR}/${FIL_WE}.pb.cc")
    list(APPEND ${HDRS} "${CMAKE_CURRENT_BINARY_DIR}/${FIL_WE}.pb.h")

    add_custom_command(
      OUTPUT "${CMAKE_CURRENT_BINARY_DIR}/${FIL_WE}.pb.cc"
             "${CMAKE_CURRENT_BINARY_DIR}/${FIL_WE}.pb.h"
	     COMMAND  ${PROTOBUF_PROTOC_EXECUTABLE_CS}
      ARGS --csharp_out ${CMAKE_CURRENT_BINARY_DIR} ${_protobuf_include_path} ${ABS_FIL}
      DEPENDS ${ABS_FIL}
      COMMENT "Running C# protocol buffer compiler on ${FIL}"
      VERBATIM )

  endforeach()

  set_source_files_properties(${${SRCS}} PROPERTIES GENERATED TRUE)
  set(${SRCS} ${${SRCS}} PARENT_SCOPE)
endfunction()


include_directories(${PROTOBUF_INCLUDE_DIRS})

### Generated and build the SWIG wrapper ###
if (NOT DEFINED HOTRODCPP_HOME AND DEFINED ENV{HOTRODCPP_HOME})
  set (HOTRODCPP_HOME $ENV{HOTRODCPP_HOME})
endif (NOT DEFINED HOTRODCPP_HOME AND DEFINED ENV{HOTRODCPP_HOME})

if (NOT DEFINED HOTRODCPP_HOME)
  message (FATAL_ERROR "Please define HOTRODCPP_HOME.")
endif (NOT DEFINED HOTRODCPP_HOME)

message (STATUS "HOTRODCPP_HOME value is :" ${HOTRODCPP_HOME} )

set (SWIG_BUILD "${CMAKE_CURRENT_BINARY_DIR}/swig")
set (NATIVE_DIR "${SWIG_BUILD}/native_client")
set (NATIVE_LIB_DIR "${NATIVE_DIR}/lib")

file (MAKE_DIRECTORY "${SWIG_BUILD}" "${NATIVE_DIR}")
file (TO_CMAKE_PATH "${HOTRODCPP_HOME}" CMAKE_CPP_HOME)
file (COPY "${CMAKE_CPP_HOME}/lib/" DESTINATION "${NATIVE_LIB_DIR}" PATTERN "*")
file (COPY "${CMAKE_CPP_HOME}/include/" DESTINATION "${NATIVE_DIR}/include" PATTERN "*")

hr_protobuf_generate_csharp(PROTO_SRCS  
  src/resources/proto/base_types.proto
  src/resources/proto/message-wrapping.proto
  src/resources/proto/query.proto
)

hr_protobuf_generate_csharp(TEST_PROTO_SRCS  
  test/resources/proto3/addressbook.proto
  test/resources/proto3/bank.proto
)


if (WIN32)

  add_custom_target(
    swig ALL
    COMMAND cmake -G "${CMAKE_GENERATOR}" "-DHOTRODCPP_HOME=${NATIVE_DIR}" "${CMAKE_CURRENT_SOURCE_DIR}/swig"
    COMMAND cmake --build . --config "${CMAKE_CFG_INTDIR}"
    WORKING_DIRECTORY "${SWIG_BUILD}"
    DEPENDS ${PROTO_SRCS} ${TEST_PROTO_SRCS}
    )

else (WIN32)
#TODO: mono
endif (WIN32)
### Compile the C# code ###
if (NOT DEFINED NLOG_DLL AND DEFINED ENV{NLOG_DLL})
  set (NLOG_DLL $ENV{NLOG_DLL})
endif (NOT DEFINED NLOG_DLL AND DEFINED ENV{NLOG_DLL})
if (NOT DEFINED NLOG_DLL)
  message (FATAL_ERROR "Please define NLOG_DLL.")
endif (NOT DEFINED NLOG_DLL)

if (NOT DEFINED NUNIT_DLL AND DEFINED ENV{NUNIT_DLL})
  set (NUNIT_DLL $ENV{NUNIT_DLL})
endif (NOT DEFINED NUNIT_DLL AND DEFINED ENV{NUNIT_DLL})
if (NOT DEFINED NUNIT_DLL)
  message (FATAL_ERROR "Please define NUNIT_DLL.")
endif (NOT DEFINED NUNIT_DLL)

if (NOT DEFINED NLOG_LICENSE AND DEFINED ENV{NLOG_LICENSE})
  set (NLOG_LICENSE $ENV{NLOG_LICENSE})
  set (NLOG_LICENSE_LOCAL "${CMAKE_CURRENT_BINARY_DIR}/NLog_License.txt")
  file (COPY "${NLOG_LICENSE}" DESTINATION "${CMAKE_CURRENT_BINARY_DIR}")
  get_filename_component (NLOG_LICENSE_NAME "${NLOG_LICENSE}" NAME)
  file (RENAME "${CMAKE_CURRENT_BINARY_DIR}/${NLOG_LICENSE_NAME}" "${NLOG_LICENSE_LOCAL}")
endif (NOT DEFINED NLOG_LICENSE AND DEFINED ENV{NLOG_LICENSE})
if (NOT DEFINED NLOG_LICENSE)
  message (WARNING "Please define NLOG_LICENSE if you plan to distribute the package.")
endif (NOT DEFINED NLOG_LICENSE)

file (COPY "${NLOG_DLL}" DESTINATION "${CMAKE_CURRENT_BINARY_DIR}")
get_filename_component (NLOG_DLL_NAME "${NLOG_DLL}" NAME)
set (NLOG_DLL_LOCAL "${CMAKE_CURRENT_BINARY_DIR}/NLog.dll")
file (RENAME "${CMAKE_CURRENT_BINARY_DIR}/${NLOG_DLL_NAME}" "${NLOG_DLL_LOCAL}")

# GAC requires strong names
if (NOT DEFINED HOTROD_SNK AND DEFINED ENV{HOTROD_SNK})
  set (HOTROD_SNK $ENV{HOTROD_SNK})
endif (NOT DEFINED HOTROD_SNK AND DEFINED ENV{HOTROD_SNK})
if (DEFINED HOTROD_SNK)
  set (KEYFILE "<KeyOriginatorFile>${HOTROD_SNK}</KeyOriginatorFile>")
else (DEFINED HOTROD_SNK)
  set (KEYFILE "")
  message (WARNING "No key provided. The generated DLL won't have a strong name.")
endif (DEFINED HOTROD_SNK)

string(REPLACE "/" "\\" W_S "${CMAKE_CURRENT_SOURCE_DIR}")
string(REPLACE "/" "\\" W_CMAKE_CURRENT_BINARY_DIR "${CMAKE_CURRENT_BINARY_DIR}")
string(REPLACE "/" "\\" W_CMAKE_CURRENT_SOURCE_DIR "${CMAKE_CURRENT_SOURCE_DIR}")
string(REPLACE "/" "\\" W_SWIG_BUILD "${SWIG_BUILD}")

configure_file(templates/hotrodcs.proj.in hotrodcs.csproj)
configure_file(templates/hotrodcs-tests.proj.in hotrodcs-tests.csproj)
configure_file(templates/AssemblyInfo.cs.in AssemblyInfo.cs)

set (ARCH 64)
#configure_file(src/main/cs/Infinispan/HotRod/Impl/RemoteCacheSWIGImpl.cs.i "${SWIG_BUILD}/RemoteCacheSWIGImpl.cs")

if (WIN32)
  find_program(MSBUILD_BINARY MSBuild.exe HINTS "[HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\MSBuild\\ToolsVersions\\4.0;MSBuildToolsPath]")
  if (NOT MSBUILD_BINARY)
    message (FATAL_ERROR "Failed to find MSBuild.exe")
  endif (NOT MSBUILD_BINARY)

  include_external_msproject(vs-hotrodcs-bin hotrodcs.csproj
	                       TYPE AA8E36D7-FD51-4D63-A68E-227F5C4871C6
			       GUID AA8E36D7-FD51-4D63-A68E-227F5C4871C6
			       swig )
  add_custom_target(hotrodcs-bin DEPENDS swig vs-hotrodcs-bin)
  include_external_msproject(vs-hotrodcs-test-bin hotrodcs-tests.csproj
				TYPE 97560E9D-C7BA-40D6-9728-B39B1333EF49
				GUID 97560E9D-C7BA-40D6-9728-B39B1333EF49
				PLATFORM ${PLATFORM}
				vs-hotrodcs-bin)
  add_custom_target(hotrodcs-test-bin DEPENDS hotrodcs-bin vs-hotrodcs-test-bin hotrod-java-events-jar)
else (WIN32)
  find_program(XBUILD_BINARY xbuild)
  if (NOT XBUILD_BINARY)
    message (FATAL_ERROR "Please make sure xbuild is on the PATH.")
  endif (NOT XBUILD_BINARY)

  find_program(MONO_BINARY mono)
  if (NOT MONO_BINARY)
    message (FATAL_ERROR "Please make sure mono is on the PATH.")
  endif (NOT MONO_BINARY)

  add_custom_target(hotrodcs-bin ALL
    COMMAND ${XBUILD_BINARY} "/tv:4.5" "hotrodcs.proj"
    DEPENDS hotrodcs_wrap)
  
  add_custom_target(hotrodcs-tests-bin ALL
    COMMAND ${XBUILD_BINARY} "/tv:4.5" "hotrodcs-tests.csproj"
    DEPENDS hotrodcs-bin)
endif (WIN32)


### Run the tests. ###
include (CTest)

if (NOT DEFINED HOTROD_JBOSS_HOME)
  if (NOT DEFINED ENV{JBOSS_HOME})
    message (WARNING "You must set the JBOSS_HOME environment variable or use -DHOTROD_JBOSS_HOME=/the/path to run integration tests")
  endif (NOT DEFINED ENV{JBOSS_HOME})
  set (HOTROD_JBOSS_HOME $ENV{JBOSS_HOME})
endif (NOT DEFINED HOTROD_JBOSS_HOME)

if (PLATFORM STREQUAL "x64") 
  find_program(NUNIT_BINARY nunit-console.exe)
  if (NOT NUNIT_BINARY)
    message (FATAL_ERROR "Please make sure nunit-console.exe is on the PATH.")
  endif (NOT NUNIT_BINARY)
else (PLATFORM STREQUAL "x64") 
  find_program(NUNIT_BINARY nunit-console-x86.exe)
  if (NOT NUNIT_BINARY)
    message (FATAL_ERROR "Please make sure nunit-console-x86.exe is on the PATH.")
  endif (NOT NUNIT_BINARY)
endif (PLATFORM STREQUAL "x64") 

if (WIN32)
  # Add the native libs to the path.
  file (TO_NATIVE_PATH "${NATIVE_LIB_DIR}" native_dir)
  set (UT_PATH "${native_dir};$ENV{PATH}")

  # Add the wrapper libs to the path.
  foreach (loop_var ${CMAKE_CONFIGURATION_TYPES})
    file (TO_NATIVE_PATH "${SWIG_BUILD}/${loop_var}" native_dir)
    set (UT_PATH "${native_dir};${UT_PATH}")
  endforeach (loop_var)
endif (WIN32)

if (NOT DEFINED ENABLE_JAVA_TESTING OR ENABLE_JAVA_TESTING)
  # Add target for building the java sources.
  find_program(MAVEN_BINARY mvn mvn.bat)
  if (NOT MAVEN_BINARY)
    message (FATAL_ERROR "Failed to find Maven.")
  endif (NOT MAVEN_BINARY)

  if (NOT IKVM_CUSTOM_BIN_PATH)
    find_program(IKVM_BINARY ikvm ikvm.exe)
    if (NOT IKVM_BINARY)
      message (FATAL_ERROR "Failed to find IKVM.")
    endif (NOT IKVM_BINARY)
  
    find_program(IKVMSTUB_BINARY ikvmstub ikvmstub.exe)
    if (NOT IKVMSTUB_BINARY)
      message (FATAL_ERROR "Failed to find IKVMSTUB.")
    endif (NOT IKVMSTUB_BINARY)
  else (NOT IKVM_CUSTOM_BIN_PATH)
	  set (IKVM_BINARY ${IKVM_CUSTOM_BIN_PATH}/ikvm.exe) 
	  set (IKVMSTUB_BINARY ${IKVM_CUSTOM_BIN_PATH}/ikvmstub.exe) 
  endif (NOT IKVM_CUSTOM_BIN_PATH)

  set(java_tests_dir ${CMAKE_CURRENT_BINARY_DIR}/java_tests)
  set(java_events_dir ${CMAKE_CURRENT_BINARY_DIR}/java_events)
  file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/java_tests DESTINATION "${CMAKE_CURRENT_BINARY_DIR}")
  file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/java_events DESTINATION "${CMAKE_CURRENT_BINARY_DIR}")

  # Pass the updated path using an external script.
  file (TO_NATIVE_PATH "${IKVM_BINARY}" IKVM_NATIVE_BINARY)
  configure_file(templates/run_ikvm.bat.in ${java_tests_dir}/run_ikvm.bat)
  configure_file(templates/run_ispn.bat.in ${CMAKE_CURRENT_BINARY_DIR}/run_ispn.bat)

  add_custom_target (hotrod-ikvm-jar ALL
    COMMAND ${IKVMSTUB_BINARY}
    -out:${java_tests_dir}/hotrodcs.jar
    -nostdlib
    "-lib:%windir%/microsoft.net/framework/v4.0.30319"
    "${CMAKE_CURRENT_BINARY_DIR}/hotrodcs.dll"

    COMMAND ${IKVMSTUB_BINARY}
    -out:${java_tests_dir}/hotrodcs-tests.jar
    -nostdlib
    "-lib:%windir%/microsoft.net/framework/v4.0.30319"
    "-lib:${CMAKE_CURRENT_BINARY_DIR}/"
    "${CMAKE_CURRENT_BINARY_DIR}/bin/${PLATFORM}/$<CONFIG>/hotrodcs-tests.dll"

    COMMAND ${IKVMSTUB_BINARY}
    -out:${java_tests_dir}/mscorlib.jar
    -nostdlib
    "-lib:%windir%/microsoft.net/framework/v4.0.30319"
    mscorlib

    COMMAND ${MAVEN_BINARY}
    -DhotrodVersion=${HOTROD_VERSION}
    -DhotrodIKVMStubJar=${java_tests_dir}/hotrodcs.jar
    -DhotrodTestsIKVMStubJar=${java_tests_dir}/hotrodcs-tests.jar
    -DmscorlibIKVMStubJar=${java_tests_dir}/mscorlib.jar
    "clean" "package"

    WORKING_DIRECTORY ${java_tests_dir}
    DEPENDS hotrodcs-test-bin)


  add_custom_target (hotrod-java-events-jar ALL
    COMMAND ${MAVEN_BINARY}
    "clean" "package"
    WORKING_DIRECTORY ${java_events_dir})
  add_custom_command(TARGET hotrod-java-events-jar POST_BUILD  
    COMMAND (${CMAKE_COMMAND} -E copy "${CMAKE_CURRENT_BINARY_DIR}/java_events/target/java-filters-converters.jar" "${JBOSS_HOME}/standalone/deployments"))

  add_test(NAME java_tests
    WORKING_DIRECTORY ${java_tests_dir}
    COMMAND ${java_tests_dir}/run_ikvm.bat
    -enableassertions
    -Dorg.jboss.logging.provider=jdk
    # -DVERBOSE_HOTROD_JAVA_TESTS=true
    -Djava.net.preferIPv4Stack=true
    -Xreference:${CMAKE_CURRENT_BINARY_DIR}/hotrodcs.dll
    -Xreference:${CMAKE_CURRENT_BINARY_DIR}/obj/${PLATFORM}/$<CONFIG>/hotrodcs-tests.dll
    "org.infinispan.client.hotrod.JavaClientTests")
  set_tests_properties(java_tests PROPERTIES ENVIRONMENT "HOTROD_IPSTACK=IPV4;JAVA_TESTS_DIR=${java_tests_dir}" TIMEOUT 1200)

  add_test(NAME cross_language_test
    WORKING_DIRECTORY ${java_tests_dir}
    COMMAND ${java_tests_dir}/run_ikvm.bat
    -enableassertions
    -Dorg.jboss.logging.provider=jdk
    # -DVERBOSE_HOTROD_JAVA_TESTS=true
    -Djava.net.preferIPv4Stack=true
    -Dinfinispan.client.hotrod.java=${java_tests_dir}/target/infinispan-client-hotrod
    -Dinfinispan.client.hotrod.dotnet=${java_tests_dir}/hotrodcs.jar
    -Xreference:${CMAKE_CURRENT_BINARY_DIR}/hotrodcs.dll
    -Xreference:${CMAKE_CURRENT_BINARY_DIR}/bin/${PLATFORM}/$<CONFIG>/hotrodcs-tests.dll
    "org.infinispan.client.hotrod.CrossLanguageHotRodTest")
  set_tests_properties(cross_language_test PROPERTIES ENVIRONMENT "HOTROD_IPSTACK=IPV4;JAVA_TESTS_DIR=${java_tests_dir}" TIMEOUT 1200)
  
  add_test(NAME compatibility_serializer_test
    WORKING_DIRECTORY ${java_tests_dir}
    COMMAND ${java_tests_dir}/run_ikvm.bat
    -enableassertions
    -Dorg.jboss.logging.provider=jdk
    # -DVERBOSE_HOTROD_JAVA_TESTS=true
    -Djava.net.preferIPv4Stack=true
    -Dinfinispan.client.hotrod.java=${java_tests_dir}/target/infinispan-client-hotrod
    -Dinfinispan.client.hotrod.dotnet=${java_tests_dir}/hotrodcs.jar
    -Xreference:${CMAKE_CURRENT_BINARY_DIR}/hotrodcs.dll
    -Xreference:${CMAKE_CURRENT_BINARY_DIR}/bin/${PLATFORM}/$<CONFIG>/hotrodcs-tests.dll
    "org.infinispan.client.hotrod.StringSerializerHotRodTest")
  set_tests_properties(compatibility_serializer_test PROPERTIES ENVIRONMENT "HOTROD_IPSTACK=IPV4;JAVA_TESTS_DIR=${java_tests_dir}" TIMEOUT 1200)
    
endif (NOT DEFINED ENABLE_JAVA_TESTING OR ENABLE_JAVA_TESTING)

if (NOT DEFINED ENABLE_CSHARP_TESTING OR ENABLE_CSHARP_TESTING)
  file(COPY ${JBOSS_HOME}/docs/examples/configs/standalone-compatibility-mode.xml DESTINATION "${JBOSS_HOME}/standalone/configuration")
  #use the specific clustered-indexing.xml until ISPN-6912 is resolved, then use docs/examples/configs/clustered-indexing.xml
  file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/test/resources/clustered-indexing.xml DESTINATION "${JBOSS_HOME}/standalone/configuration")
  file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/test/resources/clustered-xsite1.xml DESTINATION "${JBOSS_HOME}/standalone/configuration")
  file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/test/resources/clustered-xsite2.xml DESTINATION "${JBOSS_HOME}/standalone/configuration")
  file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/test/resources/standalone-hotrod-ssl.xml DESTINATION "${JBOSS_HOME}/standalone/configuration")
  file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/test/resources/clustered-sasl-cs.xml DESTINATION "${JBOSS_HOME}/standalone/configuration")
  file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/test/resources/certificates/keystore.jks DESTINATION "${JBOSS_HOME}/standalone/configuration")
  file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/test/resources/certificates/truststore.jks DESTINATION "${JBOSS_HOME}/standalone/configuration")
  file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/test/resources/certificates/keystore_server_sni1_rsa.jks DESTINATION "${JBOSS_HOME}/standalone/configuration")
  file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/test/resources/certificates/keystore_server_sni2_rsa.jks DESTINATION "${JBOSS_HOME}/standalone/configuration")
  file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/test/resources/certificates/keystore_server_sni3_untrusted_rsa.jks DESTINATION "${JBOSS_HOME}/standalone/configuration")
  file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/test/resources/application-roles.properties DESTINATION "${JBOSS_HOME}/standalone/configuration")
  file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/test/resources/application-users.properties DESTINATION "${JBOSS_HOME}/standalone/configuration")
  file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/test/resources/certificates/infinispan-ca.pem DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/bin/${PLATFORM}/${CONFIGURATION})
  file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/test/resources/certificates/keystore_client.p12 DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/bin/${PLATFORM}/${CONFIGURATION})
  file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/test/resources/certificates/malicious_client.p12 DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/bin/${PLATFORM}/${CONFIGURATION})
  file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/test/resources/certificates/keystore_server_sni1_rsa.pem DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/bin/${PLATFORM}/${CONFIGURATION})
  file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/test/resources/certificates/keystore_server_sni2_rsa.pem DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/bin/${PLATFORM}/${CONFIGURATION})
  file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/test/resources/certificates/malicious.pem DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/bin/${PLATFORM}/${CONFIGURATION})
  file(COPY "${CMAKE_CURRENT_SOURCE_DIR}/test/resources/proto2" DESTINATION "${CMAKE_CURRENT_BINARY_DIR}/bin/${PLATFORM}/${CONFIGURATION}" PATTERN "*")
  file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/test/resources/wordCountStream.js DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/bin/${PLATFORM}/${CONFIGURATION})
  file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/test/resources/macbeth.txt DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/bin/${PLATFORM}/${CONFIGURATION})
      
  if (WIN32)
    # Pass the updated path using an external script.
    file (TO_NATIVE_PATH "${NUNIT_BINARY}" NUNIT_NATIVE_BINARY)
    configure_file(templates/nunit.bat.in nunit.bat)

    add_test(NAME csharp_tests_default COMMAND nunit.bat bin/${PLATFORM}/$<CONFIG>/ Infinispan.HotRod.TestSuites.DefaultTestSuite TestResult-default.xml)
    add_test(NAME csharp_tests_security COMMAND nunit.bat bin/${PLATFORM}/$<CONFIG>/ Infinispan.HotRod.TestSuites.SecurityTestSuite TestResult-security.xml)
    add_test(NAME csharp_tests_authentication COMMAND nunit.bat bin/${PLATFORM}/$<CONFIG>/ Infinispan.HotRod.TestSuites.AuthenticationTestSuite TestResult-authentication.xml)
    add_test(NAME csharp_tests_compatibility COMMAND nunit.bat bin/${PLATFORM}/$<CONFIG>/ Infinispan.HotRod.TestSuites.CompatibilityModeTestSuite TestResult-compatibility.xml)
    add_test(NAME csharp_tests_query COMMAND nunit.bat bin/${PLATFORM}/$<CONFIG>/ Infinispan.HotRod.TestSuites.RemoteQueryTestSuite TestResult-query.xml)
    add_test(NAME csharp_tests_cluster COMMAND nunit.bat bin/${PLATFORM}/$<CONFIG>/ Infinispan.HotRod.TestSuites.ClusterTestSuite TestResult-cluster.xml)
    add_test(NAME csharp_tests_xsite COMMAND nunit.bat bin/${PLATFORM}/$<CONFIG>/ Infinispan.HotRod.TestSuites.XSiteTestSuite TestResult-xsite.xml)
  else (WIN32)
    add_test(csharp_unittests ${MONO_BINARY} ${NUNIT_BINARY} hotrodcs-tests.dll)
    set_tests_properties(csharp_unittests PROPERTIES ENVIRONMENT "JBOSS_HOME=${HOTROD_JBOSS_HOME}")
  endif (WIN32)
endif (NOT DEFINED ENABLE_CSHARP_TESTING OR ENABLE_CSHARP_TESTING)
  
### Packaging. ###
set (CPACK_GENERATOR "WIX")
set (CPACK_SOURCE_GENERATOR "ZIP")

# Native client.

install (FILES "${CMAKE_CURRENT_BINARY_DIR}/swig/native_client/lib/hotrod.dll" DESTINATION lib/)
install (FILES "${CMAKE_CURRENT_BINARY_DIR}/swig/native_client/lib/hotrod.lib" DESTINATION lib/)

# Native wrapper.
install (FILES "${CMAKE_CURRENT_BINARY_DIR}/swig/$<CONFIG>/hotrod_wrap.dll" DESTINATION lib/ OPTIONAL)
install (FILES "${CMAKE_CURRENT_BINARY_DIR}/swig/$<CONFIG>/hotrod_wrap.lib" DESTINATION lib/ OPTIONAL)
# Native wrapper.
# Managed wrapper.
install (FILES "${CMAKE_CURRENT_BINARY_DIR}/hotrodcs.dll" DESTINATION lib/)
# Managed wrapper API doc for IDE.
install (FILES "${CMAKE_CURRENT_BINARY_DIR}/hotrodcs.xml" DESTINATION lib/)
# Dependencies.
file(TO_CMAKE_PATH ${GOOGLE_PROTOBUF_NUPKG} CM_GOOGLE_PROTOBUF_NUPKG)
file(TO_CMAKE_PATH ${PROTOBUF_PROTOC_EXECUTABLE_CS} CM_PROTOBUF_PROTOC_EXECUTABLE_CS)
file(TO_CMAKE_PATH ${OPENSSL_ROOT_DIR} CM_OPENSSL_ROOT_DIR)
install (FILES "${NLOG_DLL_LOCAL}" DESTINATION lib/)
install (FILES "${NLOG_LICENSE_LOCAL}" DESTINATION lib/ OPTIONAL)
install (FILES "${CM_GOOGLE_PROTOBUF_NUPKG}/Google.Protobuf.3.0.0-beta2/lib/portable-net45+netcore45+wpa81+wp8/Google.Protobuf.dll" DESTINATION lib/)
install (FILES "${CM_PROTOBUF_PROTOC_EXECUTABLE_CS}" DESTINATION bin/)
install (DIRECTORY "${CM_OPENSSL_ROOT_DIR}/lib" "${CM_OPENSSL_ROOT_DIR}/bin" "${CM_OPENSSL_ROOT_DIR}/include" DESTINATION .)

configure_file("${CMAKE_CURRENT_SOURCE_DIR}/templates/README.txt.in" README.txt NEWLINE_STYLE CRLF)
configure_file("${CMAKE_CURRENT_SOURCE_DIR}/License.txt" License.txt NEWLINE_STYLE CRLF)
file (COPY "${CMAKE_CURRENT_SOURCE_DIR}/License.rtf" DESTINATION "${CMAKE_CURRENT_BINARY_DIR}")

install (FILES
  "${CMAKE_CURRENT_BINARY_DIR}/License.txt"
  "${CMAKE_CURRENT_BINARY_DIR}/README.txt"
  DESTINATION .)

configure_file("${CMAKE_CURRENT_SOURCE_DIR}/templates/wix.xml.in" wix.xml)
set (CPACK_WIX_TEMPLATE "wix.xml")
set (CPACK_WIX_UPGRADE_GUID "7cc17657-80b5-417f-9209-710682f852f4") # This value should not change.
set (CPACK_SOURCE_IGNORE_FILES "/cpp-client\\\\.*/;/build.*/;/\\\\.git/")

#Bundle-specific properties (see wix-bundle.cmake).
if (DEFINED HOTROD_BUILD_BUNDLE OR DEFINED ENV{HOTROD_BUILD_BUNDLE})
  if (NOT DEFINED HOTROD_VCREDIST_x86)
    if (DEFINED ENV{HOTROD_VCREDIST_x86})
      set (HOTROD_VCREDIST_x86 "$ENV{HOTROD_VCREDIST_x86}")
    else ()
      message (FATAL_ERROR "When building a bundle you need to point HOTROD_VCREDIST_x86 to the corresponding VC++ x86 runtime to be included in the bundle.")
    endif()
  endif ()

  if (NOT DEFINED HOTROD_VCREDIST_x64)
    if (DEFINED ENV{HOTROD_VCREDIST_x64})
      set (HOTROD_VCREDIST_x64 "$ENV{HOTROD_VCREDIST_x64}")
    else ()
      message (FATAL_ERROR "When building a bundle you need to point HOTROD_VCREDIST_x64 to the corresponding VC++ x64 runtime to be included in the bundle.")
    endif ()
  endif ()

  if (NOT DEFINED HOTROD_DOTNET)
    if (DEFINED ENV{HOTROD_DOTNET})
      set (HOTROD_DOTNET "$ENV{HOTROD_DOTNET}")
    else ()
      message (FATAL_ERROR "When building a bundle you need to point HOTROD_DOTNET to the corresponding .NET runtime to be included in the bundle.")
    endif ()
  endif ()

  configure_file("${CMAKE_CURRENT_SOURCE_DIR}/templates/wix-bundle.xml.in" wix-bundle.xml)
endif()

include (CPack)

### Documentation ###
find_package(Doxygen)
if (DOXYGEN_FOUND)
  #-- Configure the Template Doxyfile for our specific project
  configure_file(templates/Doxyfile.in ${PROJECT_BINARY_DIR}/Doxyfile  @ONLY IMMEDIATE)
  # Delete any previously generated docs
  if(EXISTS ${CMAKE_BINARY_DIR}/api_docs)
      file(REMOVE_RECURSE ${CMAKE_BINARY_DIR}/api_docs)
  endif(EXISTS ${CMAKE_BINARY_DIR}/api_docs)
  #-- Add a custom target to run Doxygen when ever the project is built
  add_custom_target (docs ALL
                     COMMAND ${DOXYGEN_EXECUTABLE} ${PROJECT_BINARY_DIR}/Doxyfile
                     SOURCES ${PROJECT_BINARY_DIR}/Doxyfile)
  #Include the API docs in the package.
  install (FILES ${CMAKE_BINARY_DIR}/api_docs/html/ DESTINATION docs/api)
endif (DOXYGEN_FOUND)
