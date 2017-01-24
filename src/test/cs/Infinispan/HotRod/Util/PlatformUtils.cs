using System;
using System.Diagnostics;
using System.Management;

namespace Infinispan.HotRod.Tests.Util
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
                killJavaSubprocesses(process.Id);
            }
            try
            {
                kill(process);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to kill " + process.Id + ": exception is " + e.ToString());
            }
        }

        private static void kill(Process process)
        {
            if (!process.HasExited) {
                process.Kill();        
            }
            process.WaitForExit();
            process.Close();
        }

        private static void KillProcessAndChildren(int pid)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                try
                {
                    Console.WriteLine("Killing childs of " + mo["ProcessID"]);
                    KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unable to kill "+ mo["ProcessID"]+": exception is "+e.ToString());
                }
            }
            try
            {
                Console.WriteLine("Killing " + pid);
                Process proc = Process.GetProcessById(pid);
                try
                {
                    proc.Kill();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unable to kill " + pid + ": exception is " + e.ToString());
                }
            }
            catch (ArgumentException)
            { /* process already exited */ }
        }

        private static bool killJavaSubprocesses(int pid)
        {
            KillProcessAndChildren(pid);
            return true;
        }
    }
}