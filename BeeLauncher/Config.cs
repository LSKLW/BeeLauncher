namespace BeeLauncher
{
	#region

	using System;
    using System.IO;
    using LitJson;

    #endregion

    public class Config
    {
        private static Config _instance;
        [JsonPropertyName("UserName")] public string _mUserName;
        [JsonPropertyName("Password")]  public string _mPassword;
        [JsonPropertyName("ip")] public string _mip;
        [JsonPropertyName("port")]  public string _mport;
        [JsonPropertyName("Authenticator")] public bool _mAuthenticator;
        [JsonPropertyName("JAVAPath")] public string _mJavaPath;
        [JsonPropertyName("LastVersion")] public string _mLastVersion;
        [JsonPropertyName("LaunchMode")] public bool _mLaunchMode;
        [JsonPropertyName("MaxMemory")] public int _mMaxMemory;
        [JsonPropertyName("AutoMemory")] public bool _mAutoMemory;
        [JsonPropertyName("bg")] public bool _mbg;
        [JsonPropertyName("Theme")] public string _mTheme;
        [JsonPropertyName("color")] public string _mcolor;
        [JsonPropertyName("Beelogin")] public bool _mBeelogin;
        [JsonPropertyName("beeurl")] public string _mbeeurl;
        [JsonPropertyName("Beepsw")] public string _mBeepsw;
        [JsonPropertyName("Beemod")] public bool _mBeemod;
        static Config()
        {
            Load();
        }

        public static string JavPath
        {
            get { return _instance._mJavaPath; }
            set
            {
                _instance._mJavaPath = value;
                Save();
            }
        }

        public static string LastVersion
        {
            get { return _instance._mLastVersion; }
            set
            {
                _instance._mLastVersion = value;
                Save();
            }
        }

        public static string UserName
        {
            get { return _instance._mUserName; }
            set
            {
                _instance._mUserName = value;
                Save();
            }
        }

        public static bool Authenticator
        {
            get { return _instance._mAuthenticator; }
            set
            {
                _instance._mAuthenticator = value;
                Save();
            }
        }
        public static string Password
        {
            get { return _instance._mPassword; }
            set
            {
                _instance._mPassword = value;
                Save();
            }
        }
        
        public static int MaxMemory
        {
            get { return _instance._mMaxMemory; }
            set
            {
                _instance._mMaxMemory = value;
                Save();
            }
        }

        public static bool AutoMemory
        {
            get { return _instance._mAutoMemory; }
            set
            {
                _instance._mAutoMemory = value;
                Save();
            }
        }
        public static string ip
        {
            get { return _instance._mip; }
            set
            {
                _instance._mip = value;
                Save();
            }
        }
        public static string port
        {
            get { return _instance._mport; }
            set
            {
                _instance._mport = value;
                Save();
            }
        }
        public static bool LaunchMode
        {
            get { return _instance._mLaunchMode; }
            set
            {
                _instance._mLaunchMode = value;
                Save();
            }
        }
        public static bool bg
        {
            get { return _instance._mbg; }
            set
            {
                _instance._mbg = value;
                Save();
            }
        }
        public static string Theme
        {
            get { return _instance._mTheme; }
            set
            {
                _instance._mTheme = value;
                Save();
            }
        }
        public static string color
        {
            get { return _instance._mcolor; }
            set
            {
                _instance._mcolor = value;
                Save();
            }
        }
        public static bool Beelogin
        {
            get { return _instance._mBeelogin; }
            set
            {
                _instance._mBeelogin = value;
                Save();
            }
        }
        public static string Beepsw
        {
            get { return _instance._mBeepsw; }
            set
            {
                _instance._mBeepsw = value;
                Save();
            }
        }
        public static string beeurl
        {
            get { return _instance._mbeeurl; }
            set
            {
                _instance._mbeeurl = value;
                Save();
            }
        }
        public static bool Beemod
        {
            get { return _instance._mBeemod; }
            set
            {
                _instance._mBeemod = value;
                Save();
            }
        }
        public static void Save()
        {
            try
            {
                File.WriteAllText("BeeLauncher.cfg", JsonMapper.ToJson(_instance));
            }
            catch
            {
            }
        }

        public static void Load()
        {
            try
            {
                _instance = JsonMapper.ToObject<Config>(File.ReadAllText("BeeLauncher.cfg"));
            }
            catch
            {
                _instance = new Config();
            }
        }
    }
}