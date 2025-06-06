﻿using CommunityToolkit.Mvvm.ComponentModel;
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
    internal partial class MainWindowViewModel : ViewModelBase
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
        private string exeName = string.Empty;
        [ObservableProperty]
        private string keyName = string.Empty;
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
            if (this.IsInDesignMode)
                return;
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

            this.ExeName = string.IsNullOrWhiteSpace(config.DisplayText) ? config.ExeName : config.DisplayText;
            this.ExeName = $"{ExeName}?";//带?表示执行切换失败
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
                        //User32.ShowWindow(curHwnd, ShowWindowCommand.SW_RESTORE);
                        flag = User32.SetForegroundWindow(curHwnd);
                        this.AddInfo($"SetForegroundWindow:{flag}");
                        User32.SwitchToThisWindow(curHwnd, true);
                        this.ExeName = this.ExeName.Replace("?", string.Empty);//移除"?"
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
        private string lastMsg = string.Empty;
        private int INFO_MAX_LENGTH = 1000;
        public void AddInfo(string msg)
        {
            if (msg == lastMsg)
                return;
            lastMsg = msg;
            StringBuilder sb = new();
            string txt = $"{DateTime.Now}:{msg}{Environment.NewLine}";
            Utils.LogUtil.AddInfo(txt);
            sb.Append(txt);
            sb.Append(this.Info);
            if(sb.Length > INFO_MAX_LENGTH)
            {
                sb.Remove(INFO_MAX_LENGTH, sb.Length - INFO_MAX_LENGTH);
            }
            this.Info = sb.ToString();
        }
    }
}
