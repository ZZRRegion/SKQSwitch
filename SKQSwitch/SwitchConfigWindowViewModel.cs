using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SKQSwitch.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Vanara.PInvoke;

namespace SKQSwitch
{
    internal partial class SwitchConfigWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<SwitchConfig> switchs = new();
        [ObservableProperty]
        private string exeName = string.Empty;
        [ObservableProperty]
        private string title = string.Empty;
        [ObservableProperty]
        private int time = 3;
        [ObservableProperty]
        private string displayText = string.Empty;
        [RelayCommand]
        private void Add()
        {
            if(string.IsNullOrWhiteSpace(ExeName))
            {
                MessageBox.Show("请输入要添加的软件名称！");
                return;
            }
            if(this.Time <= 0)
            {
                MessageBox.Show("请输入显示时间！");
                return;
            }
            foreach(SwitchConfig config in SwitchConfig.SwitchConfigs)
            {
                if(config.ExeName == ExeName)
                {
                    MessageBox.Show($"当前已配置该软件:{ExeName}");
                    return;
                }
            }
            string info = string.Empty;
            Process[] processes = Process.GetProcessesByName(ExeName);
            if(processes.Length > 0)
            {
                foreach(Process process in processes)
                {
                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        info += $"进程ID：{process.Id}，句柄：{process.MainWindowHandle:X};";
                    }
                }
            }
            if(string.IsNullOrWhiteSpace(info) && !string.IsNullOrWhiteSpace(Title))
            {
                HWND hwnd = User32.FindWindow(null, Title);
                if(hwnd != HWND.NULL)
                {
                    if(User32.GetWindowThreadProcessId(hwnd, out uint pid) > 0)
                    {
                        info += $"进程ID：{pid},窗口句柄：{hwnd.DangerousGetHandle():X};";
                    }

                }
            }
            if(string.IsNullOrWhiteSpace(info))
            {
                info = "当前未找到该进程，是否继续添加?";
            }
            else
            {
                info = $"当前信息：{info}，确认添加？";
            }
            LogUtil.AddInfoInsertDateTime(info);
            if (MessageBox.Show(info, "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                SwitchConfig.Add(ExeName, Title, Time, DisplayText);
                this.Update();
            }
        }
        [RelayCommand]
        private void Remove()
        {
            if(string.IsNullOrWhiteSpace(ExeName))
            {
                MessageBox.Show("请输入要删除的软件名称！");
                return;
            }
            if (MessageBox.Show("确认删除？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                SwitchConfig.Remove(ExeName);
                this.Update();
            }
        }
        public void UpdateConfig(SwitchConfig switchConfig)
        {
            this.ExeName = switchConfig.ExeName;
            this.Title = switchConfig.Title;
            this.Time = switchConfig.Time;
            this.DisplayText = switchConfig.DisplayText;
        }
        public SwitchConfigWindowViewModel()
        {
            this.Update();
        }
        private void Update()
        {
            this.Switchs.Clear();
            foreach(SwitchConfig config in SwitchConfig.SwitchConfigs)
            {
                this.Switchs.Add(config);
            }
        }
    }
}
