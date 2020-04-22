namespace BeeLauncher
{
    #region
    using MahApps.Metro.Controls;
    using MahApps.Metro.Controls.Dialogs;
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    #endregion
    /// <summary>
    /// beelogin.xaml 的交互逻辑
    /// </summary>
    public partial class Register : MetroWindow
    {
        string id;
        public Register()
        {
            InitializeComponent();
        }

        private void Start(object sender, RoutedEventArgs e)
        {
            id = Beelogin.Beegetid(Config.beeurl);


            //MessageBox.Show(img);
            //  
            Newcode();
        }
        private void Newcode()
        {
            //  image.Source = new BitmapImage(new Uri(beelogin.beegetcode(Config.beeurl, id)));
            Random ro = new Random();
            int iResult;
            iResult = ro.Next();
            ImageBrush ib = new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(Beelogin.Beegetcode(Config.beeurl, id + "&" + iResult)))
            };
            code.Background = ib;
        }
        private void Btn_确定_Click(object sender, RoutedEventArgs e)
        {
           string fh = Beelogin.Beeregister(Config.beeurl ,textBox_name.Text, passbox_1.Password,passbox_1.Password,id,textBox_code.Text,MainWindow.ip);
            if (fh == "ok") { this.ShowMessageAsync("注册成功", "已成功注册"); Close(); } else { this.ShowMessageAsync("注册失败", fh); Newcode(); }
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

        private void Code_Click(object sender, RoutedEventArgs e)
        {
            Newcode();
        }
    }

}
