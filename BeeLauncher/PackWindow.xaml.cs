namespace BeeLauncher
{
    #region
    using MahApps.Metro.Controls;
    using System.IO;
    using System.Windows.Input;
    using Newtonsoft.Json.Linq;
    using System.Collections.ObjectModel;
    using System;
    using System.Windows;
    using MahApps.Metro.Controls.Dialogs;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    //   using ICSharpCode.SharpZipLib.Zip;
    using System.ComponentModel;
    using System.Windows.Controls;
    using System.Windows.Data;
    using LitJson;
    using System.Text;

    using Ionic.Zip;
    #endregion
    /// <summary>
    /// down.xaml 的交互逻辑
    /// </summary>
    public partial class PackWindow : MetroWindow
    {
        public static List<string> verFlie = new List<string>();
        public static List<string> verdir = new List<string>();
        BackgroundWorker bgWorker = new BackgroundWorker();
        public PackWindow()
        {
            InitializeComponent();
         //   getkhd();
        }
        public class Minecraft { public string 游戏版本 { get; set; } }
    
    
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Ts(string bt,string wb)
        {

            //  Dispatcher.Invoke(new Action(() ={ this.ShowMessageAsync(bt, wb) }));
            Dispatcher.Invoke(new Action(() => { this.ShowMessageAsync(bt, wb); }));
        }
        void Bw_DoWork1(object sender, DoWorkEventArgs e)
        {
            if (!e.Cancel) {
                //load.Dispatcher.Invoke(new Action(() => { load.Visibility = Visibility;  }));
                // btn_下载.Dispatcher.Invoke(new Action(() => { btn_下载.Content = "正在下载"; }));
                string dir_name = @AppDomain.CurrentDomain.BaseDirectory;
                // string select = "";
                // Dispatcher.Invoke(new Action(() => { select = listView.SelectedItem.ToString(); }));
                Khd khd = null;
                Dispatcher.Invoke(new Action(() => { khd = listView.SelectedItem as Khd; }));
                string select = khd.Ver;//forge版本号
                try
            {

                    //MessageBox.Show(File.Exists(dir_name + @"\.minecraft\versions\" + select + "\\" + select + ".json").ToString());

                    if (File.Exists(dir_name + @"\.minecraft\versions\" + select + "\\" + select + ".json"))
                    {
                        Ts("文件已存在", "您要下载的客户端已存在,无需下载.");
                        e.Cancel = true;
                        return;
                    }
                    if (!Directory.Exists(dir_name + "/.minecraft/versions/" + select))
                {
                  
                    Directory.CreateDirectory(dir_name + "/.minecraft/versions/" + select);//创建文件夹
                }
                    string jarurl = "http://download.mcbbs.net/versions/" + select + "/client";
                    string jsonurl = "http://download.mcbbs.net/versions/" + select +"/json";
                //   MessageBox.Show(GetHTML(jsonurl));
                    if (Beelogin.DownloadFile(jarurl, dir_name + " /.minecraft/versions/" + select + "/" + select + ".jar") == true)
                    {
                        string json = Beelogin.GetGeneralContent(jsonurl);
                        if (json != "")
                        {
                         File.WriteAllText(dir_name + " /.minecraft/versions/" + select + "/" + select + ".json", json, Encoding.Default);
                         Ts("下载完成", select + "版本的客户端下载完成!");
                        e.Cancel = true;
                        return;
                    }
                    else
                    {
                        Ts("下载失败", select + "版本的客户端下载失败,请尝试下载其他版本\r\n出错URL:" + jsonurl);
                            Directory.Delete(dir_name + "/.minecraft/versions/" + select, true);
                            e.Cancel = true;
                        return;
                    }

                }
                else
                {
                    Ts("下载失败", select + "版本的客户端下载失败,请尝试下载其他版本\r\n出错URL:" + jarurl);
                        Directory.Delete(dir_name + "/.minecraft/versions/" + select, true);
                        e.Cancel = true;
                    return;
                }
            }
            catch
            {
               
                    Ts("下载失败", "客户端下载失败,请重试或下载其他版本");
                    Directory.Delete(dir_name + "/.minecraft/versions/" + select, true);
                    e.Cancel = true;
                    return;
                
            }
            }
        }
        void Bw_DoWork2(object sender, DoWorkEventArgs e)
        {

            if (!e.Cancel)
            {
                string dir_name = @AppDomain.CurrentDomain.BaseDirectory;
                Forge forge = null;
                Dispatcher.Invoke(new Action(() => { forge = forgeView.SelectedItem as Forge; }));
                try
                {
                    if (forge != null && forge is Forge)
                    {
                        string var_name = forge.Ver;//游戏版本号
                        string var_forge = forge.Id;//forge版本号http://download.mcbbs.net/versions/
                        string fz = forge.Fz;//forge分支
                     
                        if (File.Exists(dir_name + @"\.minecraft\versions\" + var_name + "-" + "forge" + var_name + "-" + var_forge+"\\" + var_name + "-" + "forge" + var_name + "-" + var_forge+ ".json"))
                        {
                            Ts("文件已存在", "您要下载的客户端已存在,无需下载.");
                            e.Cancel = true;
                            return;
                        }
                        if (!File.Exists(dir_name + @"\.minecraft\versions\" + var_name + "\\" + var_name + ".json") || !File.Exists(dir_name + @"\.minecraft\versions\" + var_name + "\\" + var_name + ".jar"))
                        {
                            if (!Directory.Exists(dir_name + @"\.minecraft\versions\" + var_name))
                            {
                                Directory.CreateDirectory(dir_name + @"\.minecraft\versions\" + var_name);//创建文件夹
                            }
                            string jarurl = "http://download.mcbbs.net/versions/" + var_name + "/client";
                            string jsonurl = "http://download.mcbbs.net/versions/" + var_name + "/json";
                            if (Beelogin.DownloadFile(jarurl, dir_name + @"\.minecraft\versions\" + var_name + "\\" + var_name + ".jar") == true)
                            {
                                if (Beelogin.DownloadFile(jsonurl, dir_name + @"\.minecraft\versions\" + var_name + "\\" + var_name + ".json") == false)
                                {
                                    Ts("下载失败", var_name + "版本的客户端下载失败,不能继续下载FORGE\r\n出错URL:" + jsonurl);
                                    Directory.Delete(dir_name + @"\.minecraft\versions\" + var_name, true);
                                    e.Cancel = true;
                                    return;
                                }
                            }
                            else
                            {
                                Ts("下载失败", var_name + "版本的客户端下载失败,不能继续下载FORGE\r\n出错URL:" + jarurl);
                                Directory.Delete(dir_name + @"\.minecraft\versions\" + var_name, true);
                                e.Cancel = true;
                                return;
                            }
                        }

                                              // string forge_link = "http://download.mcbbs.net/forge/download?mcversion=" + var_name + "&version=" + var_forge + "&branch=" + var_name + "&category=universal&format=jar";
                        string forge_link = $"http://download.mcbbs.net/maven/net/minecraftforge/forge/{var_name}-{var_forge}" + (fz != "" ? $"-{fz}" : "") + $"/forge-{var_name}-{var_forge}" + (fz != "" ? $"-{fz}" : "") + $"-universal.jar" ;
                        string forgedir = dir_name + @".minecraft\libraries\net\minecraftforge\forge\" + var_name + "-" + var_forge + (fz != "" ? $"-{fz}" : "") + "\\" + "forge-" + var_name + "-" + var_forge + (fz != "" ? $"-{fz}" : "") + ".jar";
                        if (!Directory.Exists(dir_name + @".minecraft\libraries\net\minecraftforge\forge\" + var_name + "-" + var_forge + (fz != "" ? $"-{fz}" : "")))
                        {
                            Directory.CreateDirectory(dir_name + @".minecraft\libraries\net\minecraftforge\forge\" + var_name + "-" + var_forge + (fz != "" ? $"-{fz}" : ""));
                        }
                        if (!Directory.Exists(dir_name + @".minecraft\versions\" + var_name + "-" + "forge" + var_name + "-" + var_forge))
                        {
                            Directory.CreateDirectory(dir_name + @".minecraft\versions\" + var_name + "-" + "forge" + var_name + "-" + var_forge);
                        }
                        if (!Directory.Exists(dir_name + @".minecraft\beelanuch_downtemp"))
                        {
                            Directory.CreateDirectory(dir_name + @".minecraft\beelanuch_downtemp");
                        }
                        Console.WriteLine(forge_link);
                    
                        if (Beelogin.DownloadFile(forge_link, forgedir))
                        {
                            //      MessageBox.Show(forgedir);
                            //      KMCCC.Tools.ZipTools.UnzipFile(dir_name + @".minecraft\libraries\net\minecraftforge\forge\" + var_name + "-" + var_forge + "\\" + "forge-" + var_name + "-" + var_forge + ".jar", dir_name + @".minecraft\beelanuch_downtemp", null);
                            if (!UnZip(forgedir, dir_name + @".minecraft\beelanuch_downtemp\", null)) {
                                Ts("解压失败", "解压'" + var_name + "'版本的forge'" + var_forge + "'失败,请重试或下载其他版本");
                                //   Directory.Delete(dir_name + @".minecraft\versions\" + var_name + "-" + "forge" + var_name + "-" + var_forge, true);
                                e.Cancel = true;
                                return;
                            }
                            
                            //   using (ZipFile zip = new ZipFile(forgedir))
                            //    {
                            // MessageBox.Show("1");
                            //        zip.ExtractAll(dir_name + @"\.minecraft\beelanuch_downtemp", ExtractExistingFileAction.OverwriteSilently);
                            //     }

                            File.Move(dir_name + @".minecraft\beelanuch_downtemp\version.json", dir_name + @".minecraft\versions\" + var_name + "-" + "forge" + var_name + "-" + var_forge + @"\" + var_name + "-" + "forge" + var_name + "-" + var_forge + ".json");
                            //MessageBox.Show("2");
                            Directory.Delete(dir_name + @".minecraft\beelanuch_downtemp", true);
                            Ts("下载成功!", "'" + var_name + "'版本的forge'" + var_forge + "'已下载完毕");
                            e.Cancel = true;
                            return;
                        }
                        else
                        {

                            Ts("下载失败", "下载'" + var_name + "'版本的forge'" + var_forge + "'失败,请重试或下载其他版本\r\n出错URL:"+ forge_link);
                         //   Directory.Delete(dir_name + @".minecraft\versions\" + var_name + "-" + "forge" + var_name + "-" + var_forge, true);
                            e.Cancel = true;
                            return;
                        }
                    }
                }
                catch
                {
                    Ts("下载失败", "下载失败,请重试或下载其他版本");
                    e.Cancel = true;
                    return;
                }
            }
        }
    

            void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //这时后台线程已经完成，并返回了主线程，所以可以直接使用UI控件了 
            if (load.Visibility == Visibility.Visible)
            {
                bgWorker.Dispose();
                load.Visibility = Visibility.Hidden;
                btn_下载.Content = "下载";
                rbtn1.IsEnabled = true;
                rbtn2.IsEnabled = true;
                rbtn3.IsEnabled = true;
                rbtn4.IsEnabled = true;
                MainWindow.main.Getver();
            }
        }
        private  void Btn_确定_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItems.Count != 0 || forgeView.SelectedItems.Count != 0 || LiteloaderView.SelectedItems.Count != 0 || OptifineView.SelectedItems.Count != 0)
            {
                load.Visibility = Visibility.Visible;
                btn_下载.Content = "正在下载";
                rbtn1.IsEnabled = false;
                rbtn2.IsEnabled = false;
                rbtn3.IsEnabled = false;
                rbtn4.IsEnabled = false;
                if (!bgWorker.IsBusy)
                {
                    bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Bw_RunWorkerCompleted);
                    if (rbtn1.IsChecked == true)
                    {
                        bgWorker.DoWork -= new DoWorkEventHandler(Bw_DoWork2);
                        bgWorker.DoWork -= new DoWorkEventHandler(Bw_DoWork3);
                        bgWorker.DoWork -= new DoWorkEventHandler(Bw_DoWork4);
                        bgWorker.DoWork += new DoWorkEventHandler(Bw_DoWork1);
                        bgWorker.RunWorkerAsync();
                    }
                    else if(rbtn2.IsChecked == true)
                    {
                        bgWorker.DoWork -= new DoWorkEventHandler(Bw_DoWork1);
                        bgWorker.DoWork -= new DoWorkEventHandler(Bw_DoWork3);
                        bgWorker.DoWork -= new DoWorkEventHandler(Bw_DoWork4);
                        bgWorker.DoWork += new DoWorkEventHandler(Bw_DoWork2);
                        bgWorker.RunWorkerAsync();
                    }
                    else if (rbtn3.IsChecked == true)
                    {
                        bgWorker.DoWork -= new DoWorkEventHandler(Bw_DoWork1);
                        bgWorker.DoWork -= new DoWorkEventHandler(Bw_DoWork2);
                        bgWorker.DoWork -= new DoWorkEventHandler(Bw_DoWork4);
                        bgWorker.DoWork += new DoWorkEventHandler(Bw_DoWork3);
                        bgWorker.RunWorkerAsync();
                    }
                    else if (rbtn4.IsChecked == true)
                    {
                        bgWorker.DoWork -= new DoWorkEventHandler(Bw_DoWork1);
                        bgWorker.DoWork -= new DoWorkEventHandler(Bw_DoWork2);
                        bgWorker.DoWork -= new DoWorkEventHandler(Bw_DoWork3);
                        bgWorker.DoWork += new DoWorkEventHandler(Bw_DoWork4);
                        bgWorker.RunWorkerAsync();
                    }
                }
         }
            else
            {
                Ts("不能开始下载", "您还未选择需要下载的项目");
            }
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

        public ObservableCollection<object> khdobj;
        public ObservableCollection<object> forgeobj;
        public ObservableCollection<object> Liteloaderobj;
        public ObservableCollection<object> Optifineobj;
        public class Optifine
        {
            public string Ver { get; set; }
            public string Zt { get; set; }
            public string Id { get; set; }
        }
        public class Forge
        {
            public string Ver { get; set; }
            public string Zt { get; set; }
            public string Id { get; set; }
            public string Time { get; set; }
            public string Fz { get; set; }
        }
        public class Liteloader
        {
            public string Ver { get; set; }
            public string Zt { get; set; }
            public string Id { get; set; }
        }
        public class Khd
        {
            public string Ver { get; set; }
            public string Type { get; set; }
            public string Time { get; set; }
        }

        private void Getkhd()
        {
            listView.Items.Clear();
            try
            {
                string verjson = Beelogin.GetGeneralContent("http://download.mcbbs.net/mc/game/version_manifest.json");
                JsonData json = JsonMapper.ToObject(verjson);
                json = json["versions"];
                khdobj = new ObservableCollection<object>();
                for (int i = 0; i < json.Count; i++)
                {
                    // MessageBox.Show(json[i]["id"].ToString());(fz != "" ? $"-{fz}" : "")
                    string type = json[i]["type"].ToString();
                    khdobj.Add(new Khd { Ver = json[i]["id"].ToString(), Type= (type == "release" ? "正式版-release" : (type == "snapshot" ? "快照版-snapshot" : type)), Time = json[i]["releaseTime"].ToString() });
                    //VarList_1.Add(new minecraft() { 游戏版本 = json[i].ToString() });
                    //    listView.Items.Add(data["id"][i].ToString());
                }
                //((FindName("listver")) as DataGrid).ItemsSource = VarList_1;
                //((FindName("listview")) as ListView).ItemsSource = VarList_1;
                //

                listView.DataContext = khdobj;
            }
            catch
            {
                //   this.ShowMessageAsync("链接错误", "目前无法连接网络,或服务器正忙,请稍候再试");
                listView.Items.Add("链接错误");
                btn_下载.IsEnabled = false;
            }
            //MessageBox.Show(verjson);
            listView.Visibility = Visibility.Visible;
        }
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is GridViewColumnHeader)
            {
                //Get clicked column
                GridViewColumn clickedColumn = (e.OriginalSource as GridViewColumnHeader).Column;
                if (clickedColumn != null)
                {
                    //Get binding property of clicked column
                    string bindingProperty = (clickedColumn.DisplayMemberBinding as Binding).Path.Path;
                    SortDescriptionCollection sdc = forgeView.Items.SortDescriptions;
                    ListSortDirection sortDirection = ListSortDirection.Ascending;
                    if (sdc.Count > 0)
                    {
                        SortDescription sd = sdc[0];
                        sortDirection = (ListSortDirection)((((int)sd.Direction) + 1) % 2);
                        sdc.Clear();
                    }
                    sdc.Add(new SortDescription(bindingProperty, sortDirection));
                }
            }
        }

        private void Rbtn1_Checked(object sender, RoutedEventArgs e)
        {
            label.Content = "感谢 BMCL 提供下载API,Liteloader和OptiFine需要Forge";
            forgeView.Visibility = Visibility.Hidden;
            LiteloaderView.Visibility = Visibility.Hidden;
            OptifineView.Visibility = Visibility.Hidden;
            btn_下载.IsEnabled = true;
            if (listView.Items.Count < 5)
            {
                Getkhd();
            }
            else
            {
                listView.Visibility = Visibility.Visible;
            }
        }
        private void Rbtn2_Checked(object sender, RoutedEventArgs e)
        {
            label.Content = "感谢 BMCL 提供下载API,Liteloader和OptiFine需要Forge";
            listView.Visibility = Visibility.Hidden;
            LiteloaderView.Visibility = Visibility.Hidden;
            OptifineView.Visibility = Visibility.Hidden;
            btn_下载.IsEnabled = true;
            if (forgeView.Items.Count < 5)
            {
                forgeView.Items.Clear();
              try
             {
                    string verjson = Beelogin.GetGeneralContent("http://download.mcbbs.net/forge/promos");
                    //MessageBox.Show(verjson);
                    JArray json = JArray.Parse(verjson);
                    forgeobj = new ObservableCollection<object>();
                
                    for (int i = 0; i < json.Count; i++)
                    {
                        var ver = json[i]["build"].ToString();
                    System.Diagnostics.Debug.WriteLine(ver);
                        if (ver != "")
                        {
    
                        //   var wj = json[i]["build"]["files"].ToString();
                        //   var wjm = "";
                        // if (json[i]["build"]["files"][1]["category"].ToString() == "universal")
                        //    {
                        //    wjm = json[i]["build"]["files"][1]["format"].ToString();
                        //    }
                        //   else if (json[i]["build"]["files"][2]["category"].ToString() == "universal")
                        //    {
                        //        wjm = json[i]["build"]["files"][2]["format"].ToString();
                        //   }

                        //var wjm = json[i]["build"]["files"][0]["category"].ToString();
                        //MessageBox.Show(wjm);
                        forgeobj.Add(new Forge { Ver = json[i]["build"]["mcversion"].ToString(), Zt = json[i]["name"].ToString(), Id = json[i]["build"]["version"].ToString(), Time = json[i]["build"]["modified"].ToString(), Fz = json[i]["build"]["branch"].ToString() });
                            //VarList_1.Add(new minecraft() { 游戏版本 = json[i].ToString() });
                     //     System.Diagnostics.Debug.WriteLine(json[i]["build"]["mcversion"].ToString(), json[i]["name"].ToString(),  json[i]["build"]["version"].ToString(),  json[i]["build"]["modified"].ToString(),  json[i]["build"]["branch"].ToString());
                            //VarList_2.Add(new minecraft_forge() { 游戏版本 = json_var_arr_2[i]["build"]["mcversion"].ToString(), 构建时间 = json_var_arr_2[i]["build"]["modified"].ToString(), 构建版本号 = json_var_arr_2[i]["build"]["version"].ToString(), 版本名 = json_var_arr_2[i]["name"].ToString() });
                        }
                    }
                    forgeView.DataContext = forgeobj;

                }
                catch
                {
                    btn_下载.IsEnabled = false;
                    this.ShowMessageAsync("链接错误", "目前无法连接网络,或服务器正忙,请稍候再试");
                    //   forgeView.Items.Add("链接错误");
                }
            }
            forgeView.Visibility = Visibility.Visible;
        }
        private void Rbtn3_Checked(object sender, RoutedEventArgs e)
        {
            label.Content = "MOD版Liteloader需要Forge,请确保您已安装Forge";
            listView.Visibility = Visibility.Hidden;
            forgeView.Visibility = Visibility.Hidden;
            OptifineView.Visibility = Visibility.Hidden;
            btn_下载.IsEnabled = true;
            if (LiteloaderView.Items.Count < 5)
            {
                LiteloaderView.Items.Clear();
                try
                {
                    string verjson = Beelogin.GetGeneralContent("http://download.mcbbs.net/liteloader/list");
                    //MessageBox.Show(verjson);
                    JArray json = JArray.Parse(verjson);
                    Liteloaderobj = new ObservableCollection<object>();
                    
                     
                    for (int i = 0; i < json.Count; i++)
                    {
                        var ver = json[i]["version"].ToString();
                    
                        if (ver.Contains("_") == false)
                        {
                            //   var wj = json[i]["build"]["files"].ToString();
                            //   var wjm = "";
                            // if (json[i]["build"]["files"][1]["category"].ToString() == "universal")
                            //    {
                            //    wjm = json[i]["build"]["files"][1]["format"].ToString();
                            //    }
                            //   else if (json[i]["build"]["files"][2]["category"].ToString() == "universal")
                            //    {
                            //        wjm = json[i]["build"]["files"][2]["format"].ToString();
                            //   }
                            // (json[i]["type"].ToString() == "RELEASE" ? "正式版-release" : json[i]["type"].ToString() == "SNAPSHOT" ? "快照版-snapshot" : json[i]["type"].ToString())
                            Liteloaderobj.Add(new Liteloader { Ver = json[i]["mcversion"].ToString(), Zt = json[i]["type"].ToString(), Id = json[i]["version"].ToString()});
                           // MessageBox.Show(ver);
                            //VarList_1.Add(new minecraft() { 游戏版本 = json[i].ToString() });
                            //VarList_2.Add(new minecraft_forge() { 游戏版本 = json_var_arr_2[i]["build"]["mcversion"].ToString(), 构建时间 = json_var_arr_2[i]["build"]["modified"].ToString(), 构建版本号 = json_var_arr_2[i]["build"]["version"].ToString(), 版本名 = json_var_arr_2[i]["name"].ToString() });
                        }
                    }
                    LiteloaderView.DataContext = Liteloaderobj;

                }
                catch
                {
                    btn_下载.IsEnabled = false;
                    this.ShowMessageAsync("链接错误", "目前无法连接网络,或服务器正忙,请稍候再试");
                    //   forgeView.Items.Add("链接错误");
                }
            }
            LiteloaderView.Visibility = Visibility.Visible;
        }

        private void Rbtn4_Checked(object sender, RoutedEventArgs e)
        {
            label.Content = "OptiFine MOD需要Forge,请确保您已安装Forge";
            listView.Visibility = Visibility.Hidden;
            forgeView.Visibility = Visibility.Hidden;
            LiteloaderView.Visibility = Visibility.Hidden;
            btn_下载.IsEnabled = true;
            //this.ShowMessageAsync("功能被删除", "很抱歉,此功能在本版本因资源占用过多被删除,如有需要,请使用E版来自动下载安装Optifine");
            if (OptifineView.Items.Count < 5)
            {
                OptifineView.Items.Clear();
                try
                {
                    string verjson = Beelogin.GetGeneralContent("http://download.mcbbs.net/optifine/versionlist");
                    //MessageBox.Show(verjson);
                    JArray json = JArray.Parse(verjson);
                    Optifineobj = new ObservableCollection<object>();


                    for (int i = 0; i < json.Count; i++)
                    {
                        var ver = json[i]["mcversion"].ToString();


                            //   var wj = json[i]["build"]["files"].ToString();
                            //   var wjm = "";
                            // if (json[i]["build"]["files"][1]["category"].ToString() == "universal")
                            //    {
                            //    wjm = json[i]["build"]["files"][1]["format"].ToString();
                            //    }
                            //   else if (json[i]["build"]["files"][2]["category"].ToString() == "universal")
                            //    {
                            //        wjm = json[i]["build"]["files"][2]["format"].ToString();
                            //   }
                            
                            // (json[i]["type"].ToString() == "RELEASE" ? "正式版-release" : json[i]["type"].ToString() == "SNAPSHOT" ? "快照版-snapshot" : json[i]["type"].ToString())
                            Optifineobj.Add(new Optifine { Ver = json[i]["mcversion"].ToString(), Zt = json[i]["type"].ToString(), Id = json[i]["patch"].ToString() });
                            // MessageBox.Show(ver);
                            //VarList_1.Add(new minecraft() { 游戏版本 = json[i].ToString() });
                            //VarList_2.Add(new minecraft_forge() { 游戏版本 = json_var_arr_2[i]["build"]["mcversion"].ToString(), 构建时间 = json_var_arr_2[i]["build"]["modified"].ToString(), 构建版本号 = json_var_arr_2[i]["build"]["version"].ToString(), 版本名 = json_var_arr_2[i]["name"].ToString() });
                        
                    }
                    OptifineView.DataContext = Optifineobj;

                }
                catch
                {
                    btn_下载.IsEnabled = false;
                    this.ShowMessageAsync("链接错误", "目前无法连接网络,或服务器正忙,请稍候再试");
                    //   forgeView.Items.Add("链接错误");
                }
            }
            OptifineView.Visibility = Visibility.Visible;

        }


        void Bw_DoWork3(object sender, DoWorkEventArgs e)
        {
            if (!e.Cancel)
            {
                string dir_name = @AppDomain.CurrentDomain.BaseDirectory;
                Liteloader Liteloader = null;
              //  forge forge = null;
                Dispatcher.Invoke(new Action(() => { Liteloader = LiteloaderView.SelectedItem as Liteloader; }));
                try
                {
                    if (Liteloader != null && Liteloader is Liteloader)
                    {
                        string var_name = Liteloader.Ver;//游戏版本号
                        string zt = Liteloader.Zt;//类型
                        string var_lite = Liteloader.Id;//版本号


                        if (File.Exists(dir_name + "/.minecraft/mods/liteloader-" + var_name + "-" + zt + ".jar") || File.Exists(dir_name + "/.minecraft/mods/liteloader-" + var_lite + ".jar"))
                        {
                            Ts("文件已存在", "您要下载的LiteLoader已存在,无需下载.");
                            e.Cancel = true;
                            return;
                        }
                        if (!File.Exists(dir_name + @"\.minecraft\versions\" + var_name + "\\" + var_name + ".json") || !File.Exists(dir_name + @"\.minecraft\versions\" + var_name + "\\" + var_name + ".jar"))
                        {
                            if (!Directory.Exists(dir_name + "/.minecraft/versions/" + var_name))
                            {
                                Directory.CreateDirectory(dir_name + "/.minecraft/versions/" + var_name);//创建文件夹
                            }
                            string jarurl = "http://download.mcbbs.net/versions/" + var_name + "/client";
                            string jsonurl = "http://download.mcbbs.net/versions/" + var_name + "/json";
                            if (Beelogin.DownloadFile(jarurl, dir_name + " /.minecraft/versions/" + var_name + "/" + var_name + ".jar") == true)
                            {
                                if (Beelogin.DownloadFile(jsonurl, dir_name + "/.minecraft/versions/" + var_name + "/" + var_name + ".json") == false)
                                {
                                    Ts("下载失败", var_name + "版本的客户端下载失败,不能继续下载LiteLoader\r\n出错URL:" + jsonurl);
                                    Directory.Delete(dir_name + "/.minecraft/versions/" + var_name, true);
                                    e.Cancel = true;
                                    return;
                                }
                            }
                            else
                            {
                                Ts("下载失败", var_name + "版本的客户端下载失败,不能继续下载LiteLoader\r\n出错URL:" + jarurl);
                                Directory.Delete(dir_name + "/.minecraft/versions/" + var_name, true);
                                e.Cancel = true;
                                return;
                            }
                        }

               
                       // string forge_link = $"http://download.mcbbs.net/maven/net/minecraftforge/forge/{var_name}-{var_forge}" + (fz != "" ? $"-{fz}" : "") + $"/forge-{var_name}-{var_forge}" + (fz != "" ? $"-{fz}" : "") + $"-universal.jar";
                     //   string forgedir = dir_name + "/.minecraft/libraries/net/minecraftforge/forge/" + var_name + "-" + var_forge + (fz != "" ? $"-{fz}" : "") + "/" + "forge-" + var_name + "-" + var_forge + (fz != "" ? $"-{fz}" : "") + ".jar";
                        if (!Directory.Exists(dir_name + "/.minecraft/mods"))
                        {
                            Directory.CreateDirectory(dir_name + "/.minecraft/mods");
                        }
                        if (!Directory.Exists(dir_name + "/.minecraft/beelanuch_downtemp"))
                        {
                            Directory.CreateDirectory(dir_name + "/.minecraft/beelanuch_downtemp");
                        }
                        string Lite_link;
                        if (zt == "SNAPSHOT")
                        {
                             Lite_link = "http://download.mcbbs.net/maven/com/mumfrey/liteloader/" + var_name + "-" + zt + "/liteloader-" + var_name + "-" + zt + ".jar";
                        }
                        else {
                             Lite_link = "http://download.mcbbs.net/maven/com/mumfrey/liteloader/" + var_name + "/liteloader-" + var_lite + ".jar";
                        }

                     //   Console.WriteLine(forge_link);
                      //  MessageBox.Show(forge_link);
                        if (Beelogin.DownloadFile(Lite_link, dir_name + "/.minecraft/beelanuch_downtemp/liteloader.jar"))
                        {
                           // MessageBox.Show(Lite_link);
                            //    KMCCC.Tools.ZipTools.UnzipFile(dir_name + "/.minecraft/libraries/net/minecraftforge/forge/" + var_name + "-" + var_forge + "/" + "forge-" + var_name + "-" + var_forge + ".jar", dir_name + "/.minecraft/beelanuch_downtemp", null);
                            //UnZip(forgedir, dir_name + "/.minecraft/beelanuch_downtemp");
                            if (zt == "SNAPSHOT")
                            {
                                File.Move(dir_name + "/.minecraft/beelanuch_downtemp/liteloader.jar", dir_name + "/.minecraft/mods/liteloader-" + var_name + "-" + zt + ".jar");
                            }
                            else
                            {
                                File.Move(dir_name + "/.minecraft/beelanuch_downtemp/liteloader.jar", dir_name + "/.minecraft/mods/liteloader-" + var_lite + ".jar");
                            }
                          
                            Directory.Delete(dir_name + "/.minecraft/beelanuch_downtemp", true);
                            Ts("下载成功!", "'" + var_name + "'版本的liteloader'" + var_lite + "'已下载完毕");
                            e.Cancel = true;
                            return;
                        }
                        else
                        {
                            Ts("下载失败", "下载'" + var_name + "'版本的liteloader'" + var_lite + "'失败,请重试或下载其他版本\r\n出错URL:" + Lite_link);
                           // Directory.Delete(dir_name + "/.minecraft/versions/" + var_name + "-" + "forge" + var_name + "-" + var_forge, true);
                            e.Cancel = true;
                            return;
                        }
                    }
                }
                catch
                {
                    Ts("下载失败", "下载失败,请重试或下载其他版本");
                    e.Cancel = true;
                    return;
                }
            }
        }
        void Bw_DoWork4(object sender, DoWorkEventArgs e)
        {
            if (!e.Cancel)
            {
                string dir_name = @AppDomain.CurrentDomain.BaseDirectory;
                Optifine Optifine = null;
                //  forge forge = null;
                Dispatcher.Invoke(new Action(() => { Optifine = OptifineView.SelectedItem as Optifine; }));
                try
                {
                    if (Optifine != null && Optifine is Optifine)
                    {
                        string var_name = Optifine.Ver;//游戏版本号
                        string zt = Optifine.Zt;//类型
                        string patch = Optifine.Id;//版本号


                        if (File.Exists(dir_name + "/.minecraft/mods/OptiFine_" + var_name + "_" + zt + "_" + patch + ".jar"))
                        {
                            Ts("文件已存在", "您要下载的OptiFine已存在,无需下载.");
                            e.Cancel = true;
                            return;
                        }
                        if (!File.Exists(dir_name + @"\.minecraft\versions\" + var_name + "\\" + var_name + ".json") || !File.Exists(dir_name + @"\.minecraft\versions\" + var_name + "\\" + var_name + ".jar"))
                        {
                            if (!Directory.Exists(dir_name + "/.minecraft/versions/" + var_name))
                            {
                                Directory.CreateDirectory(dir_name + "/.minecraft/versions/" + var_name);//创建文件夹
                            }
                            string jarurl = "http://download.mcbbs.net/versions/" + var_name + "/client";
                            string jsonurl = "http://download.mcbbs.net/versions/" + var_name + "/json";
                            if (Beelogin.DownloadFile(jarurl, dir_name + " /.minecraft/versions/" + var_name + "/" + var_name + ".jar") == true)
                            {
                                if (Beelogin.DownloadFile(jsonurl, dir_name + "/.minecraft/versions/" + var_name + "/" + var_name + ".json") == false)
                                {
                                    Ts("下载失败", var_name + "版本的客户端下载失败,不能继续下载OptiFine\r\n出错URL:" + jsonurl);
                                    Directory.Delete(dir_name + "/.minecraft/versions/" + var_name, true);
                                    e.Cancel = true;
                                    return;
                                }
                            }
                            else
                            {
                                Ts("下载失败", var_name + "版本的客户端下载失败,不能继续下载OptiFine\r\n出错URL:" + jarurl);
                                Directory.Delete(dir_name + "/.minecraft/versions/" + var_name, true);
                                e.Cancel = true;
                                return;
                            }
                        }


                        // string forge_link = $"http://download.mcbbs.net/maven/net/minecraftforge/forge/{var_name}-{var_forge}" + (fz != "" ? $"-{fz}" : "") + $"/forge-{var_name}-{var_forge}" + (fz != "" ? $"-{fz}" : "") + $"-universal.jar";
                        //   string forgedir = dir_name + "/.minecraft/libraries/net/minecraftforge/forge/" + var_name + "-" + var_forge + (fz != "" ? $"-{fz}" : "") + "/" + "forge-" + var_name + "-" + var_forge + (fz != "" ? $"-{fz}" : "") + ".jar";
                        if (!Directory.Exists(dir_name + "/.minecraft/mods"))
                        {
                            Directory.CreateDirectory(dir_name + "/.minecraft/mods");
                        }
                        if (!Directory.Exists(dir_name + "/.minecraft/beelanuch_downtemp"))
                        {
                            Directory.CreateDirectory(dir_name + "/.minecraft/beelanuch_downtemp");
                        }
                        string Opti_link = "http://download.mcbbs.net/optifine/" + var_name + "/" +  zt + "/" + patch;
                        //   Console.WriteLine(forge_link);
                        //  MessageBox.Show(forge_link);
                        if (Beelogin.DownloadFile(Opti_link, dir_name + "/.minecraft/beelanuch_downtemp/optifine.jar"))
                        {
                            // MessageBox.Show(Lite_link);
                            //    KMCCC.Tools.ZipTools.UnzipFile(dir_name + "/.minecraft/libraries/net/minecraftforge/forge/" + var_name + "-" + var_forge + "/" + "forge-" + var_name + "-" + var_forge + ".jar", dir_name + "/.minecraft/beelanuch_downtemp", null);
                            //UnZip(forgedir, dir_name + "/.minecraft/beelanuch_downtemp");

                                File.Move(dir_name + "/.minecraft/beelanuch_downtemp/optifine.jar", dir_name + "/.minecraft/mods/OptiFine_" + var_name + "_" + zt + "_" + patch + ".jar");
                            

                            Directory.Delete(dir_name + "/.minecraft/beelanuch_downtemp", true);
                            Ts("下载成功!", "'" + var_name + "'版本的'Optifine_" + zt + "_" + patch + "'已下载完毕");
                            e.Cancel = true;
                            return;
                        }
                        else
                        {
                            Ts("下载失败", "下载'" + var_name + "'版本的'Optifine_" + zt + "_" + patch + "'失败,请重试或下载其他版本\r\n出错URL:" +Opti_link);
                            // Directory.Delete(dir_name + "/.minecraft/versions/" + var_name + "-" + "forge" + var_name + "-" + var_forge, true);
                            e.Cancel = true;
                            return;
                        }
                    }
                }
                catch
                {
                    Ts("下载失败", "下载失败,请重试或下载其他版本");
                    e.Cancel = true;
                    return;
                }
            }
        }
        #region 解压  

        /// <summary>   
        /// 解压功能(解压压缩文件到指定目录)   
        /// </summary>   
        /// <param name="fileToUnZip">待解压的文件</param>   
        /// <param name="zipedFolder">指定解压目标目录</param>   
        /// <param name="password">密码</param>   
        /// <returns>解压结果</returns>   
        public bool UnZip(string fileToUnZip, string zipedFolder, string password)
        {
            bool result = true;
            FileStream fs = null;
            ZipInputStream zipStream = null;
            ZipEntry ent = null;
            string fileName;

            if (!File.Exists(fileToUnZip))
                return false;

            if (!Directory.Exists(zipedFolder))
                Directory.CreateDirectory(zipedFolder);
           
            try
            {
                zipStream = new ZipInputStream(File.OpenRead(fileToUnZip));
                if (!string.IsNullOrEmpty(password)) zipStream.Password = password;
                while ((ent = zipStream.GetNextEntry()) != null)
                {
                    if (ent.FileName.Contains("version.json") && !string.IsNullOrEmpty(ent.FileName) && !ent.FileName.Contains("META-INF"))
                    {
                       
                        fileName = Path.Combine(zipedFolder, ent.FileName);
                        fileName = fileName.Replace('/', '\\');//change by Mr.HopeGi   
                       
                        if (fileName.EndsWith("\\"))
                        {
                            Directory.CreateDirectory(fileName);
                            continue;
                        }
                        
                        fs = File.Create(fileName);
                        int size = 2048;
                        byte[] data = new byte[size];
                     
                        while (true)
                        {
                            size = zipStream.Read(data, 0, data.Length);
                            if (size > 0)
                                fs.Write(data, 0, size);
                            else
                                break;
                        }
                    }
                }
            }
            catch
            {
                result = false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
                if (zipStream != null)
                {
                    zipStream.Close();
                    zipStream.Dispose();
                }
                if (ent != null)
                {
                    ent = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            return result;
        }

        /// <summary>   
        /// 解压功能(解压压缩文件到指定目录)   
        /// </summary>   
        /// <param name="fileToUnZip">待解压的文件</param>   
        /// <param name="zipedFolder">指定解压目标目录</param>   
        /// <returns>解压结果</returns>   
        public bool UnZip(string fileToUnZip, string zipedFolder)
        {
            bool result = UnZip(fileToUnZip, zipedFolder, null);
            return result;
        }

        #endregion
    }

}

