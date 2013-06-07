using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NetBird.Server.Model
{
    [Serializable]
    public class MessageInfo
    {

        private Type messageType;
        private UserInfo from;
        private string messageBody;
        private List<UserInfo> to;
        private List<AttachmentInfo> attachment;
        private DateTime sendDate;
        private Sign messageSign;

        public Type MessageType
        {
            get { return messageType; }
            set { messageType = value; }
        }

        public UserInfo From
        {
            get { return from; }
            set { from = value; }
        }

        public string MessageBody
        {
            get { return messageBody; }
            set { messageBody = value; }
        }

        public List<UserInfo> To
        {
            get { return to; }
            set { to = value; }
        }

        public List<AttachmentInfo> Attachment
        {
            get { return attachment; }
            set { attachment = value; }
        }

        public DateTime SendDate
        {
            get { return sendDate; }
            set { sendDate = value; }
        }

        public Sign MessageSign
        {
            get { return messageSign; }
            set { messageSign = value; }
        }

        public enum Type
        {
            /// <summary>
            /// 普通信息
            /// </summary>
            Common, 
            /// <summary>
            /// 文件
            /// </summary>
            Attachment, 
            /// <summary>
            /// 信号
            /// </summary>
            Sign 
        }

        public enum Sign
        {
            /// <summary>
            /// 上线
            /// </summary>
            Online,
            /// <summary>
            /// 收到上线响应
            /// </summary>
            ResponseOnline,
            /// <summary>
            /// 请求传输文件
            /// </summary>
            FileRequest,
            /// <summary>
            /// 接受传输文件请求
            /// </summary>
            FileAccept,
            /// <summary>
            /// 下线
            /// </summary>
            Offline 
        }

        public override string ToString()
        {
            System.Runtime.Serialization.IFormatter format = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                format.Serialize(stream, this);
                using (System.IO.StreamReader sr = new System.IO.StreamReader(stream))
                    return sr.ReadToEnd();
            }
        }
    }
}
