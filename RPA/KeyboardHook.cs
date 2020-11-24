using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RPA
{
    internal class KeyboardHook
    {
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WH_KEYBOARD_LL = 13;

        public event KeyEventHandler KeyDownEvent;
        public event KeyEventHandler KeyUpEvent;

        private delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);
        private static int HKeyboardHook = 0; // the handle of keyboard hook
        private HookProc KeyboardHookProcedure;

        private Recorder MyRecorder;

        [StructLayout(LayoutKind.Sequential)]
        private class KeyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        // 安装钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        // 卸载钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnhookWindowsHookEx(int idHook);

        // 继续下一个钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);

        // 获取当前实例
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string name);

        public void Start()
        {
            // 安装键盘钩子
            if (HKeyboardHook == 0)
            {
                KeyboardHookProcedure = new HookProc(KeyboardHookProc);
                HKeyboardHook = SetWindowsHookEx(
                    WH_KEYBOARD_LL,
                    KeyboardHookProcedure,
                    GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName),
                    0);

                if (HKeyboardHook == 0)
                {
                    Stop();
                    throw new Exception("failed to set keyboard hook");
                }
            }
        }

        public void Stop()
        {
            bool retKeyboard = true;

            if (HKeyboardHook != 0)
            {
                retKeyboard = UnhookWindowsHookEx(HKeyboardHook);
                KeyboardHookProcedure = null;
                HKeyboardHook = 0;
            }

            if (!retKeyboard)
            {
                throw new Exception("failed to unhook keyboard hook");
            }
        }

        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                KeyboardHookStruct MyKeyboardHookStruct =
                    (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));

                if (KeyDownEvent != null && wParam == WM_KEYDOWN)
                {
                    KeyEventArgs e = new KeyEventArgs((Keys)MyKeyboardHookStruct.vkCode);
                    // 具体实现
                    MyRecorder.MyRecordModel.AddKeyDownRecord(e.KeyCode.ToString());

                    MyRecorder.UpdateRecordCounter();

                    KeyDownEvent(this, e);
                }

                if (KeyUpEvent != null && wParam == WM_KEYUP)
                {
                    KeyEventArgs e = new KeyEventArgs((Keys)MyKeyboardHookStruct.vkCode);
                    // 具体实现
                    MyRecorder.MyRecordModel.AddKeyUpRecord(e.KeyCode.ToString());

                    MyRecorder.UpdateRecordCounter();

                    KeyUpEvent(this, e);
                }
            }
            return CallNextHookEx(HKeyboardHook, nCode, wParam, lParam);
        }

        public KeyboardHook(Recorder r)
        {
            MyRecorder = r;
        }

        ~KeyboardHook()
        {
            Stop();
        }
    }
}
