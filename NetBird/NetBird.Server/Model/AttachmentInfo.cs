using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NetBird.Server.Model
{
    [Serializable]
    public class AttachmentInfo
    {
        private int id;
        private string name;
        private string ext;
        private Icon icon;
        private int length;
        private string charset;


        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Ext
        {
            get { return ext; }
            set { ext = value; }
        }
        public Icon Icon
        {
            get { return icon; }
            set { icon = value; }
        }
        public int Length
        {
            get { return length; }
            set { length = value; }
        }
        public string Charset
        {
            get { return charset; }
            set { charset = value; }
        }
    }
}
