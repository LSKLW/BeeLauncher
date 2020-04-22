﻿namespace BeeLauncher
{
    #region

    using System;
    using System.IO;

    #endregion

    public static class Logger
    {
        private static readonly TextWriter LogWriter;

        static Logger()
        {
            LogWriter = File.CreateText("BeeLauncher.log");
        }

        public static void Log(String log)
        {
            lock (LogWriter)
            {
                LogWriter.WriteLine(log);
            }
        }

        public static void End()
        {
            lock (LogWriter)
            {
                LogWriter.Flush();
                LogWriter.Dispose();
            }
        }
    }
}