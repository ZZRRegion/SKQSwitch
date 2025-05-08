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
        private bool isPause = false;
        private int switchIndex = 0;
        private const string titleIndex1 = "C:\\Users\\stdio\\source\\repos\\PECon\\Debug\\PEDll.dll - Notepad++";
        private const string titleIndex2 = "TCP/UDP Socket 调试工具 V2.2";
        //private const string titleIndex3 = "企业微信";
        //private const string titleIndex4 = "Detect It Easy v3.10 [Windows 10 Version 2009] (x86_64)";

        private string[] titles = { titleIndex1, titleIndex2 };
        [ObservableProperty]
        private string info = string.Empty;
        [RelayCommand]
        private void Pause()
        {
            this.isPause = !this.isPause;
        }
        public MainWindowViewModel()
        {
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
            string title = titles[switchIndex % titles.Length];
            string classText = "TdxRibbon";
            HWND hwnd = User32.FindWindow(null, title);
            //HWND hwnd = User32.FindWindow(classText, null);
            if(hwnd == HWND.NULL)
            {
                this.AddInfo($"未找到窗口句柄,{classText}");
                //return;
            }
            hwnd = 0x21022;
            bool flag = false;
            User32.GetWindowThreadProcessId(hwnd, out uint pid);
            //User32.AllowSetForegroundWindow(pid);
            //flag = User32.SetForegroundWindow(hwnd);
            //this.AddInfo($"SetForegroundWindow:{flag}");
            User32.SwitchToThisWindow(hwnd, true);
            //User32.SetForegroundWindow(hwnd);
            User32.SetWindowPos(hwnd, HWND.HWND_TOPMOST, 0, 0, 0, 0, User32.SetWindowPosFlags.SWP_NOMOVE | User32.SetWindowPosFlags.SWP_NOSIZE);
            HWND forHwnd = User32.GetForegroundWindow();
            if(hwnd != forHwnd)
            {
                this.AddInfo($"未激活，当前激活窗口：{forHwnd.DangerousGetHandle()}");
            }
            //flag = User32.ShowWindow(hwnd, ShowWindowCommand.SW_SHOWNORMAL);
            //this.AddInfo($"ShowWindow:{flag}");
            switchIndex++;
        }
        private void AddInfo(string msg)
        {
            StringBuilder sb = new();
            sb.AppendLine($"{DateTime.Now}:{msg}");
            sb.Append(this.Info);
            if(sb.Length > 1000)
            {
                sb.Remove(1000, sb.Length - 1000);
            }
            this.Info = sb.ToString();
        }
    }
}
