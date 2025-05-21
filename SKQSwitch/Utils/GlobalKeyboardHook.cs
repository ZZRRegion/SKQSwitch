using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Printing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;

namespace SKQSwitch.Utils
{
    public class GlobalKeyboardHook : IDisposable
    {
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;
        private HookProc _hookProc;
        private HHOOK _hookId = HHOOK.NULL;
        public event EventHandler<KeyPressedEventArgs>? OnKeyPressed;
        public GlobalKeyboardHook()
        {
            this._hookProc = HookCallback;
        }
        public void Start()
        {
            this._hookId = this.SetHook();
            if(this._hookId == HHOOK.NULL)
            {
                Debug.WriteLine("设置HOOk失败");
            }
            else
            {
                Debug.WriteLine("设置HOOK成功");
            }
        }
        public void Stop()
        {
            if (this._hookId != HHOOK.NULL)
            {
                User32.UnhookWindowsHookEx(this._hookId);
                this._hookId = HHOOK.NULL;
            }
        }
        private User32.SafeHHOOK SetHook()
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule!)
            {
                return User32.SetWindowsHookEx(User32.HookType.WH_KEYBOARD_LL, this._hookProc, Kernel32.GetModuleHandle(curModule.ModuleName), 0);
            }
        }
        /// <summary>
        /// 钩子回调函数
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if(nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                try
                {
                    KBDLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);

                    System.Windows.Input.Key key = KeyInterop.KeyFromVirtualKey((int)hookStruct.vkCode);

                    //检查是否按下Ctrl、Alt或Shift
                    bool ctrl = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control; //(Control.ModifierKeys & Keys.Control) != 0;
                    bool alt = (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt;// (Control.ModifierKeys & Keys.Alt) != 0;
                    bool shift = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;// (Control.ModifierKeys & Keys.Shift) != 0;
                    Debug.WriteLine($"key:{key},ctrl:{ctrl},alt:{alt},shift:{shift}");
                    OnKeyPressed?.Invoke(this, new(key, ctrl, alt, shift));
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            return User32.CallNextHookEx(this._hookId, nCode, wParam, lParam);
        }
        public void Dispose()
        {
            this.Stop();
        }
        public class KeyPressedEventArgs : EventArgs
        {
            public System.Windows.Input.Key Key { get; private set; }
            public bool Control { get; private set; }
            public bool Alt { get; private set; }
            public bool Shift { get; private set; }
            public KeyPressedEventArgs(System.Windows.Input.Key key, bool ctrl, bool alt, bool shift)
            {
                this.Key = key;
                this.Control = ctrl;
                this.Alt = alt;
                this.Shift = shift;
            }
        }
    }
}
