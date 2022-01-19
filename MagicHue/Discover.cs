using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MagicHue
{
    public class Discover
    {
        private static byte[] _discoveryMsg = Encoding.ASCII.GetBytes("HF-A11ASSISTHREAD");
        private const int _discoveryPort = 48899;


        public static List<Light> Start(int timeout = 1)
        {
            var foundBulbs = new List<Light>();

            var socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
            socket.ReceiveTimeout = timeout * 1000;

            socket.SendTo(_discoveryMsg, IPEndPoint.Parse($"255.255.255.255:{_discoveryPort}"));
            try
            {
                while (true)
                {
                    var buffer = new byte[1024];
                    socket.Receive(buffer);
                    var resp = Encoding.ASCII.GetString(buffer);
                    var values = resp.Split(",");
                    foundBulbs.Add(new Light(values[0], values[1][6..]));
                }
            }
            catch (Exception)
            {
                return foundBulbs;
            }
            finally
            {
                socket.Close();
                socket.Dispose();
            }
        }
    }
}
