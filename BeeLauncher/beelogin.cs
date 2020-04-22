namespace BeeLauncher
{
    using LitJson;
    using System;
    #region
    using System.IO;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Text;
    #endregion
    class Beelogin
    {
        public static string Bee_login(string url, string name, string pass, string ip)
        {
            try
            {
                string fh = GetGeneralContent(url + "login.php?username=" + name + "&psd=" + pass +  "&ip=" + ip );
                if (fh == "no") { fh = "用户名或密码有误"; }
                    return fh;
            }
            catch
            {
                return "error";
            }
        }
        public static string Beemodlogin_token(string url, string name, string pass,string configdir, string ip)
        {
            try
            {
                string fh = GetGeneralContent(url + "login.php?username=" + name + "&psd=" + pass + "&ip=" + ip+"&gettoken=true");
              //  MessageBox.Show(fh);
                if (fh == "no") { fh = "用户名或密码有误"; } else if (fh.Substring(0, 3) == "yes") {
                File.WriteAllText(configdir,"# Configuration file\r\n\r\nbeelogin {\r\n# DO NOT EDIT IT!! [default: unknow]\r\n    S:token=" + GetRight(fh,"yes;") + "\r\n}", Encoding.Default);
                    fh = "yes"; }
                return fh;
            }
            catch {
                    return "未知错误,可能是网络连接失败造成的.";      
            }
        }
        public static string Beeregister(string url, string name, string pass, string passs, string id, string code,string ip)
        {
            try
            {
              
                string fh = GetGeneralContent(url + "register.php?username="+name+"&password="+pass+"&pwd_again="+passs+"&mac="+ GetMacAddressByNetworkInformation()+"&ip="+ ip+ "&id=" +id+"&code="+code.ToUpper());
  
                if (fh.Substring(0, 2) == "ok") { fh = "ok"; } else if (fh == "bee0") { fh = "这个用户名已经被注册了"; } else if (fh == "bee1") { fh = "用户名或密码为空"; } else if (fh == "bee2") { fh = "两次密码不一致"; } else if (fh == "bee3") { fh = "已达到注册上限"; } else if (fh == "bee4") { fh = "数据库出错"; } else if (fh == "bee5") { fh = "状态码为空"; } else if (fh == "bee6") { fh = "验证码错误"; }
                return fh;
            }
            catch
            {
                return "error";
            }
        }
        public static string Beequit(string url, string name,string ip)
        {
            try
            {
                string fh = GetGeneralContent(url + "quit.php?username="+name+"&ip=" + ip);
             //   if(fh == "no") {fh = "失败";}else if(fh == "yes") { fh = "成功"; }
                return fh;
            }
            catch
            {
                return "error";
            }
        }
        public static string Beegetid(string url)
        {
            try
            {
                string fh = GetGeneralContent(url + "getid.php");
                return fh;
            }
            catch
            {
                return "error";
            }
        }
        public static string Beegetcode(string url,string id)
        {
            try {

                string fh = url + "showing.php?id=" + id;
                return fh;
            }
            catch
            {
                return "error";
            }
        }
        public static string GetMacAddressByNetworkInformation()
        {
            string macAddress = "";
            try
            {
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in nics)
                {
                    if (!adapter.GetPhysicalAddress().ToString().Equals(""))
                    {
                        macAddress = adapter.GetPhysicalAddress().ToString();
                        for (int i = 1; i < 6; i++)
                        {
                            macAddress = macAddress.Insert(3 * i - 1, ":");
                        }
                        break;
                    }
                }

            }
            catch
            {

            }
            return macAddress;
        }
        public static string GetIPAddress()
        {
                string json = GetGeneralContent("http://api.ip138.com/query/?token=f66d36f1f0dd61b6f2cee5b8bbcfb095");
                JsonData data = JsonMapper.ToObject(json);
                string ip = data["ip"].ToString();
                return ip;
    }
        public static string GetLeft(string str, string s)
        {
            string temp = str.Substring(0, str.LastIndexOf(s));
            return temp;
        }
        public static string GetRight(string str, string s)
    {
        string temp = str.Substring(str.IndexOf(s), str.Length - str.Substring(0, str.IndexOf(s)).Length);
        return temp;
    }
        public static string GetGeneralContent(string strUrl)
        {

            string strMsg = string.Empty;
            try
            {
                WebRequest request = WebRequest.Create(strUrl);
                request.Timeout = 7000;
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("gb2312"));

                strMsg = reader.ReadToEnd();

                reader.Close();
                reader.Dispose();
                response.Close();
            }
            catch
            { }
            return strMsg;
        }
        /// <summary>
        /// 获取当前版本号
        /// </summary>
        public static string Getver()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        /// <summary>
        /// 下载
        /// </summary>
        public static bool DownloadFile(string URL, string filename)
        {
            try
            {
                var downer = new WebClient();
                downer.Headers.Add("User-Agent", "BeeLauncher" + Getver());
                downer.DownloadFile(new Uri(URL), filename);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
