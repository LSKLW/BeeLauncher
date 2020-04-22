//using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Management;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.Win32;
using System.Windows;
namespace BeeLauncher
{
    class Minecraft_launch
    {
        public static string dir_name = System.Environment.CurrentDirectory;//初始化当前路径
        // 本方法由Fleey重构
        //public static void UnZip(string fileFromUnZip, string fileToUnZip)
        //{
        //    ZipInputStream inputStream = new ZipInputStream(File.OpenRead(fileFromUnZip));
        //    ZipEntry theEntry;
        //    while ((theEntry = inputStream.GetNextEntry()) != null)
        //    {
        //        fileToUnZip += "/";
        //        string fileName = System.IO.Path.GetFileName(theEntry.Name);
        //        string path = System.IO.Path.GetDirectoryName(fileToUnZip) + "/";
        //        // Directory.CreateDirectory(path);//生成解压目录
        //        if (fileName != String.Empty)
        //        {
        //            FileStream streamWriter = File.Create(path + fileName);//解压文件到指定的目录
        //            int size = 2048;
        //            byte[] data = new byte[2048];
        //            while (true)
        //            {
        //                size = inputStream.Read(data, 0, data.Length);
        //                if (size > 0)
        //                {
        //                    streamWriter.Write(data, 0, size);
        //                }
        //                else
        //                {
        //                    break;
        //                }
        //            }
        //            streamWriter.Close();
        //        }
        //    }
        //    inputStream.Close();
        //}//jar解压  = =其实是zip解压
        public static double Good_memory()
        {
            double capacity = 0.0;
            ManagementClass cimobject1 = new ManagementClass("Win32_PhysicalMemory");
            ManagementObjectCollection moc1 = cimobject1.GetInstances();
            foreach (ManagementObject mo1 in moc1)
            {
                capacity += ((Math.Round(Int64.Parse(mo1.Properties["Capacity"].Value.ToString()) / 1024 / 1024.0, 1)));
            }
            moc1.Dispose();
            cimobject1.Dispose();
            return capacity * 0.25;
        }//获取最佳内存
        static bool RunCmd(string cmdStr, string cmdExe)
        {
            bool result = false;
            try
            {
                using (Process myPro = new Process())
                {
                    //指定启动进程是调用的应用程序和命令行参数
                    ProcessStartInfo psi = new ProcessStartInfo(cmdExe, cmdStr);
                    myPro.StartInfo = psi;
                    myPro.Start();
                    result = true;
                }
            }
            catch
            {

            }
            return result;
        }
        public static String PostUrl(string url, string postData)
        {
            HttpWebRequest request = null;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                request = WebRequest.Create(url) as HttpWebRequest;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request.ProtocolVersion = HttpVersion.Version11;　　　　　　　　// 这里设置了协议类型。
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;// SecurityProtocolType.Tls1.2; 
                request.KeepAlive = false;
                ServicePointManager.CheckCertificateRevocationList = true;
                ServicePointManager.DefaultConnectionLimit = 100;
                ServicePointManager.Expect100Continue = false;
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(url);
            }

            request.Method = "POST";    //使用get方式发送数据
            request.ContentType = "application/x-www-form-urlencoded";
            request.AllowAutoRedirect = true;
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            request.Accept = "*/*";

            byte[] data = Encoding.UTF8.GetBytes(postData);
            Stream newStream = request.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                //client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                string result = string.Empty;
                using (StreamReader sr = new StreamReader(stream))
                {
                    result = sr.ReadToEnd();
                }

