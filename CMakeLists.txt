cmake_minimum_required(VERSION 2.8)

project(infinispan-hotrod-dotnet)

if (NOT DEFINED HOTROD_VERSION_MAJOR)
  set (HOTROD_VERSION_MAJOR "7")
endif (NOT DEFINED HOTROD_VERSION_MAJOR)
if (NOT DEFINED HOTROD_VERSION_MINOR)
  set (HOTROD_VERSION_MINOR "0")
endif (NOT DEFINED HOTROD_VERSION_MINOR)
if (NOT DEFINED HOTROD_VERSION_PATCH)
  set (HOTROD_VERSION_PATCH "0")
endif (NOT DEFINED HOTROD_VERSION_PATCH)
if (NOT DEFINED HOTROD_VERSION_LABEL)
  set (HOTROD_VERSION_LABEL "SNAPSHOT")
endif (NOT DEFINED HOTROD_VERSION_LABEL)

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
set (REVISION_Beta1 "10")
set (REVISION_CR1 "20")
set (REVISION_CR2 "21")
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

### Generated and build the SWIG wrapper ###
if (NOT DEFINED HOTRODCPP32_HOME AND DEFINED ENV{HOTRODCPP32_HOME})
  set (HOTRODCPP32_HOME $ENV{HOTRODCPP32_HOME})
endif (NOT DEFINED HOTRODCPP32_HOME AND DEFINED ENV{HOTRODCPP32_HOME})
if (NOT DEFINED HOTRODCPP64_HOME AND DEFINED ENV{HOTRODCPP64_HOME})
  set (HOTRODCPP64_HOME $ENV{HOTRODCPP64_HOME})
endif (NOT DEFINED HOTRODCPP64_HOME AND DEFINED ENV{HOTRODCPP64_HOME})
if (NOT DEFINED HOTRODCPP32_HOME)
  message (FATAL_ERROR "Please define HOTRODCPP32_HOME.")
endif (NOT DEFINED HOTRODCPP32_HOME)
if (NOT DEFINED HOTRODCPP64_HOME)
  message (FATAL_ERROR "Please define HOTRODCPP64_HOME.")
endif (NOT DEFINED HOTRODCPP64_HOME)

set (SWIG32_BUILD "${CMAKE_CURRENT_BINARY_DIR}/swig32")
set (SWIG64_BUILD "${CMAKE_CURRENT_BINARY_DIR}/swig64")
set (NATIVE32_DIR "${SWIG32_BUILD}/native_client")
set (NATIVE64_DIR "${SWIG64_BUILD}/native_client")
set (NATIVE32_LIB_DIR "${NATIVE32_DIR}/lib")
set (NATIVE64_LIB_DIR "${NATIVE64_DIR}/lib")

file (MAKE_DIRECTORY "${SWIG32_BUILD}" "${SWIG64_BUILD}" "${NATIVE32_DIR}" "${NATIVE64_DIR}")
file (COPY "${HOTRODCPP32_HOME}/lib/" DESTINATION "${NATIVE32_LIB_DIR}" PATTERN "*")
file (COPY "${HOTRODCPP64_HOME}/lib/" DESTINATION "${NATIVE64_LIB_DIR}" PATTERN "*")
file (COPY "${HOTRODCPP32_HOME}/include/" DESTINATION "${NATIVE32_DIR}/include" PATTERN "*")
file (COPY "${HOTRODCPP64_HOME}/include/" DESTINATION "${NATIVE64_DIR}/include" PATTERN "*")

