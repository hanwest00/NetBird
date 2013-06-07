#define DEBUG

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using NetBird.Server.Model;
using NetBird.Server;
using NetBird.Util;

namespace NetBird
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //private Thread listenThread;

        //文件传输线程
        private Thread fileThread;

        //通信端口
        private int port = 3081;
        //文件传输端口
        private int filePort = 6890;

        //缓存边框画刷
        private Brush bs;

        //本机信息
        private UserInfo localInfo;

        //目标信息
        private UserInfo selectInfo;

        //上线广播信息
        private MessageInfo Online;


        private UdpServer broadcast;
        private UdpServer fileRequestListen;

        //文件接受队列
        private Queue fileReceive;

        //文件发送队列
        private Queue fileSend;

        //线程开关
        private bool running;

        private int fileSpeed = 60 * 1024;//buffer lebngth

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                this.fileScrollViewer.Background = Brushes.LightGray;
                this.fileScrollViewer.BorderBrush = Brushes.Gray;
            }
            catch (Exception ex)
            {
                AppLog.SysLog(ex.ToString());
            }

            try
            {
                localInfo = new UserInfo
                {
                    IpAddress = NetworkUtil.GetLocalIp(),
                    Name = this.textBox1.Text
                };

                richTextBox1.KeyDown += richTextBox1_KeyDown;

                this.userList.SelectionChanged += new SelectionChangedEventHandler(new EventHandler((object obj, EventArgs e) =>
                {
                    if (fileCanvas.Children.Count > 0)
                    {
                        MessageBox.Show("有尚未完成的文件传输");
                        return;
                    }
                    if (userList.SelectedItem != null)
                        selectInfo = new UserInfo { IpAddress = (userList.SelectedItem as ListBoxItem).Content.ToString() };
                }));

                fileReceive = new Queue();
                fileSend = new Queue();
                running = true;
                broadcast = new UdpServer(IPAddress.Parse(NetworkUtil.GetNetBroad()), IPAddress.Any, port);
                Online = new MessageInfo { From = localInfo, MessageType = MessageInfo.Type.Sign, MessageSign = MessageInfo.Sign.Online };

                broadcast.OnReceiveComplete += new UdpServer.ReceiveComplete((s) =>
                {
                    MemoryStream ms = new MemoryStream(s);
                    MessageInfo msg = NetBird.Util.Serializable.Deserialize<MessageInfo>(ms);
                    ms.Close();
                    if (msg.From != null)
                        switch (msg.MessageType)
                        {
                            case MessageInfo.Type.Sign:
                                switch (msg.MessageSign)
                                {
                                    case MessageInfo.Sign.Online: this.userList.Dispatcher.Invoke(new Action(() =>
                                    {
                                        //AddUserList(msg.From); //测试时允许添加本机IP到UserList
                                        if (msg.From.IpAddress == localInfo.IpAddress)
                                            return;
                                        AddUserList(msg.From);
                                        MessageInfo rspInfo = new MessageInfo { From = localInfo, MessageSign = MessageInfo.Sign.ResponseOnline, MessageType = MessageInfo.Type.Sign };
                                        broadcast.SendMessage(Serializable.SerializableToBytes(rspInfo), IPAddress.Parse(msg.From.IpAddress), port);
                                    }));
                                        break;
                                    case MessageInfo.Sign.Offline:

                                        this.userList.Dispatcher.Invoke(new Action(() =>
                                        {
                                            if (localInfo.IpAddress != msg.From.IpAddress)
                                                RemoveUserList(msg.From);
                                        }));
                                        break;
                                    case MessageInfo.Sign.FileRequest:
                                        if (msg.Attachment != null)
                                            fileReceive.Enqueue(msg);
                                        break;
                                    case MessageInfo.Sign.FileAccept:
                                        if (msg.Attachment != null)
                                            fileSend.Enqueue(msg);
                                        break;
                                    case MessageInfo.Sign.ResponseOnline:
                                        this.userList.Dispatcher.Invoke(new Action(() =>
                                        {
                                           AddUserList(msg.From);
                                        }));
                                        break;
                                }
                                break;
                            case MessageInfo.Type.Common:
                                CommonMessage(string.Format("{0}:{1}", msg.From.IpAddress, msg.MessageBody));
                                break;
                        }
                });

                broadcast.OnSendComplete += new UdpServer.SendComplete(() => { });

                broadcast.SendMessage(NetBird.Util.Serializable.SerializableToBytes(Online));
                broadcast.StartListen();

                //udpClient = new UdpClient(port);
                //listen = new UdpClient(port);
                //tcpClient = new TcpClient();

                fileThread = new Thread(new ThreadStart(() =>
                {
                    fileRequestListen = new UdpServer(IPAddress.Parse(localInfo.IpAddress), IPAddress.Any, filePort);
                    fileRequestListen.StartListen();
                    fileRequestListen.OnSendComplete += new UdpServer.SendComplete(() => { });
                    fileRequestListen.OnReceiveComplete += new UdpServer.ReceiveComplete((byte[] content) =>
                    {
                        MemoryStream ms = new MemoryStream(content);
                        FileSendInfo fsInfo = Serializable.Deserialize<FileSendInfo>(ms);
                        ms.Close();
                        switch (fsInfo.Type)
                        {
                            case FileSendInfo.InfoType.Send:
                                this.fileRCanvas.Dispatcher.Invoke(new Action(() =>
                                {
                                    try
                                    {
                                        var rlb = GetFileLabelById(fsInfo.Id);
                                        if (rlb != null)
                                        {
                                            FileStream rfs = File.OpenWrite(rlb.FilePath);
                                            rfs.Position = fsInfo.Seek;
                                            rfs.Write(fsInfo.Content, 0, fsInfo.Content.Length);
                                            int rseek = (int)rfs.Length;
                                            rfs.Close();
                                            //double ss = (double)rseek / (double)rlb.FileLength;
                                            rlb.Progress.Value = ((double)rseek / (double)rlb.FileLength) * 100;
                                            FileSendInfo rnfsInfo = new FileSendInfo { Id = fsInfo.Id, Seek = rseek, Type = FileSendInfo.InfoType.Back, IpAddress = localInfo.IpAddress };
                                            fileRequestListen.SendMessage(Serializable.SerializableToBytes(rnfsInfo), IPAddress.Parse(fsInfo.IpAddress), filePort);
                                        }
                                    }
                                    catch (Exception ex0)
                                    {
                                        throw ex0;
                                    }
                                }));
                                break;
                            case FileSendInfo.InfoType.Back:
                                this.fileCanvas.Dispatcher.Invoke(new Action(() =>
                                {
                                    try
                                    {
                                        var lb = fileCanvas.Children[fsInfo.Id] as Controls.FileLabel;
                                        int seek = fsInfo.Seek;
                                        FileSendInfo nfsInfo = null;
                                        FileStream fs = File.OpenRead(lb.FilePath);
                                        //文件发送完信号
                                        if (fs.Length - 1 < seek)
                                        {
                                            nfsInfo = new FileSendInfo { Id = fsInfo.Id, IpAddress = localInfo.IpAddress, Type = FileSendInfo.InfoType.Over };
                                           
                                        }
                                        else
                                        {
                                            byte[] buffer = new byte[fileSpeed];
                                            fs.Position = seek;
                                            fs.Read(buffer, 0, buffer.Length);
                                            fs.Close();
                                            lb.Progress.Value = ((double)seek / (double)lb.FileLength) * 100;
                                            nfsInfo = new FileSendInfo { Id = fsInfo.Id, Seek = seek, Content = buffer, IpAddress = localInfo.IpAddress, Type = FileSendInfo.InfoType.Send };
                                        }
                                        fileRequestListen.SendMessage(Serializable.SerializableToBytes(nfsInfo), IPAddress.Parse(fsInfo.IpAddress), filePort);
                                    }
                                    catch (Exception ex0)
                                    {
                                        throw ex0;
                                    }
                                }));
                                break;
                            case FileSendInfo.InfoType.SendClose:
                            case FileSendInfo.InfoType.Over:
                                this.fileRCanvas.Dispatcher.Invoke(new Action(() =>
                                {
                                    var olb = GetFileLabelById(fsInfo.Id);
                                    if (olb != null)
                                        olb.Close();
                                }));
                                break;
                            case FileSendInfo.InfoType.AcceptClose:
                                this.fileCanvas.Dispatcher.Invoke(new Action(() =>
                                {
                                    var lb = fileCanvas.Children[fsInfo.Id] as Controls.FileLabel;
                                    if (lb != null)
                                        lb.Close();
                                }));
                                break;
                        }
                    });

                    while (running)
                    {
                        if (fileSend.Count > 0)
                        {
                            MessageInfo fileInfo = fileSend.Dequeue() as MessageInfo;
                            AttachmentInfo attr = fileInfo.Attachment[0];
                            this.fileCanvas.Dispatcher.Invoke(new Action(() =>
                            {
                                Controls.FileLabel lb = fileCanvas.Children[attr.Id] as Controls.FileLabel;
                                lb.onCloseFile += new Controls.FileLabel.CloseEvent(() =>
                                {
                                    FileSendInfo fsOver = new FileSendInfo { Id = attr.Id, IpAddress = localInfo.IpAddress, Type = FileSendInfo.InfoType.SendClose };
                                    fileRequestListen.SendMessage(Serializable.SerializableToBytes(fsOver), IPAddress.Parse(fileInfo.From.IpAddress), filePort);
                                });
                                byte[] buffer = new byte[fileSpeed];
                                FileStream fs = File.OpenRead(lb.FilePath);
                                fs.Position = 0;
                                fs.Read(buffer, 0, buffer.Length);
                                fs.Close();
                                FileSendInfo fsInfo = new FileSendInfo { Id = attr.Id, Seek = 0, Content = buffer, IpAddress = localInfo.IpAddress, Type = FileSendInfo.InfoType.Send };
                                fileRequestListen.SendMessage(Serializable.SerializableToBytes(fsInfo), IPAddress.Parse(fileInfo.From.IpAddress), filePort);
                                //fileRequestListen.SendMessage();

                                //todu 发送文件
                            }));
                        }

                        if (fileReceive.Count > 0)
                        {
#if DEBUG
                            try
                            {
#endif
                                //todo 接受文件 (需要Add到FileCanvas)
                                MessageInfo fileInfo = fileReceive.Dequeue() as MessageInfo;
                                AttachmentInfo attr = fileInfo.Attachment[0];
                                this.fileRCanvas.Dispatcher.Invoke(new Action(() =>
                                {

                                    var fb = new Controls.FileLabel(string.Format("{0}{1}.{2}", PathInfo.GetUserAttachmentDir(), attr.Name, attr.Ext));
                                    fb.Id = attr.Id;
                                    fb.FileLength = attr.Length;
                                    fb.fileIcon.Source = IconHelper.GetIcon(attr.Icon.Handle);
                                    fb.onCloseFile += new Controls.FileLabel.CloseEvent(() =>
                                    {
                                        foreach (UserControl uie in this.fileRCanvas.Children)
                                        {
                                            if (uie.Margin.Top > fb.Margin.Top)
                                                uie.Margin = new Thickness(uie.Margin.Left, uie.Margin.Top - fb.Height, uie.Margin.Right, uie.Margin.Bottom);
                                        }
                                        if (this.fileRCanvas.Height > 200)
                                            this.fileRCanvas.Height -= fb.Height;
                                    });
                                    fb.onClosedFile += new Controls.FileLabel.ClosedEvent(() =>
                                    {
                                        if (this.fileRCanvas.Children.Count > 0)
                                            return;
                                        this.Width -= 200;
                                        fileRScrollViewer.Visibility = System.Windows.Visibility.Collapsed;
                                        FileSendInfo fsOver = new FileSendInfo { Id = attr.Id, IpAddress = localInfo.IpAddress, Type = FileSendInfo.InfoType.AcceptClose };
                                        fileRequestListen.SendMessage(Serializable.SerializableToBytes(fsOver), IPAddress.Parse(fileInfo.From.IpAddress), filePort);
                                    });
                                    fb.onProgressBegin += new Controls.FileLabel.ProgressBeginEvent(() =>
                                    {
                                        fb.Height += 10;
                                        FileSendInfo fsInfo = new FileSendInfo { Id = attr.Id, Seek = 0, IpAddress = localInfo.IpAddress, Type = FileSendInfo.InfoType.Back };
                                        fileRequestListen.SendMessage(Serializable.SerializableToBytes(fsInfo), IPAddress.Parse(fileInfo.From.IpAddress), filePort);
                                    });
                                    fb.onProgressEnd += new Controls.FileLabel.ProgressEndEvent(() =>
                                    {
                                        fb.Height -= 10;
                                    });
                                    AddFileToCanvas(fb, ref this.fileRScrollViewer, ref this.fileRCanvas);
                                    fb.Accept();
                                }));
#if DEBUG
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
#endif
                        }
                    }
                }));

                fileThread.IsBackground = true;
                fileThread.Start();

                this.richTextBox1.AddHandler(RichTextBox.DragOverEvent, new DragEventHandler(richTextBox1_DragOver), true);
                this.richTextBox1.AddHandler(RichTextBox.DropEvent, new DragEventHandler(richTextBox1_Drop), true);
                this.richTextBox1.AddHandler(RichTextBox.DragLeaveEvent, new DragEventHandler(richTextBox1_DragLeave), true);
                bs = this.richTextBox1.BorderBrush;


            }
            catch (Exception e)
            {
                AppLog.SysLog(e.ToString());
            }
        }

        void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                this.DoSend();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show((sender as MenuItem).Header.ToString());
        }

        private void AddUserList(UserInfo user)
        {
            ListBoxItem listbox = new ListBoxItem();
            listbox.Content = user.IpAddress;
            userList.Items.Add(listbox);
        }

        private void RemoveUserList(UserInfo user)
        {
            int i = 0;

            while (i < userList.Items.Count)
            {
                if ((userList.Items[i] as ListBoxItem).Content.ToString() == user.IpAddress)
                    break;
                i++;
            }

            userList.Items.RemoveAt(i);
        }

        private void richTextBox1_Drop(object sender, DragEventArgs e)
        {
            if (userList.SelectedIndex < 0)
            {
                MessageBox.Show("请选择目标IP");
                return;
            }
            string[] fileArr = e.Data.GetData(DataFormats.FileDrop) as string[];

            foreach (string path in fileArr)
            {
                //只接受文件传输 
                //todo:文件夹传输
                bool isDir = Directory.Exists(path) ? true : false;
                if (isDir)
                    continue;
                var fileLabel1 = new Controls.FileLabel(path);

                //获取文件icon并设置到FileLabel 需引用GDI+
                //System.Drawing.Bitmap img = System.Drawing.Icon.ExtractAssociatedIcon(path).ToBitmap();
                //fileLabel1.fileIcon.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(img.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                //获取文件icon并设置到FileLabel 使用SHGetFileInfo
                fileLabel1.fileIcon.Source = IconHelper.GetIcon(path, false, false);
                var flr = File.OpenRead(path);
                fileLabel1.FileLength = (int)flr.Length;
                flr.Close();
                fileLabel1.onCloseFile += new Controls.FileLabel.CloseEvent(() =>
                {
                    this.fileCanvas.Dispatcher.Invoke(new Action(() =>
                    {
                        foreach (UserControl uie in this.fileCanvas.Children)
                        {
                            if (uie.Margin.Top > fileLabel1.Margin.Top)
                                uie.Margin = new Thickness(uie.Margin.Left, uie.Margin.Top - fileLabel1.Height, uie.Margin.Right, uie.Margin.Bottom);
                        }
                        if (this.fileCanvas.Height > 200)
                            this.fileCanvas.Height -= fileLabel1.Height;
                    }));
                });
                fileLabel1.onClosedFile += new Controls.FileLabel.ClosedEvent(() =>
                {
                    if (this.fileCanvas.Children.Count > 0)
                        return;
                    this.Width -= 200;
                    fileScrollViewer.Visibility = System.Windows.Visibility.Collapsed;
                });
                fileLabel1.onProgressBegin += new Controls.FileLabel.ProgressBeginEvent(() =>
                {
                    fileLabel1.Height += 10;
                });

                fileLabel1.onProgressEnd += new Controls.FileLabel.ProgressEndEvent(() =>
                {
                    fileLabel1.Height -= 10;
                });

                AddFileToCanvas(fileLabel1, ref this.fileScrollViewer, ref this.fileCanvas);

                MessageInfo msgInfo = new MessageInfo
                {
                    MessageType = MessageInfo.Type.Sign,
                    MessageSign = MessageInfo.Sign.FileRequest,
                    From = localInfo,
                    Attachment = new List<AttachmentInfo> {
                            new AttachmentInfo { 
                                Id = fileCanvas.Children.Count - 1,
                                Charset = "UTF-8",
                                Ext = fileLabel1.FileExt, 
                                Length = fileLabel1.FileLength,
                                Icon = System.Drawing.Icon.FromHandle(IconHelper.GetIconIntPtr(path,false,isDir)),
                                Name = fileLabel1.FileName
                            }
                        }
                };
                fileLabel1.ProgresBegin();
                broadcast.SendMessage(Serializable.SerializableToBytes(msgInfo), IPAddress.Parse(selectInfo.IpAddress), port);
            }
            richTextBox1.BorderBrush = bs;
        }

        private void AddFileToCanvas(Controls.FileLabel element, ref ScrollViewer scrollViewer, ref Canvas fileParent)
        {

            if (scrollViewer.Visibility == System.Windows.Visibility.Collapsed)
            {
                scrollViewer.Visibility = System.Windows.Visibility.Visible;
                this.Width += 200;
            }

            double d = fileParent.Children.Count > 0 ? (fileParent.Children.Count * element.Height) : 0;

            element.Margin = new Thickness(0, d, 0, 0);

            if (d + element.Height >= fileParent.Height)
                fileParent.Height += element.Height;
            fileParent.Children.Add(element);
        }

        private void richTextBox1_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
            e.Handled = false;
            richTextBox1.BorderBrush = Brushes.Gray;
        }

        private void richTextBox1_DragLeave(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
            e.Handled = false;
            richTextBox1.BorderBrush = bs;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void CommonMessage(string msg)
        {
            this.textBlock1.Dispatcher.Invoke(new Action(() =>
            {
                this.textBlock1.Inlines.Add(string.Format("{0}\r\n", msg));
            }));
        }

        private void SendInfo(string info, string ip)
        {
            MessageInfo send = new MessageInfo();
            send.From = localInfo;
            send.MessageType = MessageInfo.Type.Common;
            send.MessageBody = info;
            broadcast.SendMessage(Serializable.SerializableToBytes(send), IPAddress.Parse(ip), port);
        }

        private Controls.FileLabel GetFileLabelById(int id)
        {
            foreach (var fb in fileRCanvas.Children)
            {
                var ffb = fb as Controls.FileLabel;
                if (ffb.Id == id)
                    return ffb;
            }
            return null;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            running = false;
            MessageInfo offInfo = new MessageInfo { From = localInfo, MessageType = MessageInfo.Type.Sign, MessageSign = MessageInfo.Sign.Offline };
            if (broadcast != null)
            {
                broadcast.SendMessage(Serializable.SerializableToBytes(offInfo));
                broadcast.Dispose();
            }
            if (fileRequestListen != null)
                fileRequestListen.Dispose();
            if (fileThread != null)
            fileThread.Abort();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DoSend();
        }

        private void DoSend()
        {
            if (selectInfo == null)
            {
                MessageBox.Show("请选择目标IP");
                return;
            }
            string msg = new TextRange(richTextBox1.Document.ContentStart, richTextBox1.Document.ContentEnd).Text;
            this.SendInfo(msg, selectInfo.IpAddress);

            this.textBlock1.Inlines.Add(string.Format("{0}\r\n", msg));
            this.richTextBox1.Document.Blocks.Clear();
        }
    }
}
