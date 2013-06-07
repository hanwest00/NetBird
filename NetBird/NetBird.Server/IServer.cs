using System;
using System.Collections.Generic;
using System.Text;

namespace NetBird.Server
{
    public interface IServer
    {
        void SendMessage(byte[] info);
        void StartListen();
        void StopListen();
    }
}
