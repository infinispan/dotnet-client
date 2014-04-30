using System;
using System.Diagnostics;
using System.Management;

namespace Infinispan.HotRod.Tests
{
    public class PlatformUtils
    {
        /**
           Returns true if the runtime platform is Unix.
        */
        public static bool isUnix()
        {
            int p = (int) Environment.OSVersion.Platform;
            return (p == 4) || (p == 6) || (p == 128);
        }
        
        /**
           On Unix Process.Kill will not kill the forked jvm process. Due to the use of `eval' in standalone.sh
           it will only kill the shell script. This method kills the first level subprocesses as well.
        */
        public static void killServer(Process process)
        {
            if (isUnix()) {
                /* Kill the process and subprocesses. */
                Process killProcess = new Process();
                killProcess.StartInfo.FileName = "bash";
                killProcess.StartInfo.Arguments = String.Format("-c \"ps h --format pid --pid {0} --ppid {0} | xargs kill\"", process.Id);
                killProcess.Start();
                killProcess.WaitForExit();
            } else {
                if (!killJavaSubprocesses(process.Id) && !killAllStandaloneServerProcesses()) {
                    Console.WriteLine("Could not find any server subprocess.");
                }
            }
            kill(process);
        }

        private static void kill(Process process)
        {
            process.CloseMainWindow();
            if (!process.HasExited) {
                process.Kill();        
            }
            process.WaitForExit();
            process.Close();
        }

        private static bool killJavaSubprocesses(int pid)
        {
            bool found = false;

            foreach (var process in Process.GetProcesses()) {
                int ppid = (int) (new PerformanceCounter("Process", "Creating Process ID", process.ProcessName)).RawValue;
                if (ppid == pid) {
                    if ("java".Equals(process.ProcessName)) {
                        process.CloseMainWindow();
                        if (!process.HasExited) {
                            process.Kill();        
                        }
                        process.WaitForExit();
                        process.Close();
                        found = true;
                    } else {
                        killJavaSubprocesses(process.Id);
                    }
                }
            }

            return found;
        }

        private static bool killAllStandaloneServerProcesses()
        {
            bool found = false;

            // Look for all processes which look like standalone server instances and kill them.
            ManagementObjectCollection objs = (new ManagementObjectSearcher("select CommandLine,ProcessId from Win32_Process")).Get();
            foreach (ManagementObject obj in objs) {
                String cmdline = (String) obj["CommandLine"];
                if ((cmdline != null) && cmdline.Contains("org.jboss.as.standalone")) {
                    UInt32 pid = (UInt32) obj["ProcessId"];
                    kill(Process.GetProcessById((int) pid));
                    found = true;
                }
            }

            return found;
        }
    }
}