if (WIN32)
  string(REPLACE " Win64" "" GENERATOR "${CMAKE_GENERATOR}")
  add_custom_target(
    swig32 ALL
    COMMAND cmake -G "${GENERATOR}" "-DARCH=32" "-DHOTRODCPP_HOME=${NATIVE32_DIR}" "${CMAKE_CURRENT_SOURCE_DIR}/swig"
    COMMAND cmake --build . --config "${CMAKE_CFG_INTDIR}"
    WORKING_DIRECTORY "${SWIG32_BUILD}"
    )
  add_custom_target(
    swig64 ALL
    COMMAND cmake -G "${GENERATOR} Win64" "-DARCH=64" "-DHOTRODCPP_HOME=${NATIVE64_DIR}" "${CMAKE_CURRENT_SOURCE_DIR}/swig"
    COMMAND cmake --build . --config "${CMAKE_CFG_INTDIR}"
    WORKING_DIRECTORY "${SWIG64_BUILD}"
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
string(REPLACE "/" "\\" W_SWIG32_BUILD "${SWIG32_BUILD}")
string(REPLACE "/" "\\" W_SWIG64_BUILD "${SWIG64_BUILD}")

configure_file(templates/hotrodcs.proj.in hotrodcs.csproj)
configure_file(templates/hotrodcs-tests.proj.in hotrodcs-tests.csproj)
configure_file(templates/AssemblyInfo.cs.in AssemblyInfo.cs)

set (ARCH 32)
configure_file(src/main/cs/Infinispan/HotRod/Impl/RemoteCacheSWIGImpl.cs.i "${SWIG32_BUILD}/RemoteCacheSWIG32Impl.cs")
set (ARCH 64)
configure_file(src/main/cs/Infinispan/HotRod/Impl/RemoteCacheSWIGImpl.cs.i "${SWIG64_BUILD}/RemoteCacheSWIG64Impl.cs")

if (WIN32)
  find_program(MSBUILD_BINARY MSBuild.exe HINTS "[HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\MSBuild\\ToolsVersions\\4.0;MSBuildToolsPath]")
  if (NOT MSBUILD_BINARY)
    message (FATAL_ERROR "Failed to find MSBuild.exe")
  endif (NOT MSBUILD_BINARY)

  include_external_msproject(vs-hotrodcs-bin hotrodcs.csproj
	                       TYPE AA8E36D7-FD51-4D63-A68E-227F5C4871C6
			       GUID AA8E36D7-FD51-4D63-A68E-227F5C4871C6
			       swig32 )
  add_custom_target(hotrodcs-bin DEPENDS swig32 swig64 vs-hotrodcs-bin)
  if (ISWIN64)
	  include_external_msproject(vs-hotrodcs-test-bin hotrodcs-tests.csproj
					TYPE 97560E9D-C7BA-40D6-9728-B39B1333EF49
				GUID 97560E9D-C7BA-40D6-9728-B39B1333EF49
				PLATFORM x64
				vs-hotrodcs-bin)
  else (ISWIN64)
	  include_external_msproject(vs-hotrodcs-test-bin hotrodcs-tests.csproj
		                TYPE 97560E9D-C7BA-40D6-9728-B39B1333EF49
				GUID 97560E9D-C7BA-40D6-9728-B39B1333EF49
				PLATFORM x86
				vs-hotrodcs-bin)
  endif (ISWIN64)
  add_custom_target(hotrodcs-test-bin DEPENDS hotrodcs-bin vs-hotrodcs-test-bin)
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

find_program(NUNIT_BINARY nunit-console.exe)
if (NOT NUNIT_BINARY)
  message (FATAL_ERROR "Please make sure nunit-console.exe is on the PATH.")
endif (NOT NUNIT_BINARY)

find_program(NUNIT_BINARY_X86 nunit-console-x86.exe)
if (NOT NUNIT_BINARY_X86)
  message (FATAL_ERROR "Please make sure nunit-console-x86.exe is on the PATH.")
endif (NOT NUNIT_BINARY_X86)

if (WIN32)
  # Add the native libs to the path.
  file (TO_NATIVE_PATH "${NATIVE32_LIB_DIR}" native_dir32)
  file (TO_NATIVE_PATH "${NATIVE64_LIB_DIR}" native_dir64)
  set (UT_PATH "${native_dir32};${native_dir64};$ENV{PATH}")

  # Add the wrapper libs to the path.
  foreach (loop_var ${CMAKE_CONFIGURATION_TYPES})
    file (TO_NATIVE_PATH "${SWIG32_BUILD}/${loop_var}" native_dir32)
    file (TO_NATIVE_PATH "${SWIG64_BUILD}/${loop_var}" native_dir64)
    set (UT_PATH "${native_dir32};${native_dir64};${UT_PATH}")
  endforeach (loop_var)
endif (WIN32)

if (NOT DEFINED ENABLE_JAVA_TESTING OR ENABLE_JAVA_TESTING)
  # Add target for building the java sources.
  find_program(MAVEN_BINARY mvn mvn.bat)
  if (NOT MAVEN_BINARY)
    message (FATAL_ERROR "Failed to find Maven.")
  endif (NOT MAVEN_BINARY)

  find_program(IKVM_BINARY ikvm ikvm.exe)
  if (NOT IKVM_BINARY)
    message (FATAL_ERROR "Failed to find IKVM.")
  endif (NOT IKVM_BINARY)

  find_program(IKVMSTUB_BINARY ikvmstub ikvmstub.exe)
  if (NOT IKVMSTUB_BINARY)
    message (FATAL_ERROR "Failed to find IKVMSTUB.")
  endif (NOT IKVMSTUB_BINARY)

  set(java_tests_dir ${CMAKE_CURRENT_BINARY_DIR}/java_tests)
  file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/java_tests DESTINATION "${CMAKE_CURRENT_BINARY_DIR}")

  # Pass the updated path using an external script.
  file (TO_NATIVE_PATH "${IKVM_BINARY}" IKVM_NATIVE_BINARY)
  configure_file(templates/run_ikvm.bat.in ${java_tests_dir}/run_ikvm.bat)

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

