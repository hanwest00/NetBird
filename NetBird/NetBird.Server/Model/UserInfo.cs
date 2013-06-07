using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NetBird.Server.Model
{
    [Serializable]
    public class UserInfo
    {
        private string ipAddress;
        private string name;

        public string IpAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
