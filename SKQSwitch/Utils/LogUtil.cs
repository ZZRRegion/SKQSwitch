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
        private static string FileName = DateTime.Now.ToString("yyyy-MM-dd HH-mm") + "_log.txt";
        static LogUtil()
        {
            string location = Assembly.GetExecutingAssembly().Location;
            string locationDir = Path.GetDirectoryName(location);
            string logDir = Path.Combine(locationDir, "Logs");
            if(!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            FileName = Path.Combine(logDir, FileName);
        }
        public static void AddInfo(string info)
        {
            File.AppendAllText(FileName, info);
        }
        public static void AddInfoInsertDateTime(string info)
        {
            string msg = $"{DateTime.Now}{info}{Environment.NewLine}";
            File.AppendAllText(FileName, msg);
        }
    }
}
