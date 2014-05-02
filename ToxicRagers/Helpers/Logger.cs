using System;
using System.IO;

namespace ToxicRagers.Helpers
{
    public static class Logger
    {
        public static void ResetLog()
        {
            using (StreamWriter w = File.CreateText(Environment.CurrentDirectory + "\\Flummery.log"))
            {
            }
        }

        public static void LogToFile(string format, params object[] args) {
            using (StreamWriter w = File.AppendText(Environment.CurrentDirectory + "\\Flummery.log"))
            {
                w.WriteLine(string.Format(format, args));
            }
        }
    }
}
