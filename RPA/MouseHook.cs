using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RPA
{
    internal class MouseHook
    {
        private const int WM_MOUSEMOVE = 0x200;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_MBUTTONDOWN = 0x207;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_RBUTTONUP = 0x205;
        private const int WM_MBUTTONUP = 0x208;
        private const int WM_MOUSEWHEEL = 0x20A;
        private const int WH_MOUSE_LL = 14;

        public event MouseEventHandler MouseEvent;

        private delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);
        private static int HMouseHook = 0; // the handle of mouse hook
        private HookProc MouseHookProcedure;

        private Recorder MyRecorder;

        [StructLayout(LayoutKind.Sequential)]
        private class MouseHookStruct
        {
            public Point pt;
            public int hWnd;
            public int wHitTestCode;
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
            // 安装鼠标钩子
            if (HMouseHook == 0)
            {
                MouseHookProcedure = new HookProc(MouseHookProc);
                HMouseHook = SetWindowsHookEx(
                    WH_MOUSE_LL,
                    MouseHookProcedure,
                    GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName),
                    0);

                if (HMouseHook == 0)
                {
                    Stop();
                    throw new Exception("failed to set mouse hook");
                }
            }
        }

        public void Stop()
        {
            bool retMouse = true;

            if (HMouseHook != 0)
            {
                retMouse = UnhookWindowsHookEx(HMouseHook);
                MouseHookProcedure = null;
                HMouseHook = 0;
            }

            if (!retMouse)
            {
                throw new Exception("failed to unhook mouse hook");
            }
        }

        private int MouseHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                MouseButtons button;
                short wheelDegree = 0;

                MouseHookStruct MyMouseHookStruct =
                    (MouseHookStruct)Marshal.PtrToStructure(
                        lParam,
                        typeof(MouseHookStruct));

                switch (wParam)
                {
                    case WM_LBUTTONDOWN:
                        button = MouseButtons.Left;
                        if (MyRecorder.MyRecordModel != null)
                            MyRecorder.MyRecordModel.AddMouseClickedRecord("Left", "Down");
                        break;
                    case WM_LBUTTONUP:
                        button = MouseButtons.Left;
                        if (MyRecorder.MyRecordModel != null)
                            MyRecorder.MyRecordModel.AddMouseClickedRecord("Left", "Up");
                        break;
                    case WM_RBUTTONDOWN:
                        button = MouseButtons.Right;
                        if (MyRecorder.MyRecordModel != null)
                            MyRecorder.MyRecordModel.AddMouseClickedRecord("Right", "Down");
                        break;
                    case WM_RBUTTONUP:
                        button = MouseButtons.Right;
                        if (MyRecorder.MyRecordModel != null)
                            MyRecorder.MyRecordModel.AddMouseClickedRecord("Right", "Up");
                        break;
                    case WM_MBUTTONDOWN:
                        button = MouseButtons.Middle;
                        if (MyRecorder.MyRecordModel != null)
                            MyRecorder.MyRecordModel.AddMouseClickedRecord("Middle", "Down");
                        break;
                    case WM_MBUTTONUP:
                        button = MouseButtons.Middle;
                        if (MyRecorder.MyRecordModel != null)
                            MyRecorder.MyRecordModel.AddMouseClickedRecord("Middle", "Up");
                        break;
                    case WM_MOUSEMOVE:
                        button = MouseButtons.None;
                        /* no need
                        if (MyRecorder.MyRecordModel != null)
                            MyRecorder.MyRecordModel.AddMouseMovedRecord()
                        */
                        break;
                    case WM_MOUSEWHEEL:
                        button = MouseButtons.None;
                        wheelDegree = (short)((MyMouseHookStruct.hWnd >> 16) & 0xffff);
                        /*
                        if (MyRecorder.MyRecordModel != null)
                            MyRecorder.MyRecordModel.AddMouseWheelRecord(wheelDegree);
                        */
                        break;
                    default:
                        return CallNextHookEx(HMouseHook, nCode, wParam, lParam);
                }

                MyRecorder.UpdateRecordCounter();
                MouseEvent(this,
                    new MouseEventArgs(
                    button,
                    1,
                    MyMouseHookStruct.pt.X,
                    MyMouseHookStruct.pt.Y,
                    wParam == WM_MOUSEWHEEL ? wheelDegree : 0));
            }
            return CallNextHookEx(HMouseHook, nCode, wParam, lParam);
        }

        public MouseHook(Recorder r)
        {
            MyRecorder = r;
        }

        ~MouseHook()
        {
            Stop();
        }
    }
}
