using System;
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

namespace NetBird.Controls
{
    /// <summary>
    /// FileLabel.xaml 的交互逻辑
    /// </summary>
    public partial class FileLabel : UserControl
    {
        public int Id 
        {
            get; 
            set; 
        }

        private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set
            {
                filePath = value;
            }
        }

        public string FullFileName
        {
            get { return this.fileName.Content.ToString(); }
        }

        public string FileExt
        {
            get { return FullFileName.Split('.')[1]; }
        }

        public string FileName
        {
            get { return FullFileName.Split('.')[0]; }
        }

        public new int Width
        {
            get
            {
                return (int)this.main.Width;
            }
            set
            {
                this.main.Width = value;
            }
        }

        public int FileLength
        { get; set; }

        private int height;
        public new int Height
        {
            get
            {
                return (int)this.main.Height;
            }
            set { height = value; this.main.Height = value; }
        }

        public delegate void CloseEvent();
        public CloseEvent onCloseFile;

        public delegate void ClosedEvent();
        public ClosedEvent onClosedFile;

        public delegate void ProgressBeginEvent();
        public ProgressBeginEvent onProgressBegin;

        public delegate void ProgressEndEvent();
        public ProgressEndEvent onProgressEnd;
        public delegate void AcceptEvent();
        public AcceptEvent onAccept;

        public FileLabel(string path)
        {
            this.FilePath = path;
            InitializeComponent();
            onCloseFile += new CloseEvent(() => { });
            onClosedFile += new ClosedEvent(() => { });
            onProgressBegin += new ProgressBeginEvent(() => { });
            onProgressEnd += new ProgressEndEvent(() => { });
            onAccept += new AcceptEvent(() => { });

            this.fileName.Content = filePath.Substring(filePath.LastIndexOf("\\") + 1);
            this.Progress.Maximum = 100;
            
            ImageBrush imgB = new ImageBrush(System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.Close1.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));
            this.btnClose.Background = imgB;
            this.btnClose.BorderThickness = new Thickness(1);
            this.btnClose.Click += new RoutedEventHandler((object obj, RoutedEventArgs e) =>
            {
                this.Close();
            });

            this.acceptLabel.MouseLeftButtonUp += new MouseButtonEventHandler((object obj1, MouseButtonEventArgs e1) =>
            {
                this.ProgresBegin();
                this.HiddenAccept();
            });
        }

        public void Close()
        {
            try
            {
                onCloseFile();
                (this.Parent as Panel).Children.Remove(this);
                onClosedFile();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Accept()
        {
            if (this.acceptLabel.Visibility == System.Windows.Visibility.Collapsed)
            {
                this.acceptLabel.Visibility = System.Windows.Visibility.Visible;
                this.Height += 15;
            }
            this.onAccept();
        }

        public void HiddenAccept()
        {
            if (this.acceptLabel.Visibility == System.Windows.Visibility.Visible)
            {
                this.acceptLabel.Visibility = System.Windows.Visibility.Collapsed;
                this.Height -= 15;
            }
        }

        public void ProgresBegin()
        {
            if (this.Progress.Visibility == System.Windows.Visibility.Collapsed)
            {
                this.Progress.Visibility = System.Windows.Visibility.Visible;
                this.Height += 10;
            }
            onProgressBegin();
        }

        public void ProgresEnd()
        {
            if (this.Progress.Visibility == System.Windows.Visibility.Visible)
            {
                this.Progress.Visibility = System.Windows.Visibility.Collapsed;
                this.Height -= 10;
            }
            onProgressEnd();
        }
    }
}
