using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// <summary>
        /// 软件进程名称
        /// </summary>
        private string[] processNames = { "UltraGalvo" , "wps" };
        [ObservableProperty]
        private string info = string.Empty;
        [RelayCommand]
        private void Pause()
        {
            this.isPause = !this.isPause;
        }
        public MainWindowViewModel()
        {
            this.AddInfo("软件启动");
            DispatcherTimer dispatcherTimer = new()
            {
                Interval = TimeSpan.FromSeconds(3),
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
            string processName = this.processNames[switchIndex++ % this.processNames.Length];
            this.SwitchWindow(processName);
                
        }
        private void SwitchWindow(string processName)
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(processName);
                if (processes.Length == 0)
                {
                    this.AddInfo($"未找到进程:{processName}");
                }
                else
                {
                    foreach (Process process in processes)
                    {
                        this.AddInfo($"名称：{processName},标题：{process.MainWindowTitle},句柄：{process.MainWindowHandle:X}，PID：{process.Id}");
                        if (string.IsNullOrWhiteSpace(process.MainWindowTitle))
                        {
                            this.AddInfo("进程标题为空，不处理");
                            continue;
                        }
                        HWND hwnd = User32.GetForegroundWindow();
                        if (hwnd == process.MainWindowHandle)
                        {
                            this.AddInfo($"前台窗口已是当前窗口，不处理");
                            continue;
                        }
                        if (!User32.IsWindow(process.MainWindowHandle))
                        {
                            this.AddInfo("非窗口句柄，不处理");
                            continue;
                        }
                        User32.AllowSetForegroundWindow(uint.MaxValue);
                        User32.SetForegroundWindow(process.MainWindowHandle);
                        User32.SwitchToThisWindow(process.MainWindowHandle, true);
                        User32.SetActiveWindow(process.MainWindowHandle);
                        User32.ShowWindow(process.MainWindowHandle, ShowWindowCommand.SW_MAXIMIZE);
                        //User32.SetWindowPos(process.MainWindowHandle, HWND.HWND_TOPMOST, 0, 0, 0, 0, User32.SetWindowPosFlags.SWP_NOMOVE | User32.SetWindowPosFlags.SWP_NOSIZE);
                        //User32.SetWindowPos(process.MainWindowHandle, HWND.HWND_NOTOPMOST, 0, 0, 0, 0, User32.SetWindowPosFlags.SWP_NOMOVE | User32.SetWindowPosFlags.SWP_NOSIZE);

                    }
                }
            }
            catch(Exception ex)
            {
                this.AddInfo(ex.ToString());
            }
        }
        private void AddInfo(string msg)
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
