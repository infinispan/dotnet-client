using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Infinispan.HotRod.Tests.Util
{
    public class PortProbe
    {
        private enum PortState
        {
            Open, Closed
        }

        public static bool IsPortClosed(string hostname, int port, int millisRetry = 500, int millisTimeout = 60000, bool retry = true)
        {
            return IsPort(hostname, port, millisRetry, millisTimeout, retry, PortState.Closed);
        }

        public static bool IsPortOpen(string hostname, int port, int millisRetry = 500, int millisTimeout = 60000, bool retry = true)
        {
            return IsPort(hostname, port, millisRetry, millisTimeout, retry, PortState.Open);
        }

        private static bool IsPort(string hostname, int port, int millisRetry, int millisTimeout, bool retry, PortState state)
        {
            IPAddress[] addrs;
            try {
                /* Try to parse the value as an IP address. */
                addrs = new IPAddress[] {IPAddress.Parse(hostname)};
            } catch (System.FormatException) {
                /* If it fails lookup the entry in DNS. */
                IPHostEntry entry = Dns.GetHostEntry(hostname);
                addrs = entry.AddressList;
            }

            DateTime start = DateTime.Now;
 
            while (true) {
                DateTime now = DateTime.Now;
                TimeSpan delta = now - start;
                if (delta.TotalMilliseconds >= millisTimeout) {
                    Console.WriteLine("Timeout reached.");
                    return false;
                }

                bool connected = false;
                for (int i = 0; i < addrs.Length; i++) {
                    string result = "";
                    Socket socket = new Socket(addrs[i].AddressFamily, SocketType.Stream, ProtocolType.IP);
                    try {
                        socket.Connect(addrs[i], port);
                        result = "succeded";
                        connected = true;
                        break;
                    } catch (Exception ex) {
                        result = String.Format("failed ({0})", ex.Message);
                    } finally {
                        Console.WriteLine("Probing {0}:{1} {5} (address {3} of {4}) {2}.", addrs[i], port, result, i + 1, addrs.Length, state);
                        socket.Close();
                    }
                }

                if ((connected && (state == PortState.Open)) || (!connected && (state == PortState.Closed))) {
                    return true;
                }

                if (!retry) {
                    Console.WriteLine("No retry.");
                    return false;
                }

                Console.WriteLine("Will retry in {0} millis. {1:0} millis to timeout.", millisRetry, millisTimeout - delta.TotalMilliseconds);
                Thread.Sleep(millisRetry);
            }
        }
    }
}