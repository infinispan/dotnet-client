cmake_minimum_required(VERSION 2.8)
project (infinispan-hotrod-cs-swig)

find_package(SWIG REQUIRED)
include(UseSWIG)

if (NOT DEFINED ARCH)
  message (FATAL_ERROR "Please define -DARCH.")
endif (NOT DEFINED ARCH)

if (NOT DEFINED HOTRODCPP_HOME)
  message (FATAL_ERROR "Please define -DHOTRODCPP_HOME.")
endif (NOT DEFINED HOTRODCPP_HOME)
file(TO_CMAKE_PATH "${HOTRODCPP_HOME}" HOTRODCPP_HOME)

if (ARCH EQUAL "64") 
  set (HOTRODCPP_LIB_SUFFIX "")
else (ARCH EQUAL "64")
  set (HOTRODCPP_LIB_SUFFIX "32")
endif (ARCH EQUAL "64")

set (HOTROD_SWIG_MODULE "hotrod_wrap${ARCH}")
set (HOTROD_SWIG_MODULE_SOURCE hotrodcs.i)
set_source_files_properties(${HOTROD_SWIG_MODULE_SOURCE} PROPERTIES CPLUSPLUS ON)

set (HOTRODCPP_LIB_DIR "${HOTRODCPP_HOME}/lib")
set (HOTRODCPP_INCLUDE_DIR "${HOTRODCPP_HOME}/include")
if (WIN32)
  set (HOTRODCPP_LIB "${HOTRODCPP_LIB_DIR}/hotrod${HOTRODCPP_LIB_SUFFIX}.lib") 
  set (HOTROD_WRAP_LIB "${HOTROD_SWIG_MODULE}.dll")
else (WIN32)
  set (HOTRODCPP_LIB "${HOTRODCPP_LIB_DIR}/libhotrod${HOTRODCPP_LIB_SUFFIX}.so") 
  set (HOTROD_WRAP_LIB "${HOTROD_SWIG_MODULE}.so")
endif (WIN32)

if (CMAKE_MAJOR_VERSION LESS 3)
  set_source_files_properties(${HOTROD_SWIG_MODULE_SOURCE} PROPERTIES SWIG_FLAGS "-v;-namespace;Infinispan.HotRod.SWIG${ARCH};-dllimport;${HOTROD_WRAP_LIB}")
else (CMAKE_MAJOR_VERSION LESS 3)
  set_source_files_properties(${HOTROD_SWIG_MODULE_SOURCE} PROPERTIES SWIG_FLAGS "-v;-namespace;Infinispan.HotRod.SWIG${ARCH};")
endif (CMAKE_MAJOR_VERSION LESS 3)

include_directories(${HOTRODCPP_INCLUDE_DIR})
swig_add_module(${HOTROD_SWIG_MODULE} csharp ${HOTROD_SWIG_MODULE_SOURCE})
swig_link_libraries(${HOTROD_SWIG_MODULE} ${HOTRODCPP_LIB})
