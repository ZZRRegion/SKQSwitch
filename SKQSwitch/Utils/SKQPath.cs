using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SKQSwitch.Utils
{
    internal static class SKQPath
    {
        public static string RootPath = string.Empty;
        public static string LogFileName = string.Empty;
        public static string SwitchConfigFileName = string.Empty;
        static SKQPath()
        {
            string location = Assembly.GetExecutingAssembly().Location;
            RootPath = Path.GetDirectoryName(location) ?? string.Empty;
            SwitchConfigFileName = Path.Combine(RootPath, "SwitchConfig.json");
            string logsPath = Path.Combine(RootPath, "Logs");
            if(!Directory.Exists(logsPath))
            {
                Directory.CreateDirectory(logsPath);
            }
            LogFileName = Path.Combine(logsPath, DateTime.Now.ToString("yyyy-MM-dd") + "_log.txt");
        }
    }
}
