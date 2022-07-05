using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LightHTTP.Internal
{
    internal static class TcpHelpers
    {
        public static int GetOpenPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            try
            {
                listener.Start();
                return (listener.LocalEndpoint as IPEndPoint)!.Port;
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}
