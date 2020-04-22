namespace BeeLauncher
{
    #region

    using System.Reflection;
    using System.Threading;
    using System.Windows;
    using KMCCC.Launcher;
    using System;
    using System.IO;
    using System.Linq;


    #endregion

    /// <summary>
    ///     App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        public static LauncherCore Core = LauncherCore.Create();
        private MainWindow _mainWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Reporter.SetClientName("BeeLauncher @" + Assembly.GetExecutingAssembly().GetName().Version);
          
            _mainWindow = new MainWindow();
            _mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Thread.Sleep(1000);
            Logger.End();
        }
    }
}