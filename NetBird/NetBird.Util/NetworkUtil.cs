using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace NetBird.Util
{
    public class NetworkUtil
    {
        public static string GetLocalIp()
        {
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
            if (ips.Length > 1)
            {
                for (int i = ips.Length - 1; i >= 0; i--)
                {
                    if (ips[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        return ips[i].ToString();
                }
                return string.Empty;
            }
            else
                return ips[0].ToString();
        }

        public static string GetNetBroad()
        {
            string ip = GetLocalIp();
            return string.Format("{0}.{1}",ip.Remove(ip.LastIndexOf('.')), "255");
        }
    }
}
