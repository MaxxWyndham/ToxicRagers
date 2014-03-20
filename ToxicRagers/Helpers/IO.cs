using System;
using System.IO;

namespace ToxicRagers.Helpers
{
    public static class IO
    {
        public static void LoopDirectoriesIn(string sPath, Action<DirectoryInfo> callBack)
        {
            foreach (DirectoryInfo d in new DirectoryInfo(sPath).GetDirectories())
            {
                LoopDirectoriesIn(d.FullName, callBack);
                callBack(d);
            }
        }
    }
}