                return result;
            }
            catch (System.Exception ex)
            {
                return ex.ToString();
            }
            //获取网页响应结果

        }//http操作部分
        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }//ssl肯定部分
        public static String GetGeneralContent(string strUrl)
        {
            string strMsg = string.Empty;
            try
            {
                WebRequest request = WebRequest.Create(strUrl);
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
        }//获取网页内容  别问我为毛不用之前的post方法...
        public static String[] Login_mojang(string username, string password)
        {
            if (username != null && password != null)
            {
                string result = PostUrl("http://fleey.org/mojang_login.php", "username=" + username + "&password=" + password);
                JObject json_arr = (JObject)JsonConvert.DeserializeObject(result);
                if (json_arr["error"] != null)
                {
                    return null;
                }
                else
                {
                    String[] json = new string[3];
                    json[0] = json_arr["accessToken"].ToString();//返回token
                    json[1] = json_arr["selectedProfile"]["name"].ToString();//返回用户名
                    json[2] = json_arr["selectedProfile"]["id"].ToString();//返回uuid
                    return json;
                }
            }
            else
            {
                return null;
            }
        }//正版登录mojang  [0]token [1]uesename [2]uuid 登录失败返回null
        public static List<String> Check_libraries(string minecraft_json)
        {
            JObject json_arr = (JObject)JsonConvert.DeserializeObject(minecraft_json);//载入json
            List<String> str_list = new List<String>();//初始化list变量
            for (int i = 0; i < json_arr["libraries"].Count(); i++)
            {
                String[] str = json_arr["libraries"][i]["name"].ToString().Split(new char[] { ':' });
                if (!File.Exists(dir_name + "/.minecraft/libraries/" + str[0].Replace(".", "/").ToString() + "/" + str[1] + "/" + str[2] + "/" + str[1] + "-" + str[2] + ".jar"))
                {
                    str_list.Add(str[0].Replace(".", "/").ToString() + "/" + str[1] + "/" + str[2] + "/" + str[1] + "-" + str[2] + ".jar");
                }
            }
            return str_list;
        }//检测缺失libraries并返回
        private static String Libraries_conversion(String minecraft_json, bool is_read_json = false)
        {
            if (is_read_json == true)
            {
                minecraft_json = Read(minecraft_json);
            }
            JObject json_arr = (JObject)JsonConvert.DeserializeObject(minecraft_json);
            String libraries_dir = "";
            for (int i = 0; i < json_arr["libraries"].Count(); i++)
            {
                String[] str = json_arr["libraries"][i]["name"].ToString().Split(new char[] { ':' });
                if (File.Exists(dir_name + "/.minecraft/libraries/" + str[0].Replace(".", "/").ToString() + "/" + str[1] + "/" + str[2] + "/" + str[1] + "-" + str[2] + ".jar"))
                {
                    libraries_dir += dir_name + "/.minecraft/libraries/" + str[0].Replace(".", "/").ToString() + "/" + str[1] + "/" + str[2] + "/" + str[1] + "-" + str[2] + ".jar;";
                }
            }
            return libraries_dir;
        }//解释json
        public static void Release_natives(string minecraft_json)
        {
            JObject json_arr = (JObject)JsonConvert.DeserializeObject(minecraft_json);
            if (!File.Exists(dir_name + "/.minecraft/versions/" + json_arr["id"].ToString() + "/" + json_arr["id"].ToString() + ".json"))
            {
                return;
            }
            minecraft_json = Read(dir_name + "/.minecraft/versions/" + json_arr["id"].ToString() + "/" + json_arr["id"].ToString() + ".json");
            //优化代码
            for (int i = 0; i < json_arr["libraries"].Count(); i++)
            {
                String[] str = json_arr["libraries"][i]["name"].ToString().Split(new char[] { ':' });
                if (!Directory.Exists(dir_name + "/.minecraft/versions/" + json_arr["id"].ToString() + "/" + json_arr["id"].ToString() + "-natives"))
                {
                    Directory.CreateDirectory(dir_name + "/.minecraft/versions/" + json_arr["id"].ToString() + "/" + json_arr["id"].ToString() + "-natives");//创建文件夹
                }
                try
                {
                    if (json_arr["libraries"][i]["natives"]["windows"] != null || json_arr["libraries"][i]["downloads"]["classifiers"]["natives-windows"] != null)
                    {
                        if (json_arr["libraries"][i]["natives"]["windows"].ToString() != null)
                        {
                            if (Environment.Is64BitOperatingSystem)
                            {
                                //UnZip(dir_name + "/.minecraft/libraries/" + str[0].Replace(".", "/").ToString() + "/" + str[1] + "/" + str[2] + "/" + str[1] + "-" + str[2] + "-" + json_arr["libraries"][i]["natives"]["windows"].ToString().Replace("${arch}", "64") + ".jar", dir_name + "/.minecraft/versions/" + json_arr["id"].ToString() + "/" + json_arr["id"].ToString() + "-natives");
                            }
                            else
                            {
                                //UnZip(dir_name + "/.minecraft/libraries/" + str[0].Replace(".", "/").ToString() + "/" + str[1] + "/" + str[2] + "/" + str[1] + "-" + str[2] + "-" + json_arr["libraries"][i]["natives"]["windows"].ToString().Replace("${arch}", "32") + ".jar", dir_name + "/.minecraft/versions/" + json_arr["id"].ToString() + "/" + json_arr["id"].ToString() + "-natives");
                            }
                        }
                        else
                        {
                            //UnZip(dir_name + "/.minecraft/libraries/" + str[0].Replace(".", "/").ToString() + "/" + str[1] + "/" + str[2] + "/" + str[1] + "-" + str[2] + ".jar", dir_name + "/.minecraft/versions/" + json_arr["id"].ToString() + "/" + json_arr["id"].ToString() + "-natives");
                        }
                    }
                }
                catch (System.Exception)
                {

                }
            }
            //判断forge版本
        }//解压natives
        public static String Read(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.UTF8);
            String line;
            String str = "";
            while ((line = sr.ReadLine()) != null)
            {
                str += line.ToString();
            }
            return str;
        }//读入文件
        public static String[] Check_assets(string minecraft_json)
        {
            JObject json_arr = (JObject)JsonConvert.DeserializeObject(minecraft_json);
            if (File.Exists(dir_name + "/.minecraft/assets/indexes/" + json_arr["assets"] + ".json"))
            {
                json_arr = (JObject)JsonConvert.DeserializeObject(Read(dir_name + "/.minecraft/assets/indexes/" + json_arr["assets"] + ".json"));
                JObject json_arr_2 = (JObject)json_arr["objects"];
                int g = 0;
                String assets = "";
                foreach (var first_name in json_arr_2)
                {
                    if (!File.Exists(dir_name + "/.minecraft/assets/objects/" + json_arr_2[first_name.Key]["hash"].ToString().Substring(0, 2) + "/" + json_arr_2[first_name.Key]["hash"]))
                    {
                        assets += json_arr_2[first_name.Key]["hash"].ToString().Substring(0, 2) + "/" + json_arr_2[first_name.Key]["hash"] + ";";
                        g++;
                    }
                }

                return assets.Split(new char[] { ';' });
            }
            else
            {
                return null;
            }
        }//需要进行-1个数组就这样
        public static void Load_minecraft(string minecraft_json, string mojang_username_f, string maxmemory, string java_dir, string mojang_uuid_f = "{}", string mojang_token_f = "{}")
        {
            JObject json_arr = (JObject)JsonConvert.DeserializeObject(minecraft_json);
            String minecraftArguments;//机构化字符
            minecraftArguments = json_arr["minecraftArguments"].ToString().Replace("${auth_player_name}", mojang_username_f);
            minecraftArguments = minecraftArguments.Replace("${version_name}", "\"" + json_arr["id"].ToString() + "\"");
            minecraftArguments = minecraftArguments.Replace("${game_directory}", "\"" + dir_name + @"\.minecraft" + "\"");
            minecraftArguments = minecraftArguments.Replace("${game_assets}", "\"" + dir_name + "\"\\.minecraft\\assets\"" + "\"");
            minecraftArguments = minecraftArguments.Replace("${assets_root}", "\"" + dir_name + "\\.minecraft\\assets" + "\"");
            if (json_arr["assets"] != null)
            {
                minecraftArguments = minecraftArguments.Replace("${assets_index_name}", json_arr["assets"].ToString());
            }
            else
            {
                minecraftArguments = minecraftArguments.Replace("${assets_index_name}", json_arr["inheritsFrom"].ToString());
            }
            minecraftArguments = minecraftArguments.Replace("${auth_uuid}", mojang_uuid_f);
            minecraftArguments = minecraftArguments.Replace("${auth_session}", mojang_token_f);
            minecraftArguments = minecraftArguments.Replace("${auth_access_token}", mojang_token_f);
            minecraftArguments = minecraftArguments.Replace("${user_properties}", "{}");
            minecraftArguments = minecraftArguments.Replace("${user_type}", "Legacy");

            //替换minecraftArguments
            String dir_natives;//存在选用
            //判断natives路径是否存在
            String jar_lib;//jar_lib
            String cmd;//执行cmd命令
            String link;
            if (!File.Exists(dir_name + "/.minecraft/versions/" + json_arr["id"].ToString() + "/" + json_arr["id"] + ".jar"))
            {
                if (json_arr["assets"] == null)
                {
                    link = dir_name + @"\.minecraft\versions\" + json_arr["inheritsFrom"] + @"\" + json_arr["inheritsFrom"] + ".jar";
                }
                else
                {
                    link = dir_name + @"\.minecraft\versions\" + json_arr["assets"] + @"\" + json_arr["assets"] + ".jar";
                }
            }
            else
            {
                link = dir_name + @"\.minecraft\versions\" + json_arr["id"] + @"\" + json_arr["id"] + ".jar";//判断目录下是否有原版jar如果有即采用
            }
            //判断原版jar
            if (json_arr["hidden"] != null || json_arr["inheritsFrom"] != null)
            {
                if (json_arr["assets"] == null)
                {
                    dir_natives = dir_name + "/.minecraft/versions/" + json_arr["inheritsFrom"].ToString() + "/" + json_arr["inheritsFrom"].ToString() + "-natives";
                }
                else
                {
                    dir_natives = dir_name + "/.minecraft/versions/" + json_arr["assets"].ToString() + "/" + json_arr["assets"].ToString() + "-natives";
                }
                //判断natives路径
                if (json_arr["assets"] == null)
                {
                    if (!File.Exists(dir_name + "/.minecraft/versions/" + json_arr["inheritsFrom"] + "/" + json_arr["inheritsFrom"] + ".json"))
                    {
                        return;
                    }
                    jar_lib = Libraries_conversion(minecraft_json) + Libraries_conversion(dir_name + "/.minecraft/versions/" + json_arr["inheritsFrom"] + "/" + json_arr["inheritsFrom"] + ".json", true);
                }
                else
                {
                    if (!File.Exists(dir_name + "/.minecraft/versions/" + json_arr["inheritsFrom"] + "/" + json_arr["inheritsFrom"] + ".json"))
                    {
                        return;
                    }
                    jar_lib = Libraries_conversion(minecraft_json) + Libraries_conversion(dir_name + "/.minecraft/versions/" + json_arr["assets"] + "/" + json_arr["assets"] + ".json", true);
                }
                jar_lib += link;
                cmd = ("-Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true -XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump -XX:+UseG1GC -XX:-UseAdaptiveSizePolicy -XX:-OmitStackTraceInFastThrow -Xmn128m -Xmx" + maxmemory + "m -Djava.library.path=\"" + dir_natives + "\" -cp \"" + jar_lib + "\" " + json_arr["mainClass"].ToString() + " " + minecraftArguments + " --height 480 --width 854").Replace("/", @"\");
                //Console.WriteLine("Forge");
                //forge状态
                if (json_arr["assets"] == null)
                {
                    Release_natives(Read(dir_name + @"\.minecraft\versions\" + json_arr["inheritsFrom"] + @"\" + json_arr["inheritsFrom"] + ".json"));//释放natives
                }
                else
                {
                    Release_natives(Read(dir_name + @"\.minecraft\versions\" + json_arr["assets"] + @"\" + json_arr["assets"] + ".json"));//释放natives
                }
            }
            else
            {
                dir_natives = dir_name + "/.minecraft/versions/" + json_arr["id"].ToString() + "/" + json_arr["id"].ToString() + "-natives";
                jar_lib = Libraries_conversion(minecraft_json) + link;
                cmd = ("-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump -XX:+UseG1GC -XX:-UseAdaptiveSizePolicy -XX:-OmitStackTraceInFastThrow -Xmn128m -Xmx" + maxmemory + "m -Djava.library.path=\"" + dir_natives + "\" -cp \"" + jar_lib + "\" " + json_arr["mainClass"].ToString() + " " + minecraftArguments + " --height 480 --width 854").Replace("/", @"\");
                //Console.WriteLine("No Forge");
                //非forge状态
                Release_natives(Read(dir_name + @"\.minecraft\versions\" + json_arr["id"] + @"\" + json_arr["id"] + ".json"));//释放natives
            }
            //Console.WriteLine("/c \"" + @"C:\Java8\bin\java.exe" + "\" " + cmd);
            //MessageBox.Show("\"" + java_dir + "\"" + " " + cmd);
            try
            {
                RunCmd(cmd, "\"" + java_dir + "\"");
            }
            catch (Exception e)
            {
                MessageBox.Show("运行Java过程中异常！" + e);
            }
        }//启动Minecraft
        public static string GetJavaPath()
        {
            string javaRunTimePath = @"SOFTWARE\JavaSoft\Java Runtime Environment\";
            var javaRK = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(javaRunTimePath);

            string currentVersion = javaRK.GetValue("CurrentVersion").ToString();
            javaRK = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(javaRunTimePath);

            using (var javaPathRK = javaRK.OpenSubKey(currentVersion))
            {
                return javaPathRK.GetValue("JavaHome").ToString() + @"\bin\javaw.exe";
            }
        }//获取java_path路径
    }
}

