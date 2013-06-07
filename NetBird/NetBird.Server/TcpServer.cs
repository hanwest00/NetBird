using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NetBird.Server
{
    public class TcpServer : IServer, IDisposable, ICloneable
    {
        #region Field
        private TcpClient server;
        private NetworkStream ns;
        private Thread listenThread;
        private bool listening;
        #endregion

        #region Property
        public int Port
        { get; set; }

        public IPAddress Ip
        { get; set; }
        #endregion

        #region Constructor
        public TcpServer()
            : this(null, 0)
        {

        }

        public TcpServer(IPAddress ip, int port)
        {
            this.Port = port;
            this.Ip = ip;
            server = new TcpClient(new IPEndPoint(Ip, Port));
            ns = server.GetStream();
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
                ns.Write(info, 0, info.Length);
                ns.Flush();
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
                listenThread = new Thread(new ThreadStart(() =>
                {
                    while (listening)
                    {
                        try
                        {
                            byte[] buffer = new byte[36];
                            ns.Read(buffer, 0, buffer.Length);
                            string lengStr = Encoding.UTF8.GetString(buffer);
                            int bufferLeng = int.Parse(System.Text.RegularExpressions.Regex.Match("(?>=LENGTH:\r\n).*?(?=\r\n\r\n)", lengStr).Value.Replace(" ", ""));
                            if (bufferLeng < 1)
                                return;
                            buffer = new byte[bufferLeng];
                            ns.Read(buffer, 0, buffer.Length);
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

        public virtual void StopListen()
        {
            if (listenThread == null)
                return;
            listenThread.Abort();
        }

        public virtual object Clone()
        {
            return new TcpServer(this.Ip, this.Port);
        }

        public virtual void Dispose()
        {
            StopListen();
            server.Close();
        }
        #endregion
    }
}
