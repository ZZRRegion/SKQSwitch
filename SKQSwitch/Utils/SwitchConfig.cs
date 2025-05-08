using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKQSwitch.Utils
{
    /// <summary>
    /// 配置信息
    /// </summary>
    internal class SwitchConfig
    {
        /// <summary>
        /// 进程名称
        /// </summary>
        public string ExeName { get; set; }
        /// <summary>
        /// 主窗口名称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 展示时间
        /// </summary>
        public int Time { get; set; }
        public SwitchConfig(string exeName, string title, int time)
        {
            this.ExeName = exeName;
            this.Title = title;
            this.Time = time;
        }
        static SwitchConfig()
        {
            SwitchConfigs = Load();
        }
        public static void Add(string exeName, string title, int time)
        {
            SwitchConfigs.Add(new(exeName, title, time));
            Save();
        }
        public static bool Remove(string? exeName)
        {
            foreach(SwitchConfig config in SwitchConfigs)
            {
                if(config.ExeName == exeName)
                {
                    SwitchConfigs.Remove(config);
                    Save();
                    return true;
                }
            }
            return false;
        }

        public static List<SwitchConfig> SwitchConfigs;
        /// <summary>
        /// 加载配置信息
        /// </summary>
        /// <returns></returns>
        public static List<SwitchConfig> Load()
        {
            string fileName = SKQPath.SwitchConfigFileName;
            if (File.Exists(fileName))
            {
                string content = File.ReadAllText(fileName);
                if(string.IsNullOrWhiteSpace(content))
                {
                    LogUtil.AddInfoInsertDateTime($"配置文件内容不能为空！{fileName}");
                    return new();
                }
                try
                {
                    return System.Text.Json.JsonSerializer.Deserialize<List<SwitchConfig>>(content) ?? new();
                }
                catch(Exception ex)
                {
                    LogUtil.AddInfoInsertDateTime(ex.ToString());
                }
            }
            else
            {
                LogUtil.AddInfoInsertDateTime($"配置文件不存在！{fileName}");
            }
            return new();
        }
        /// <summary>
        /// 保存配置信息
        /// </summary>
        /// <param name="list"></param>
        public static void Save()
        {
            string content = System.Text.Json.JsonSerializer.Serialize(SwitchConfigs);
            File.WriteAllText(SKQPath.SwitchConfigFileName, content);
        }
    }
}
