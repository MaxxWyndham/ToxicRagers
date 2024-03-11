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

        public static bool AllowLog { get; set; } = true;

        public static string LogName { get; set; } = "Flummery.log";

        public static void ResetLog()
        {
            if (!AllowLog) { return; }

            try
            {
                using StreamWriter w = File.CreateText(Path.Combine(Environment.CurrentDirectory, LogName));
            }
            catch { }
        }

        public static void LogToFile(LogLevel level, string format, params object[] args)
        {
            if (!AllowLog) { return; }

            if (level == LogLevel.All || Level.HasFlag(level))
            {
                try
                {
                    using StreamWriter w = File.AppendText(Path.Combine(Environment.CurrentDirectory, LogName));
                    w.WriteLine(string.Format(format, args));
                }
                catch { }
            }
        }

        public static void LogToFile(LogLevel level, string line)
        {
            if (!AllowLog) { return; }

            if (level == LogLevel.All || Level.HasFlag(level))
            {
                try
                {
                    using StreamWriter w = File.AppendText(Path.Combine(Environment.CurrentDirectory, LogName));
                    w.WriteLine(line);
                }
                catch { }
            }
        }
    }
}