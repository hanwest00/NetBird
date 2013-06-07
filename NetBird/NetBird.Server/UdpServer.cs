using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NetBird.Server
{
    public class UdpServer : IServer, IDisposable, ICloneable
    {
        #region Field
        private UdpClient server;
        private Thread listenThread;
        private bool listening;
        #endregion

        #region Property
        public int Port
        { get; set; }

        public IPAddress SendIp
        { get; set; }

        public IPAddress ReceiveIp
        { get; set; }
        #endregion

        #region Constructor
        public UdpServer(IPAddress send)
            : this(send, null, 0)
        {

        }

        public UdpServer(IPAddress send, IPAddress receive, int port)
        {
            this.Port = port;
            this.ReceiveIp = receive;
            this.SendIp = send;
            server = new UdpClient(Port);
        }

        #endregion

        #region Event
        public delegate void SendComplete();
        public event SendComplete OnSendComplete;

        public delegate void ReceiveComplete(byte[] buffer);
        public event ReceiveComplete OnReceiveComplete;
        #endregion

        #region Public method
        public virtual void SendMessage(byte[] info)
        {
            try
            {
                server.Send(info, info.Length, new IPEndPoint(SendIp, Port));
                OnSendComplete();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public virtual void SendMessage(byte[] info, IPAddress ip, int port)
        {
            try
            {
                server.Send(info, info.Length, new IPEndPoint(ip, port));
                OnSendComplete();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public virtual void StartListen()
        {
            if (!listening)
                listening = true;

            if (listenThread == null)
            {
                IPEndPoint end = new IPEndPoint(ReceiveIp, 0);
                listenThread = new Thread(new ThreadStart(() =>
                {
                    while (listening)
                    {
                        try
                        {
                            byte[] buffer = server.Receive(ref end);
                            OnReceiveComplete(buffer);
                        }
                        catch (Exception e)
                        {
                            break;
                            throw e;
                        }
                    }
                }));
            }

            listenThread.IsBackground = true;
            listenThread.Start();
        }

        public virtual object Clone()
        {
            return new UdpServer(this.SendIp, this.ReceiveIp, this.Port);
        }

        public virtual void StopListen()
        {
            if (listenThread == null)
                return;
            listenThread.Abort();
        }

        public virtual void Dispose()
        {
            StopListen();
            server.Close();
        }
        #endregion
    }
}
