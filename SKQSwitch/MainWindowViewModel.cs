using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SKQSwitch.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Vanara.PInvoke;

namespace SKQSwitch
{
    internal partial class MainWindowViewModel : ObservableObject
    {
        /// <summary>
        /// 是否暂停切换
        /// </summary>
        private bool isPause = false;
        /// <summary>
        /// 切换软件的下标
        /// </summary>
        private int switchIndex = 0;
        [ObservableProperty]
        private string? exeName;
        private int time = 0;
        [ObservableProperty]
        private string info = string.Empty;
        [ObservableProperty]
        private string memo = "暂停";
        [RelayCommand]
        private void Pause()
        {
            this.isPause = !this.isPause;
            this.Memo = isPause ? "启动" : "暂停";
        }
        [RelayCommand]
        private void SwitchConfigWin(Window win)
        {
            SwitchConfigWindow switchConfigWindow = new();
            switchConfigWindow.Owner = win;
            switchConfigWindow.ShowDialog();
        }
        public MainWindowViewModel()
        {
            DispatcherTimer dispatcherTimer = new()
            {
                Interval = TimeSpan.FromSeconds(1),
            };
            dispatcherTimer.Tick += DispatcherTimer_Tick; ;
            dispatcherTimer.Start();
        }

        private void DispatcherTimer_Tick(object? sender, EventArgs e)
        {
            if (this.isPause)
            {
                this.AddInfo("当前是暂停状态");
                return;
            }
            if (SwitchConfig.SwitchConfigs.Count == 0)
                return;
            if(this.time > 0)//让计数归零
            {
                this.time--;
                return;
            }
            SwitchConfig config = SwitchConfig.SwitchConfigs[switchIndex++ % SwitchConfig.SwitchConfigs.Count];
            this.time = config.Time;
            this.ExeName = config.ExeName;
            this.SwitchWindow(config);
                
        }
        private void SwitchWindow(SwitchConfig config)
        {
            try
            {
                string processName = config.ExeName;
                Process[] processes = Process.GetProcessesByName(processName);
                if (processes.Length == 0)
                {
                    this.AddInfo($"未找到进程:{processName}");
                }
                else
                {
                    this.AddInfo($"找到进程：{processName}，数量：{processes.Length}");
                    foreach (Process process in processes)
                    {
                        this.AddInfo($"名称：{processName},标题：{process.MainWindowTitle},句柄：{process.MainWindowHandle:X}，PID：{process.Id}");
                        HWND curHwnd = process.MainWindowHandle;
                        if(curHwnd == HWND.NULL)
                        {
                            this.AddInfo("未找到进程主窗口！");
                            if(string.IsNullOrWhiteSpace(config.Title))
                            {
                                continue;
                            }
                            curHwnd = User32.FindWindow(null, config.Title);
                            if (curHwnd == HWND.NULL)
                            {
                                this.AddInfo($"通过窗口标题未找到窗口：{config.Title}");
                                continue;
                            }
                        }
                        //if (string.IsNullOrWhiteSpace(process.MainWindowTitle))
                        //{
                        //    this.AddInfo("进程标题为空，不处理");
                        //    continue;
                        //}
                        HWND hwnd = User32.GetForegroundWindow();
                        if (hwnd == curHwnd)
                        {
                            this.AddInfo($"前台窗口已是当前窗口，不处理");
                            continue;
                        }
                        if (!User32.IsWindow(curHwnd))
                        {
                            this.AddInfo("非窗口句柄，不处理");
                            continue;
                        }
                        bool flag = false;
                        flag = User32.AllowSetForegroundWindow(uint.MaxValue);
                        this.AddInfo($"AllowSetForegroundWindow:{flag}");
                        flag = User32.SetForegroundWindow(curHwnd);
                        this.AddInfo($"SetForegroundWindow:{flag}");
                        User32.SwitchToThisWindow(curHwnd, true);
                        //User32.SetActiveWindow(process.MainWindowHandle);
                        //User32.ShowWindow(process.MainWindowHandle, ShowWindowCommand.SW_MAXIMIZE);
                        //User32.SetWindowPos(process.MainWindowHandle, HWND.HWND_TOPMOST, 0, 0, 0, 0, User32.SetWindowPosFlags.SWP_NOMOVE | User32.SetWindowPosFlags.SWP_NOSIZE);
                        //User32.SetWindowPos(process.MainWindowHandle, HWND.HWND_NOTOPMOST, 0, 0, 0, 0, User32.SetWindowPosFlags.SWP_NOMOVE | User32.SetWindowPosFlags.SWP_NOSIZE);
                    }
                    //User32.SwitchToThisWindow(Process.GetCurrentProcess().MainWindowHandle, true);
                }
            }
            catch(Exception ex)
            {
                this.AddInfo(ex.ToString());
            }
        }
        public void AddInfo(string msg)
        {
            StringBuilder sb = new();
            string txt = $"{DateTime.Now}:{msg}{Environment.NewLine}";
            Utils.LogUtil.AddInfo(txt);
            sb.Append(txt);
            sb.Append(this.Info);
            if(sb.Length > 1000)
            {
                sb.Remove(1000, sb.Length - 1000);
            }
            this.Info = sb.ToString();
        }
    }
}
