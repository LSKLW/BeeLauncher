namespace BeeLauncher
{
    #region

    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using KMCCC.Authentication;
    using KMCCC.Launcher;
    using Version = KMCCC.Launcher.Version;
    using System.IO;
    using MahApps.Metro.Controls;
    using MahApps.Metro.Controls.Dialogs;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using MahApps.Metro;
    using System.Collections.Generic;
    using LitJson;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json;
    using System.Text;
    using System.Windows.Threading;
    using System.ComponentModel;
    using System.Diagnostics;
    using static KMCCC.Launcher.Reporter;



    #endregion

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>

    public partial class MainWindow : MetroWindow
    {
        public static MainWindow main;
        string rundir = @AppDomain.CurrentDomain.BaseDirectory;
        public static List<string> lostFlie = new List<string>();
        public static List<string> lostasset = new List<string>();
        public static List<string> lostlegacyasset = new List<string>();
        BackgroundWorker bgWorker = new BackgroundWorker();
        public static string ip;

        public MainWindow()
        {
            InitializeComponent();

         //   System.Windows.Forms.NotifyIcon notify = new System.Windows.Forms.NotifyIcon()
           // {
          //      Icon = new System.Drawing.Icon(@"ico.ico"),
          //      Visible = true
        //    };
            bool cfg = File.Exists(rundir + "BeeLauncher.cfg");
            if (!cfg || string.IsNullOrWhiteSpace(Config.UserName))
            {
                Autonc();
                try
                {
                    textBox_java.Text = KMCCC.Tools.SystemTools.FindJava().Last();
                }
                catch
                {
                    textBox_java.Text = "未获取到Java,请手动指定";
                }
                if (Config.Beelogin && !string.IsNullOrWhiteSpace(Config.beeurl))
                {
                    CheckBox_beelogin.IsChecked = true;
                    CheckBox_beijing.IsChecked = true;
                    passbox_beelogin.IsEnabled = true;
                    textblpsw.Visibility = Visibility.Visible;
                    passbox_beelogin.Visibility = Visibility.Visible;
                    CheckBox_mod.Visibility = Visibility.Visible;
                    passbox_beelogin.Password = Config.Beepsw;
                    Btn_zhuce.Visibility = Visibility.Visible;
                    if (Config.Beemod)
                    {

                        CheckBox_mod.IsChecked = true;
                    }
                }
                设置框.Visibility = Visibility.Visible;
            } else
            {
                if (!string.IsNullOrWhiteSpace(Config.color) && !string.IsNullOrWhiteSpace(Config.Theme))
                {
                    if (!string.IsNullOrWhiteSpace(Config.color))
                    {

                        ThemeManager.ChangeAppStyle(System.Windows.Application.Current,
                                       ThemeManager.GetAccent(Config.color),
                                       ThemeManager.GetAppTheme("BaseLight"));
                    }
                    else
                    {
                        Config.color = "Blue";
                    }
                    if (Config.Theme == "Dark")
                    {
                        ThemeManager.ChangeAppStyle(System.Windows.Application.Current,
                                       ThemeManager.GetAccent(Config.color),
                                       ThemeManager.GetAppTheme("BaseDark"));
                    }
                }

            }
            if (Config.Beelogin) { ip = Beelogin.GetIPAddress(); }
            App.Core.GameExit += OnExit;
            App.Core.GameLog += OnLog;
            Getver();
        }
        public void Getver()
        {
            main = this;
            var versions = App.Core.GetVersions().ToArray();
            ListVersions.ItemsSource = versions;
            var last = Config.LastVersion;
            //var lj = AppDomain.CurrentDomain.BaseDirectory + ".minecraft/versions/";
            //  DirectoryInfo di = new DirectoryInfo(lj);
            // var wj = di.GetDirectories();//获取子文件夹列表
            if (versions.Count(ver => ver.Id == last) > 0)
            {
                ListVersions.SelectedItem = versions.First(ver => ver.Id == last);
            }
            else if (versions.Any())
            {
                ListVersions.SelectedItem = versions[0];
            }
        }
        private void Autonc()
        {
            var nc = KMCCC.Tools.SystemTools.GetTotalMemory() / 2;
            var ws = KMCCC.Tools.SystemTools.GetArch();
            if (ws == "64")
            {
                if (nc > 5000)
                {
                    textBox_neicun.Text = "4096";
                }
            }
            else
            {
                textBox_neicun.Text = "1024";
            }
        }
        private static void OnLog(LaunchHandle handle, string line)
        {
            Logger.Log(line);
        }
        private void OnExit(LaunchHandle handle, int code)
        {
            Dispatcher.Invoke((Action<int>)OnGameExit, code);
        }
        private void OnGameExit(int code)
        {
           // MessageBox.Show(code.ToString());
            if (Config.Beelogin) { Beelogin.Beequit(Config.beeurl,Config.UserName,ip); }
            if (code == 0)
            {
                //  Show();
                //Close();
                Process.GetCurrentProcess().Kill();
            }
            else
            {
                Show();
                //   message.msg.gb();
                this.ShowMessageAsync("游戏已崩溃", "游戏日志保存在BeeLauncher.log", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                bgWorker.Dispose();
            }
        }
        private void Button2_Click(object sender, RoutedEventArgs e)//设置按钮
        {
            if (设置框.Visibility == Visibility.Hidden)
            {
                textBox_name.Text = Config.UserName;
                CheckBox_zhengban.IsChecked = Config.Authenticator;
                passbox_zhengban.Password = Config.Password;
                CheckBox_neicun.IsChecked = Config.AutoMemory;
                if (Config.AutoMemory)
                {
                    Autonc();
                }
                else
                {
                    textBox_neicun.IsEnabled = true;
                    textBox_neicun.Text = Config.MaxMemory.ToString(CultureInfo.InvariantCulture);
                }
                textBox_java.Text = Config.JavPath;
                textBox_canshu.Text = Config.ip;
                textBox_port.Text = Config.port;
                CheckBox_beijing.IsChecked = Config.bg;
                rbtn1.IsChecked = Config.LaunchMode;
                rbtn2.IsChecked = !Config.LaunchMode;

                if (Config.Beelogin && !string.IsNullOrWhiteSpace(Config.beeurl))
                {
                    CheckBox_beelogin.IsChecked = true;
                    CheckBox_beijing.IsChecked = true;
                    passbox_beelogin.IsEnabled = true;
                    textblpsw.Visibility = Visibility.Visible;
                    passbox_beelogin.Visibility = Visibility.Visible;
                    CheckBox_mod.Visibility = Visibility.Visible;
                    passbox_beelogin.Password = Config.Beepsw;
                    Btn_zhuce.Visibility = Visibility.Visible;
                    if (Config.Beemod)
                    {
                        CheckBox_mod.IsChecked = true;
                    }
                }
                else if (Config.Beelogin) {
                    this.ShowMessageAsync("无法启用蜜蜂登录", "未配置蜜蜂登录地址,请在BeeLauncher.cfg中设置", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                    Config.Beelogin = false;
                }
                if (text.Count == 0) {
                    text.Add("BeeStudio 提供技术支持");
                    text.Add("Fleey.org 提供技术支持");
                    text.Add("KMCCC 提供启动技术支持");
                    text.Add("BMCLAPI 提供下载加速");
                    text.Add("LitJson 提供Json解析");
                    text.Add("Newtonsoft.Json 提供Json解析");
                    text.Add("MahApps.Metro 提供UI支持");
                    text.Add("欢迎使用BeeLauncher!");
                    var dtime = new DispatcherTimer(TimeSpan.FromSeconds(2), DispatcherPriority.Normal, Tick, Dispatcher);
                }
                设置框.Visibility = Visibility.Visible;
            }
        }
        List<string> text = new List<string>();
        int cs;
        void Tick(object sender, EventArgs e)
        {
            if (cs ==  text.Count-1) { cs = 0; } else { cs = cs + 1; }
          //  MessageBox.Show(text.Count+cs.ToString());
            SecondcustomTransitioning.Content = new TextBlock { Text = text[cs], SnapsToDevicePixels = true };
        }
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Ts(string bt, string wb)
        {
            Dispatcher.Invoke(new Action(() => { this.ShowMessageAsync(bt, wb); }));
        }
        Version ver;
        LauncherCore core;
        DownWindow downw = new DownWindow();
        Message msg = new Message();
         bool islaunch;
        public static bool lostassets;
        private async void Btn_启动_Click(object sender, RoutedEventArgs e)
        {
            if (设置框.IsVisible)
            {
                Nosave();
                return;
            }
            Btn_启动.IsEnabled = false;
            runts.Text = "正在启动中";
            runts.Visibility = Visibility.Visible;
            if (ListVersions.Items.Count == 0)
            {
                await this.ShowMessageAsync("无法启动游戏", "没有读取到游戏版本", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                Btn_启动.IsEnabled = true;
                runts.Text = "";
                load.Visibility = Visibility.Hidden;
                return;
            }
            
            if (DownWindow.bgWorker.IsBusy)
            {
                //bgWorker.IsBusy await this.ShowMessageAsync("正在下载", "正在下载资源文件", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                // new downWindow { Owner = this }.ShowDialog();
                //downWindow.Showdown();
                downw.ShowDialog();
                //this.Hide();
                //down.Show();
            }
            else
            {
                runts.Text = "检查支持库";
                lostFlie.Clear();//清空列表，防止重新启动后，列表重复
                ver = (Version)ListVersions.SelectedItem;
                core = LauncherCore.Create();
                var version = ver;//设置读取的版本配置为当前选择的项


                var jarid = ver.JarId;
               
          //      MessageBox.Show(core.GetVersionJsonPath(jarid));
                if (!File.Exists(core.GetVersionJsonPath(jarid)))
                {
                    Ts("无法启动游戏", "游戏核心不存在,请确认已选择正确的核心版本!");
                    runts.Visibility = Visibility.Hidden;
                    Btn_启动.Content = "启动游戏";
                    load.Visibility = Visibility.Hidden;
                    return;
                }
                try
                {
                    var libs = version.Libraries.Select(lib => core.GetLibPath(lib));

                    var natives = version.Natives.Select(native => core.GetNativePath(native));

                    foreach (string libflie in libs)
                    {
                        if (!File.Exists(libflie))
                        {
                            lostFlie.Add(libflie);
                            //  MessageBox.Show("当前缺少的Libraries文件有：" + file);
                        }
                    }
                    foreach (string libflie in natives)
                    {
                        if (!File.Exists(libflie))
                        {
                            lostFlie.Add(libflie);
                            //MessageBox.Show("当前缺少的Libraries-natives文件有：" + file);
                        }
                    }
                    if (lostFlie.Count != 0)
                    {
                        //返回一个值，中止继续执行后面的代码
                        new DownWindow { Owner = this }.ShowDialog();
                    }
                }
                catch
                {
                    Ts("无法启动游戏", "版本信息获取失败,无法读取所需的支持库,请确认已选择正确的核心版本!");
                    runts.Visibility = Visibility.Hidden;
                    Btn_启动.Content = "启动游戏";
                    load.Visibility = Visibility.Hidden;
                    return;
                }
                lostFlie.Clear();
                lostasset.Clear();
                lostlegacyasset.Clear();
                runts.Text = "检查资源文件";
                if (!Directory.Exists(rundir + ".minecraft/assets/indexes"))
                {
                    Directory.CreateDirectory(rundir + ".minecraft/assets/indexes");//创建文件夹
                }
                if (!Directory.Exists(rundir + ".minecraft/assets/objects"))
                {
                    Directory.CreateDirectory(rundir + ".minecraft/assets/objects");//创建文件夹
                }
               
                var jsondir = core.GetVersionJsonPath(jarid);
                string json = File.ReadAllText(jsondir);
                // MessageBox.Show(core.VersionLocator);
                JsonData data = JsonMapper.ToObject(json);
                string type;
                string url;
                try
                {
                    type = data["assetIndex"]["id"].ToString();
                    url = data["assetIndex"]["url"].ToString().Replace("https://launchermeta.mojang.com", "http://download.mcbbs.net");
                    if (type != "legacy")
                    {

                        string jsonindex;

                        if (File.Exists(rundir + @".minecraft\assets\indexes\" + type + ".json"))
                        {
                            jsonindex = File.ReadAllText(rundir + @".minecraft\assets\indexes\" + type + ".json");
                        }
                        else
                        {
                            jsonindex = Beelogin.GetGeneralContent(url);
                            File.WriteAllText(rundir + @".minecraft\assets\indexes\" + type + ".json", jsonindex, Encoding.Default);
                        }
                        JsonData jdata = JsonMapper.ToObject(jsonindex);
                        jdata = jdata["objects"];
                        string hash;
                        string two;
                        string lj;
                        for (int i = 0; i < jdata.Count; i++)
                        {
                            hash = jdata[i]["hash"].ToString();
                            two = hash.Substring(0, 2);
                            lj = two + "\\" + hash;
                            if (!File.Exists(rundir + @".minecraft\assets\objects\" + lj))
                            {
                                lostasset.Add(hash);
                            }
                        }
                    }
                    else if (type == "legacy")
                    {

                        string jsonindex;
                        if (File.Exists(rundir + @".minecraft\assets\indexes\legacy.json")) { jsonindex = File.ReadAllText(rundir + ".minecraft/assets/indexes/legacy.json"); }
                        else
                        {
                            jsonindex = Beelogin.GetGeneralContent(url);
                        }
                        File.WriteAllText(rundir + @".minecraft\assets\indexes\legacy.json", jsonindex, Encoding.Default);
                        JObject jdata = (JObject)JsonConvert.DeserializeObject(jsonindex);
                        jdata = (JObject)jdata["objects"];
                        //  jdata = jdata["objects"];
                        foreach (var name in jdata)
                        {
                            //MessageBox.Show(rundir + @".minecraft\assets\" + name.Key);
                            if (!File.Exists(rundir + @".minecraft\assets\" + name.Key))
                            {
                                lostlegacyasset.Add(name.Key.Replace("/", "\\"));
                            }
                        }


                    }
                }
                catch
                {
                    type = data["assets"].ToString();
                    url = "null";
                    if (type != "legacy")
                    {

                        string jsonindex;

                        if (File.Exists(rundir + @".minecraft\assets\indexes\" + type + ".json"))
                        {
                            jsonindex = File.ReadAllText(rundir + @".minecraft\assets\indexes\" + type + ".json");
                            JsonData jdata = JsonMapper.ToObject(jsonindex);
                            jdata = jdata["objects"];
                            string hash;
                            string two;
                            string lj;
                            for (int i = 0; i < jdata.Count; i++)
                            {
                                hash = jdata[i]["hash"].ToString();
                                two = hash.Substring(0, 2);
                                lj = two + "\\" + hash;
                                if (!File.Exists(rundir + @".minecraft\assets\objects\" + lj))
                                {
                                    lostasset.Add(hash);
                                }
                            }
                            lostassets = false;
                        }
                        else
                        {
                            lostassets = true;
                        }
                       
                    }
                    else if (type == "legacy")
                    {

                        string jsonindex;
                        if (File.Exists(rundir + @".minecraft\assets\indexes\legacy.json")) {
                            jsonindex = File.ReadAllText(rundir + ".minecraft/assets/indexes/legacy.json");
                            File.WriteAllText(rundir + @".minecraft\assets\indexes\legacy.json", jsonindex, Encoding.Default);
                            JObject jdata = (JObject)JsonConvert.DeserializeObject(jsonindex);
                            jdata = (JObject)jdata["objects"];
                            //  jdata = jdata["objects"];
                            foreach (var name in jdata)
                            {
                                //MessageBox.Show(rundir + @".minecraft\assets\" + name.Key);
                                if (!File.Exists(rundir + @".minecraft\assets\" + name.Key))
                                {
                                    lostlegacyasset.Add(name.Key.Replace("/", "\\"));
                                }
                            }
                            lostassets = false;
                        }
                        else
                        {
                            lostassets = true;
                        }
                    }
                }

                try
                {

                   
                    if (lostasset.Count != 0 || lostlegacyasset.Count != 0)
                    {

                        if (url == "null")
                        {
                            await this.ShowMessageAsync("资源文件缺失", "发现缺少了" + (lostasset.Count != 0 ? lostasset.Count.ToString() : lostlegacyasset.Count != 0 ? lostlegacyasset.Count.ToString() : "") + "个资源文件,这可能导致游戏声音丢失\n但您的游戏核心版本较旧,不能自动补全.\n建议前往下载游戏窗口下载新版本客户端!", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                         //   Ts("资源文件缺失", "发现缺少了" + (lostasset.Count != 0 ? lostasset.Count.ToString() : lostlegacyasset.Count != 0 ? lostlegacyasset.Count.ToString() : "") + "个资源文件,但您的游戏核心版本较旧,不能自动补全.\n建议前往下载游戏窗口下载新版本客户端!");
                    }
                        else {
                            // new downWindow { Owner = this }.ShowDialog();

                            downw.Owner = this;
                            downw.ShowDialog();
                        }
                    }
                }
                catch
                {
                    //ts("错误", "尝试检查资源文件时失败,建议前往下载游戏窗口下载新版本客户端!");
                    lostassets = true;

                    // await this.ShowMessageAsync("错误", "尝试检查资源文件时失败,建议前往下载游戏窗口下载新版本客户端!", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                }

            }
          

            if (!bgWorker.IsBusy)
            {
                //MessageBox.Show("1");
                // bgWorker.CancelAsync();
                //bgWorker.Dispose();
                if (islaunch) {
                    bgWorker.RunWorkerAsync();
                } else {
                    bgWorker.WorkerReportsProgress = true;
                    bgWorker.ProgressChanged += new ProgressChangedEventHandler(Bw_ProgressChanged);
                    bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Bw_RunWorkerCompleted);
                    bgWorker.DoWork += new DoWorkEventHandler(Bw_DoWork1);
                    bgWorker.RunWorkerAsync();
                    islaunch = true;
                }

                
            }
        }
        void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //这时后台线程已经完成，并返回了主线程，所以可以直接使用UI控件了 

            if (islaunch)
            {
                runts.Text = "正在启动中";
                SetReportLevel(ReportLevel.None);
                Config.LastVersion = ver.Id;
                LaunchOptions Options = new LaunchOptions();
                
                var result = App.Core.Launch(new LaunchOptions
                {
                    Version = ver, //Ver为Versions里你要启动的版本名字
                    MaxMemory = Config.MaxMemory, //最大内存，int类型
                    Authenticator = (Config.Authenticator == true)
                        ? new YggdrasilLogin(Config.UserName, Config.Password, true)
                        : ((IAuthenticator)new OfflineAuthenticator(Config.UserName)),
                    Mode = (Config.LaunchMode == true) ? null : ((Config.LaunchMode == false) ? LaunchMode.MCLauncher : null),
                    VersionType = "BeeLauncher",
                    Server = new ServerInfo { Address = Config.ip, Port = (!string.IsNullOrWhiteSpace(Config.port) ? ushort.Parse(Config.port) : ushort.Parse("25565")) }
                    //ushort.Parse(Config.port) 
                    
                }, x => { });
                //Server = new ServerInfo { Address = "服务器IP地址", Port = "服务器端口" }, //设置启动游戏后，自动加入指定IP的服务器，可以不要
                // Size = new WindowSize { Height = 768, Width = 1280 } //设置窗口大小，可以不要



                if (!result.Success)
                {
                    //MessageBox.Show(result.ErrorMessage, result.ErrorType.ToString());
                    runts.Visibility = Visibility.Hidden;
                    Btn_启动.Content = "启动游戏";
                    load.Visibility = Visibility.Hidden;
                    Btn_启动.IsEnabled = true;
                    Show();
                    switch (result.ErrorType)
                    {
                        case ErrorType.NoJAVA:
                            this.ShowMessageAsync("游戏启动失败", "您系统的Java有异常,请尝试重新安装\n详细信息：" + result.ErrorMessage, MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                            break;
                        case ErrorType.AuthenticationFailed:
                            this.ShowMessageAsync("正版验证失败", "请检查你的正版账号密码,如不了解,请在设置取消勾选正版登陆\n详细信息：" + result.ErrorMessage, MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                            break;
                        case ErrorType.UncompressingFailed:
                            this.ShowMessageAsync("游戏启动失败", "游戏多开或文件损坏，请确认文件完整且不要多开游戏\n详细信息：" + result.ErrorMessage, MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                            break;
                        default:
                            this.ShowMessageAsync("启动错误", "请将此窗口截图并反馈给作者,对此带来不便深表歉意\n详细信息：" + result.ErrorMessage + "\n" + (result.Exception == null ? string.Empty : result.Exception.StackTrace), MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                            break;

                    }
                }
                else
                {
                    new Message{ Owner = this  }.Show();
                   
                    Btn_启动.Content = "启动游戏";
                    load.Visibility = Visibility.Hidden;
                    runts.Visibility = Visibility.Hidden;
                    Btn_启动.IsEnabled = true;
                    Hide();
                    //  this.ShowMessageAsync("游戏正在启动", "出现无响应等情况是正常现象,请耐心等待.\n五秒后自动关闭.", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });

                }
            }
        }
        private void Bw_ProgressChanged(object sender, ProgressChangedEventArgs e)

        {

            //  jdt.Value = e.ProgressPercentage;
            var jd = e.ProgressPercentage;
            if (jd == 233)
            {
                runts.Visibility = Visibility.Hidden;
                Btn_启动.Content = "启动游戏";
                load.Visibility = Visibility.Hidden;
                Btn_启动.IsEnabled = true;
                return;
            }
            else if (jd == 2) { runts.Text = "正在登录"; }

        }

        void Bw_DoWork1(object sender, DoWorkEventArgs e)
        {
            if (Config.Beelogin)
            {
                bgWorker.ReportProgress(2);
                if (!Config.Beemod)
                {
                    string fh = Beelogin.Bee_login(Config.beeurl, Config.UserName, Config.Beepsw, ip);
                    if (fh != "yes")
                    {
                        Ts("登录失败", "详细信息:" + fh);
                        bgWorker.ReportProgress(233);
                        return;
                    }
                }
                else
                {
                    var jsondir = core.GetVersionJsonPath(ver.JarId);
                    string tdir = Beelogin.GetLeft(jsondir, "\\") + "\\config";
                    if (!Directory.Exists(tdir))
                    {
                        Directory.CreateDirectory(tdir);//创建文件夹
                    }
                    string fh = Beelogin.Beemodlogin_token(Config.beeurl, Config.UserName, Config.Beepsw, (Config.LaunchMode ? rundir + @".minecraft\config\BeeLogin.cfg" : tdir + "\\BeeLogin.cfg"), ip);
                    if (fh != "yes")
                    {
                        Ts("登录失败", "详细信息:" + fh);
                        bgWorker.ReportProgress(233);
                    }
                }
            }
        }
        private void Nosave()
        {
            this.ShowMessageAsync("设置未保存", "请先保存设置.", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
        }

        private void Btn_save_Click(object sender, RoutedEventArgs e)
        {
            if (textBox_name.Text == "")
            {
                this.ShowMessageAsync("请填写游戏名", "游戏名不能为空,请填写完整.", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                TextBoxHelper.SetIsWaitingForData(textBox_name, true);
                return;
            }
            Config.UserName = textBox_name.Text;
            if(CheckBox_zhengban.IsChecked == true)
            {
                if (passbox_zhengban.Password == "")
                {
                    this.ShowMessageAsync("请填写密码", "正版密码不能为空,请填写完整,如无须正版登录,请取消勾选", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                    return;
                }
                Config.Authenticator = true;
                Config.Password = passbox_zhengban.Password;
            }
            else 
            {
                Config.Authenticator = false;
                Config.Password = null;
            }
            if (!int.TryParse(textBox_neicun.Text, out int maxMem))
            {
                this.ShowMessageAsync("请填写最大内存", "最大内存不能为空,请填写完整或勾选自动", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                return;
            }
            if (File.Exists(textBox_java.Text))
            {
                Config.JavPath = textBox_java.Text;
            }
            else
            {
                this.ShowMessageAsync("JAVA路径错误", "自动读取的Java不存在,请手动指定", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                return;
            }
            if (CheckBox_beelogin.IsChecked == true)
            {
                if (Config.beeurl == null)
                {
                    this.ShowMessageAsync("无法使用BeeLogin", "未设置URL,请在配置文件内设置", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                    return;
                }
                else if (passbox_beelogin.Password == null)
                {
                    this.ShowMessageAsync("请填写密码", "登录密码不能为空,请填写完整,如无须蜜蜂登录,请取消勾选", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                    return;
                }
                Config.Beelogin = true;
                Config.Beepsw = passbox_beelogin.Password;
            }
            else 
            {
                Config.Beelogin = false;
                Config.Beepsw = null;
            }

            
            if (CheckBox_neicun.IsChecked == true)
            {
                Config.AutoMemory = true;
            }
            else
            {
                Config.AutoMemory = false;
            }
            Config.MaxMemory = maxMem;
            Config.ip = textBox_canshu.Text;
            Config.port = textBox_port.Text;
            if (rbtn1.IsChecked == true)
            {
                Config.LaunchMode = true;
            }
            else
            {
                Config.LaunchMode = false;
            }

            设置框.Visibility = Visibility.Hidden;
        }
        private void CheckBox_zhengban_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBox_zhengban.IsChecked == true)
            {
                passbox_zhengban.IsEnabled = true;

            }
            else
            {
                passbox_zhengban.IsEnabled = false;
                passbox_zhengban.Clear();
            }
        }

        private void TextBox_name_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBox_name.Text != "")
            {
                TextBoxHelper.SetIsWaitingForData(textBox_name, false);
            }
        }

        private void CheckBox_beelogin_Click(object sender, RoutedEventArgs e)
        { 
            if (CheckBox_beelogin.IsChecked == true)
            {
              
                if (string.IsNullOrWhiteSpace(Config.beeurl)) {
                    this.ShowMessageAsync("无法启用蜜蜂登录", "未配置蜜蜂登录地址,请在BeeLauncher.cfg中设置", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                    CheckBox_beelogin.IsChecked = false;
                } else {
                    textblpsw.Visibility = Visibility.Visible;
                    passbox_beelogin.Visibility = Visibility.Visible;
                    CheckBox_mod.Visibility = Visibility.Visible;
                    passbox_beelogin.IsEnabled = true;
                    Btn_zhuce.Visibility = Visibility.Visible;
                }
            } else {
                textblpsw.Visibility = Visibility.Hidden;
                passbox_beelogin.Visibility = Visibility.Hidden;
                CheckBox_mod.Visibility = Visibility.Hidden;
                Btn_zhuce.Visibility = Visibility.Hidden;
            }
        }

        private void CheckBox_neicun_Checked(object sender, RoutedEventArgs e)
        {
            if (CheckBox_neicun.IsChecked == true)
            {
                textBox_neicun.IsEnabled = false;

                Autonc();
            }
            else
            {
                textBox_neicun.IsEnabled = true;

            }
        }

        private void Btn_zhuce_Click(object sender, RoutedEventArgs e)
        {
            new Register { Owner = this }.ShowDialog();
        }

        private void Btn_findjava_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                textBox_java.Text = KMCCC.Tools.SystemTools.FindJava().Last();
            }
            catch
            {
                textBox_java.Text = "未获取到Java,请手动指定";
            }
        }

        private void Btn_delpass_Click(object sender, RoutedEventArgs e)
        {
            passbox_zhengban.Password = "";
            Config.Password = "";
            Config.Beepsw = "";
            passbox_zhengban.IsEnabled = false;
            CheckBox_zhengban.IsChecked = false;
            Config.Authenticator = false;
            this.ShowMessageAsync("密码信息已删除", "如需删除所有信息,请删除目录下BeeLauncher.cfg", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
        }

        private void Btn_下载_Click(object sender, RoutedEventArgs e)
        {
            if (设置框.Visibility == Visibility.Visible)
            {
                Nosave();
                return;
            }
            new PackWindow { Owner = this }.ShowDialog();
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            try {

            if (ListVersions.Text != "")
            {
                    //    ver = (Version)ListVersions.SelectedItem;
                    //   core = LauncherCore.Create();
                    //   var dir = core.GetVersionRootPath(ver.JarId);
                    string dir = rundir + ".minecraft\\versions\\" + ListVersions.Text;
                    //    MessageBox.Show(dir);

                    if (!Directory.Exists(dir))
                    {
                         dir = rundir + ".minecraft\\versions\\";
                    }
                    Process.Start("explorer.exe", dir);
            }
            else
            {
                    //  string path = rundir + ".minecraft\\versions\\";
                    this.ShowMessageAsync("打开目录失败", "没有选择要打开的版本", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                }
            }
            catch {
                this.ShowMessageAsync("打开目录失败", "BeeLauncher无法打开目录,有可能是被杀毒软件拦截。", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
            }
        }

        private void CheckBox_beijing_Checked(object sender, RoutedEventArgs e)
        {
            if (CheckBox_beijing.IsChecked == true)
            {
                string dir = rundir + "启动器背景";
                if (!Directory.Exists(dir))
                {

                    Directory.CreateDirectory(dir);
                    File.Create(dir + "\\请把背景图放在此目录");
                    Process.Start("explorer.exe", dir);
                }
                Config.bg = true;
            }
            else
            {
                Config.bg = false;
            }
        }

        private void Bg(object sender, RoutedEventArgs e)
        {
            //背景
            if (Config.bg)
            {
                string dir = rundir + "启动器背景";
                if (Directory.Exists(dir))
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
                else
                {
                    Directory.CreateDirectory(dir);
                }
            }
        }


        private async void DialogsBeforeExit()
        {
            MessageDialogResult result = await this.ShowMessageAsync("您还未保存设置", "您要保存设置吗?", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() { AffirmativeButtonText = "保存退出", NegativeButtonText = "不保存退出" });
            //  await this.ShowMessageAsync("标题", "内容", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
            if (result == MessageDialogResult.Negative)
            {

            }
            else//确认退出
            {
                // Btn_save_Click();

                Btn_save.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent, Btn_save));

                //系统退出需要修改的
            }
            Application.Current.Shutdown();
        }


        private MetroWindow accentThemeTestWindow;

        private void Btn_color_Click(object sender, RoutedEventArgs e)
        {
            if (accentThemeTestWindow != null)
            {
                accentThemeTestWindow.Activate();
                return;
            }

            accentThemeTestWindow = new AccentStyleWindow()
            {
                Owner = this
            };
            accentThemeTestWindow.Closed += (o, args) => accentThemeTestWindow = null;
            accentThemeTestWindow.Left = Left + ActualWidth / 2.0;
            accentThemeTestWindow.Top = Top + ActualHeight / 2.0;
            accentThemeTestWindow.Show();
        }

        private void Numonly(object sender, TextChangedEventArgs e)
        {
            //屏蔽中文输入和非法字符粘贴输入
            System.Windows.Controls.TextBox textBox = sender as System.Windows.Controls.TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);

            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                if (!Double.TryParse(textBox.Text, out double num))
                {
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        private void CheckBox_mod_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBox_mod.IsChecked == true)
            {
                Config.Beemod = true;
            }
            else
            {
                Config.Beemod = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (accentThemeTestWindow != null)
            {
                accentThemeTestWindow.Activate();
                return;
            }

            accentThemeTestWindow = new AccentStyleWindow()
            {
                Owner = this
            };
            accentThemeTestWindow.Closed += (o, args) => accentThemeTestWindow = null;
            accentThemeTestWindow.Left = Left + ActualWidth / 2.0;
            accentThemeTestWindow.Top = Top + ActualHeight / 2.0;
            accentThemeTestWindow.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog()
            {
                InitialDirectory = textjava.Text,
                Filter = "选择java|javaw.exe|调试模式|java.exe",
                FilterIndex = 1,
                RestoreDirectory = true
            };
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox_java.Text = openFileDialog1.FileName;
              //  MessageBox.Show(openFileDialog1.FileName);
            }
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
          //  if (设置框.Visibility == Visibility.Visible)
          //  {
           //     e.Cancel = true;
           //     CancellationToken token;
           //     TaskScheduler uiSched = TaskScheduler.FromCurrentSynchronizationContext();
           //     Task.Factory.StartNew(DialogsBeforeExit, token, TaskCreationOptions.None, uiSched);
           // }
            Application.Current.Shutdown();
        }
    }
}