endif (NOT DEFINED ENABLE_JAVA_TESTING OR ENABLE_JAVA_TESTING)

if (NOT DEFINED ENABLE_CSHARP_TESTING OR ENABLE_CSHARP_TESTING)
  if (WIN32)
    # Pass the updated path using an external script.
    file (TO_NATIVE_PATH "${NUNIT_BINARY}" NUNIT_NATIVE_BINARY)
    configure_file(templates/nunit.bat.in nunit64.bat)
    file (TO_NATIVE_PATH "${NUNIT_BINARY_X86}" NUNIT_NATIVE_BINARY)
    configure_file(templates/nunit.bat.in nunit32.bat)

    if (ISWIN64)
      add_test(NAME csharp_unittests COMMAND nunit64.bat bin/x64/$<CONFIG>/)
    else (ISWIN64)
      add_test(NAME csharp_unittests COMMAND nunit32.bat bin/x86/$<CONFIG>/)
    endif (ISWIN64)
  else (WIN32)
    add_test(csharp_unittests ${MONO_BINARY} ${NUNIT_BINARY} hotrodcs-tests.dll)
    set_tests_properties(csharp_unittests PROPERTIES ENVIRONMENT "JBOSS_HOME=${HOTROD_JBOSS_HOME}")
  endif (WIN32)
endif (NOT DEFINED ENABLE_CSHARP_TESTING OR ENABLE_CSHARP_TESTING)

### Packaging. ###
set (CPACK_GENERATOR "WIX")
set (CPACK_SOURCE_GENERATOR "ZIP")

# Native client.
if (ISWIN64)
  set (native_lib_suffix "")
  set (arch "64")
else (ISWIN64)
  set (native_lib_suffix "32")
  set (arch "32")
endif (ISWIN64)

install (FILES "${CMAKE_CURRENT_BINARY_DIR}/swig${arch}/native_client/lib/hotrod${native_lib_suffix}.dll" DESTINATION lib/)
install (FILES "${CMAKE_CURRENT_BINARY_DIR}/swig${arch}/native_client/lib/hotrod${native_lib_suffix}.lib" DESTINATION lib/)
# Native wrapper.
install (FILES "${CMAKE_CURRENT_BINARY_DIR}/swig${arch}/$<CONFIG>/hotrod_wrap${arch}.dll" DESTINATION lib/ OPTIONAL)
install (FILES "${CMAKE_CURRENT_BINARY_DIR}/swig${arch}/$<CONFIG>/hotrod_wrap${arch}.lib" DESTINATION lib/ OPTIONAL)
# Managed wrapper.
install (FILES "${CMAKE_CURRENT_BINARY_DIR}/hotrodcs.dll" DESTINATION lib/)
# Managed wrapper API doc for IDE.
install (FILES "${CMAKE_CURRENT_BINARY_DIR}/hotrodcs.xml" DESTINATION lib/)
# Dependencies.
install (FILES "${NLOG_DLL_LOCAL}" DESTINATION lib/)
install (FILES "${NLOG_LICENSE_LOCAL}" DESTINATION lib/ OPTIONAL)

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
set (CPACK_SOURCE_IGNORE_FILES "/build.*/;/\\\\.git/")

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
