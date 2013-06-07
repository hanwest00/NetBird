using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NetBird.UI.Forms
{
    public partial class TestAnimation : Form
    {
        public Graphics g = null;

        public TestAnimation()
        {
            InitializeComponent();
            Bitmap bm = new Bitmap(this.Width, this.Height);
            g = Graphics.FromImage(bm);
            
        }

        ~TestAnimation()
        {
            g.Dispose();
        }
    }
}
