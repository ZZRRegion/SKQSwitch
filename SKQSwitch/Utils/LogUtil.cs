using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SKQSwitch.Utils
{
    internal static class LogUtil
    {
        public static void AddInfo(string info)
        {
            File.AppendAllText(SKQPath.LogFileName, info);
        }
        public static void AddInfoInsertDateTime(string info)
        {
            string msg = $"{DateTime.Now}{info}{Environment.NewLine}";
            File.AppendAllText(SKQPath.LogFileName, msg);
        }
    }
}
