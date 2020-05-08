using System;
using System.IO;

namespace ToxicRagers.Helpers
{
    public static class Logger
    {
        [Flags]
        public enum LogLevel
        {
            Debug = 0x1,
            Info = 0x2,
            Warning = 0x4,
            Error = 0x8,
            All = Debug | Info | Warning | Error
        }

        public static LogLevel Level = LogLevel.Info | LogLevel.Error;

        public static void ResetLog()
        {
            using (StreamWriter w = File.CreateText(Environment.CurrentDirectory + "\\Flummery.log"))
            {
            }
        }

        public static void LogToFile(LogLevel level, string format, params object[] args)
        {
            if (level == LogLevel.All || Level.HasFlag(level))
            {
                using (StreamWriter w = File.AppendText(Environment.CurrentDirectory + "\\Flummery.log"))
                {
                    w.WriteLine(string.Format(format, args));
                }
            }
        }

        public static void LogToFile(LogLevel level, string line)
        {
            if (level == LogLevel.All || Level.HasFlag(level))
            {
                using (StreamWriter w = File.AppendText(Environment.CurrentDirectory + "\\Flummery.log"))
                {
                    w.WriteLine(line);
                }
            }
        }
    }
}