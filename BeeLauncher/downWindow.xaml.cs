namespace BeeLauncher
{
    #region
    using MahApps.Metro.Controls;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    #endregion
    /// <summary>
    /// downWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DownWindow : MetroWindow
    {
        public static List<string> downFile = new List<string>();
        public static BackgroundWorker bgWorker = new BackgroundWorker();
        public DownWindow()
        {

            InitializeComponent();

        }
        public static DownWindow down;
        private void Bg(object sender, RoutedEventArgs e)
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
        private void Btn_确定_Click(object sender, RoutedEventArgs e)
        {
           
           //Close();
            Hide();
        }
        private void Start(object sender, EventArgs e)
        {
            var max = MainWindow.lostFlie.Count;
            var ass = MainWindow.lostasset.Count;
            var oldass = MainWindow.lostlegacyasset.Count;
            if(!bgWorker.IsBusy){
                if (max != 0)
                {
                    jdt.Maximum = max;
                    ts.Text = "正在下载缺少的" + max.ToString() + "个支持库文件";
                    bgWorker.WorkerReportsProgress = true;
                    bgWorker.ProgressChanged += new ProgressChangedEventHandler(Bw_ProgressChanged);
                    bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Bw_RunWorkerCompleted);
                    bgWorker.DoWork += new DoWorkEventHandler(Bw_DoWork1);
                    bgWorker.RunWorkerAsync();
                }
                else if (ass != 0 || oldass != 0)
                {
                    btn_确定.Content = "后台下载";
                    jdt.Maximum = (ass != 0 ? ass : oldass);
                    ts.Text = "正在下载" + (ass != 0 ? "当前版本" : "") + "缺少的" + (ass != 0 ? ass.ToString() : oldass.ToString()) + "个" + (oldass != 0 ? "低版本通用" : "") + "资源文件";
                    bgWorker.WorkerReportsProgress = true;
                    bgWorker.ProgressChanged += new ProgressChangedEventHandler(Bw_ProgressChanged);
                    bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Bw_RunWorkerCompleted);
                    bgWorker.DoWork += new DoWorkEventHandler(Bw_asswork);
                    bgWorker.RunWorkerAsync();
                }
            }

         //       Thread t = new Thread(libdown);
            //t.Start();
     //       t.Join();
        }

        void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //这时后台线程已经完成，并返回了主线程，所以可以直接使用UI控件了 

            Close();
        }
        private void Bw_ProgressChanged(object sender, ProgressChangedEventArgs e)

        {

            jdt.Value = e.ProgressPercentage;

        }
        void Bw_DoWork1(object sender, DoWorkEventArgs e)
        {
                downFile = MainWindow.lostFlie;
                    int max = downFile.Count;
                    //   SetmaxProgressBar(max + 1);

                    for (int i = 0; max > i; i++)
                    {
                        string filename = downFile[i];
                        string dir = Beelogin.GetLeft(downFile[i], "\\");

                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);//创建文件夹
                        }
                        string URL = "http://bmclapi2.bangbang93.com" + Beelogin.GetRight(downFile[i], "\\libraries").Replace("\\", "/").ToString();
                        //MessageBox.Show(URL);
     
                            if (Beelogin.DownloadFile(URL, filename))
                            {
                                bgWorker.ReportProgress(i);
                            }
                           
                        else
                        {
                            Dispatcher.Invoke(new Action(() => { ts.Text = "下载过程中出现了错误,将继续下载.出错URL:"+URL; }));
                        }
                        // jdt.Dispatcher.Invoke(new Action(() => { jdt.Value = i; }));
                }
               
        }
        void Bw_asswork(object sender, DoWorkEventArgs e)
        {
                bool newass;
                if (MainWindow.lostasset.Count != 0) { newass = true; } else { newass = false; }
                downFile = (newass == true ? MainWindow.lostasset : MainWindow.lostlegacyasset);
                int max = downFile.Count;
                //   SetmaxProgressBar(max + 1);
                for (int i = 0; max > i; i++)
                {
                    string filename;
                    string two;
                   
                    if (newass==true) {
                        two = downFile[i].Substring(0, 2);
                        filename = @AppDomain.CurrentDomain.BaseDirectory + @".minecraft\assets\objects\" + two + "\\" + downFile[i];
                        //MessageBox.Show(filename);
                    }else
                    {
                         filename = @AppDomain.CurrentDomain.BaseDirectory + @".minecraft\assets\"+downFile[i];
                    }
                    string dir = Beelogin.GetLeft(filename, "\\");
                if (!Directory.Exists(dir))
                    {
                    Directory.CreateDirectory(dir);//创建文件夹
                    
                }
                    string URL = "http://bmclapi2.bangbang93.com/"+ (newass == true ? "assets" : "resources")   + "/" + (newass == true ? $"{downFile[i].Substring(0, 2)}/" : "") + downFile[i].Replace("\\", "/");
                   // MessageBox.Show(dir+URL);
                    try
                    {
                        if (Beelogin.DownloadFile(URL, filename) == true)
                        {
                            bgWorker.ReportProgress(i);
                        }
                    }
                    catch
                    {
                    Dispatcher.Invoke(new Action(() => { ts.Text = "下载过程中出现了错误,将继续下载.出错URL:" + URL; }));
                }
            }
        }
    }
}
