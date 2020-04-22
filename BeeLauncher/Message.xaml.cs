namespace BeeLauncher
{
    #region
    using MahApps.Metro.Controls;
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    #endregion
    /// <summary>
    /// message.xaml 的交互逻辑
    /// </summary>
    public partial class Message : MetroWindow
    {
        public static Message msg;
        private DispatcherTimer dTimer = new DispatcherTimer();
        public Message()
        { 
            InitializeComponent();
            if (MainWindow.lostassets == true)
            {
                TextMainTs.Text = "检查资源文件失败,游戏可能没有声音!";
            }
            else {
                TextMainTs.Text = "";
            }
            dTimer.Tick += new EventHandler(DTimer_Tick);
            dTimer.Interval = new TimeSpan(0, 0, 5);
            dTimer.Start();
        }

        private void Btn_确定_Click(object sender, RoutedEventArgs e)
        {
            Close();

        }
        public void DTimer_Tick(object sender, EventArgs e)
        {
            
            dTimer.Stop();
            Close();
        }
        private void Bg(object sender, RoutedEventArgs e)
        {
            if (Config.bg)
            {
                string dir = @AppDomain.CurrentDomain.BaseDirectory + "启动器背景";
                if (Directory.Exists(dir) && Config.bg)
                {
                    string[] pic = Directory.GetFiles(dir).Where(s => { return s.EndsWith(".jpg") || s.EndsWith(".bmp") || s.EndsWith(".png") || s.EndsWith(".gif"); }).ToArray();
                    int n = pic.Length;
                    if (n != 0)
                    {
                        Random ran = new Random();
                        int RandKey = ran.Next(1, n);
                        // this.BackgroundImage=Image.FromFile(r.Next(10).ToString()+".jpg");
                        ImageBrush b3 = new ImageBrush()
                        {
                            ImageSource = new BitmapImage(new Uri(pic[RandKey], UriKind.RelativeOrAbsolute))
                        };
                        grid.Background = b3;
                    }

                }
            }
        }
        public void Gb()
        {
           
            Close();
            msg = this;
        }
    }
}