using System;
using System.Collections.Generic;
using System.Text;

namespace NetBird.Server.Model
{
    [Serializable]
    public class FileSendInfo
    {
        public int Id
        {
            get;
            set;
        }

        public int Seek
        {
            get;
            set;
        }

        public string IpAddress
        {
            get;
            set;
        }

        public byte[] Content
        {
            get;
            set;
        }

        public InfoType Type
        {
            get;
            set;
        }

        public enum InfoType
        {
            Send,
            Back,
            Over,
            SendClose,
            AcceptClose
        }
    }
}
