﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Infinispan.HotRod.Impl;
using Infinispan.HotRod.Config;
using Infinispan.HotRod;
using System.Diagnostics;
using System.IO;

namespace Infinispan.HotRod.Tests.Util
{
    public class HotRodServer
    {
        public const string hostname = "127.0.0.1";
        public int port;
        string configurationFile;
        private Process hrServer;
        private string arguments;
        private string serverHome;

        public HotRodServer(string configurationFile, string arguments = "", string serverHome="server",  int port = 11222)
        {
            this.configurationFile = configurationFile;
            this.arguments = arguments;
            this.port = port;
            this.serverHome = serverHome;
        }

        public void StartHotRodServer()
        {
            try
            {
                Console.WriteLine("Starting Infinispan Server ...");
                StartHotrodServerInternal();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                ShutDownHotrodServer();
                throw ex;
            }
        }

        public bool IsStopped()
        {
            return PortProbe.IsPortClosed(hostname, port, millisTimeout: 10000);
        }

        public bool IsRunning(int timeout = 60000)
        {
            return PortProbe.IsPortOpen(hostname, port, millisTimeout: timeout);
        }

        void StartHotrodServerInternal()
        {
            var jbossHome = Environment.GetEnvironmentVariable("JBOSS_HOME");
            var path = Environment.GetEnvironmentVariable("PATH");
            Console.WriteLine("PATH= "+path);
            if (jbossHome == null)
            {
                throw new Exception("JBOSS_HOME env variable not set.");
            }

            Assert.IsTrue(IsStopped(),
                          "Another process already listening on the same ip/port.");
            // Cleanup data dir
            if (PlatformUtils.isUnix())
            {
                Directory.Delete(Path.Combine(jbossHome, serverHome+"/data"), true);
            }
            else
            {
                Directory.Delete(Path.Combine(jbossHome, serverHome+"\\data"), true);
            }
            hrServer = new Process
            {
                StartInfo =
                {
                    FileName = buildStartCommand(jbossHome),
                    Arguments = "-c " + configurationFile,
                    UseShellExecute = false
                }
            };
            if (arguments.Length != 0)
            {
                hrServer.StartInfo.Arguments = hrServer.StartInfo.Arguments + " " + arguments;
            }
            hrServer.StartInfo.UseShellExecute = false;
            hrServer.StartInfo.EnvironmentVariables["NOPAUSE"] = "YES";
            if (PlatformUtils.isUnix())
            {
                // Drop the output generated by the server on the console (data present in log file).
                hrServer.StartInfo.RedirectStandardOutput = true;
                hrServer.StartInfo.RedirectStandardError = true;
                hrServer.OutputDataReceived += new DataReceivedEventHandler(DropOutputHandler);
                hrServer.ErrorDataReceived += new DataReceivedEventHandler(DropOutputHandler);
            }
            hrServer.Start();


            Assert.IsTrue(IsRunning(),
                          "Server not listening on the expected ip/port.");
        }

        public void ShutDownHotrodServer()
        {
            Console.WriteLine("Shutting down Infinispan Server ...");
            if (hrServer != null)
            {
                if (PlatformUtils.isUnix()) {
                /* Kill the process and subprocesses. */
                    Process killProcess = new Process();
                    killProcess.StartInfo.FileName = "bash";
                    killProcess.StartInfo.Arguments = String.Format("-c \"ps h --format pid --pid {0} --ppid {0} | xargs kill\"", hrServer.Id);
                    killProcess.Start();
                    killProcess.WaitForExit();
                }
                else
                {
                /* Kill the process and subprocesses. */
                    Process killProcess = new Process();
                    killProcess.StartInfo.FileName = "taskkill";
                    killProcess.StartInfo.Arguments = String.Format("/PID {0} /T /F", hrServer.Id);
                    killProcess.Start();
                    killProcess.WaitForExit();
                }
                Assert.IsTrue(IsStopped(),
                              "A process is still listening on the ip/port. Kill failed?");
                hrServer.Close();
            }
        }

        private string buildStartCommand(string homePath)
        {
            if (PlatformUtils.isUnix())
            {
                return Path.Combine(homePath, "bin/server.sh");
            }
            else
            {
                return Path.Combine(homePath, "bin\\server.bat");
            }
        }

        private static void DropOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            // Do nothing. Drop the data.
        }
    }
}
