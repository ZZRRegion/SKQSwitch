using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Printing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;

namespace SKQSwitch.Utils
{
    public class GlobalKeyboardHook : IDisposable
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;
        private HookProc _hookProc;
        private HHOOK _hookId = HHOOK.NULL;
        public event EventHandler<KeyPressedEventArgs>? OnKeyPressed;
        public GlobalKeyboardHook()
        {
            this._hookProc = new(HookCallback);
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
                UnhookWindowsHookEx((int)this._hookId.DangerousGetHandle());
                //User32.UnhookWindowsHookEx(this._hookId);
                this._hookId = HHOOK.NULL;
            }
        }
        private int SetHook()
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule!)
            {
                Kernel32.SafeHINSTANCE hinstance = Kernel32.GetModuleHandle(curModule.ModuleName);
                return SetWindowsHookEx((int)User32.HookType.WH_KEYBOARD_LL, this._hookProc, hinstance.DangerousGetHandle(), 0); //User32.SetWindowsHookEx(User32.HookType.WH_KEYBOARD_LL, _hookProc, hinstance, 0);
            }
        }
        /// <summary>
        /// 钩子回调函数
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private nint HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if(nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                try
                {
                    KBDLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);

                    System.Windows.Input.Key key = System.Windows.Input.KeyInterop.KeyFromVirtualKey((int)hookStruct.vkCode);

                    //检查是否按下Ctrl、Alt或Shift
                    bool ctrl = (System.Windows.Input.Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Control) == System.Windows.Input.ModifierKeys.Control; //(Control.ModifierKeys & Keys.Control) != 0;
                    bool alt = (System.Windows.Input.Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Alt) == System.Windows.Input.ModifierKeys.Alt;// (Control.ModifierKeys & Keys.Alt) != 0;
                    bool shift = (System.Windows.Input.Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Shift) == System.Windows.Input.ModifierKeys.Shift;// (Control.ModifierKeys & Keys.Shift) != 0;
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
