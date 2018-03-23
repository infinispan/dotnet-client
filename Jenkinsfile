#!/usr/bin/env groovy

pipeline {
    agent {
        label 'windows'
    }
    
    stages {
        stage('Build C++ core') {
            environment {
                cppTag = '8.1.0.Final'
                CMAKE_HOME = 'C:\\\\PROGRA~2\\\\CMake\\\\bin'
                generator = '"Visual Studio 14 2015 Win64"'
                INFINISPAN_VERSION = '9.0.0.Final'
                JAVA_HOME = 'C:\\\\PROGRA~1\\\\JAVA\\\\JDK18~1.0_7'
                M2_HOME = 'C:\\\\APACHE~1.9'
                MVN_PROGRAM = 'C:\\\\APACHE~1.9\\\\BIN\\\\MVN.BAT'
                PROTOBUF_INCLUDE_DIR = 'C:\\\\protobuf-2.6.1-pack\\\\include'
                PROTOBUF_LIBRARY = 'C:\\\\protobuf-2.6.1-pack\\\\lib\\\\libprotobuf-static.lib'
                PROTOBUF_PROTOC_EXECUTABLE = 'C:\\\\protobuf-2.6.1-pack\\\\bin\\\\protoc.exe'
                PROTOBUF_PROTOC_LIBRARY = 'C:\\\\protobuf-2.6.1-pack\\\\lib\\\\libprotoc.lib'
                SWIG_DIR = 'C:\\\\PROGRA~1\\\\SWIGWI~1.12'
                SWIG_EXECUTABLE = 'C:\\\\PROGRA~1\\\\SWIGWI~1.12\\\\SWIG.EXE'
                test32 = 'empty'
                test64 = 'empty'
                version_1major = '8'
                version_2minor = '1'
                version_3micro = '0'
                version_4qualifier = 'Final'
                HOTROD_LOG_LEVEL = 'TRACE'
            }
            steps {
                dir('cpp-client') {
                    checkout scm: [$class: 'GitSCM',
                       userRemoteConfigs: [[url: 'https://github.com/infinispan/cpp-client.git']],
                       branches: [[name: 'master']]], changelog: false, poll: false
                }
                script {
                    dir ('cpp-client') {
                        bat ".\\build.bat"
                    }
                }
            }
        }

        stage('SCM Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Build') {
            environment {
                checkoutDir  = 'Y:'
                CMAKE_HOME  = 'C:\\\\PROGRA~2\\\\CMake\\\\bin'
                generator  = '"Visual Studio 14 2015 Win64"'
                GOOGLE_PROTOBUF_NUPKG  = 'C:\\\\Users\\\\Administrator'
                HOTROD_SNK  = 'c:\\\\data\\\\hotrod_cs.key'
                IKVM_CUSTOM_BIN_PATH  = 'C:\\\\Users\\\\Administrator\\\\ikvm-8.1.5717.0\\\\bin'
                INFINISPAN_VERSION  = '9.0.0.Final'
                JBOSS_HOME  = 'Y:\\\\cpp-client\\\\infinispan-server-9.0.0.Final'
                JAVA_HOME  = 'C:\\\\PROGRA~1\\\\JAVA\\\\JDK18~1.0_7'
                M2_HOME  = 'C:\\\\APACHE~1.9'
                MAVEN_OPTS  = '"-Dmaven.multiModuleProjectDirectory=C:\\\\APACHE~1.9"'
                MVN_PROGRAM  = 'C:\\\\APACHE~1.9\\\\BIN\\\\MVN'
                NLOG_DLL  = 'c:\\\\data\\\\NLog-2.1.0\\\\net40\\\\NLog.dll'
                NLOG_LICENSE  = 'c:\\\\data\\\\NLog_License.txt'
                NUNIT_DLL  = 'C:\\\\Users\\\\Administrator\\\\NUnit.3.8.0\\\\lib\\\\net45\\\\nunit.framework.dll'
                OPENSSL_ROOT_DIR  = 'c:/OpenSSL-Win64'
                PROTOBUF_INCLUDE_DIR  = 'C:\\\\protobuf-2.6.1-pack\\\\include'
                PROTOBUF_LIBRARY  = 'C:\\\\protobuf-2.6.1-pack\\\\lib\\\\libprotobuf-static.lib'
                PROTOBUF_PROTOC_EXECUTABLE  = 'C:\\\\protobuf-2.6.1-pack\\\\bin\\\\protoc.exe'
                PROTOBUF_PROTOC_EXECUTABLE_CS  = 'C:\\\\Users\\\\Administrator\\\\Google.Protobuf.Tools.3.4.0\\\\tools\\\\windows_x64\\\\protoc.exe'
                PROTOBUF_PROTOC_LIBRARY  = 'C:\\\\protobuf-2.6.1-pack\\\\lib\\\\libprotoc.lib'
                SWIG_DIR  = 'C:\\\\PROGRA~1\\\\SWIGWI~1.12'
                SWIG_EXECUTABLE  = 'C:\\\\PROGRA~1\\\\SWIGWI~2.12\\\\SWIG.EXE'
                test32  = 'skip'
                test64  = 'run'
                version_1major  = '8'
                version_2minor  = '1'
                version_3micro  = '0'
                version_4qualifier  = 'Final'
                cppTag  = '8.1.0.Final'
            }
            steps {
                script {
                    bat ".\\build.bat"
                    archiveArtifacts artifacts: 'build_windows\\_CPack_Packages\\win64\\WIX\\*.msi, build_windows\\_CPack_Packages\\win64-Source\\ZIP\\*.zip'
                }
            }
        }
    }
}
