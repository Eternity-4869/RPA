using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace RPA
{
    internal class Record
    {
        public String ComputerId { get; set; }
        public String Timestamp { get; set; }
        public int HourSpan { get; set; }
        public String Event { get; set; }
        public Point UIAPoint { get; set; }
        public IntPtr UIAFocusHandle { get; set; } // no need to save
        public IntPtr UIACursorPosHandle { get; set; } // no need to save
        public String UIAWindowText { get; set; }
        public Rect UIAWindowRect { get; set; }
        public Rect UIAClientRect { get; set; }
        public String UIAKeyCode { get; set; }
        public String UIAButtonClicked { get; set; }
        public String UIAClickType { get; set; }
        public short UIAWheelDegree { get; set; }
        public String UIAClipboard { get; set; }
        public String UIAFileName { get; set; }

        public String UIAControlInfo { get; set; }

        public Record()
        {
            ComputerId = "";
            Timestamp = "";
            HourSpan = 100;
            Event = "";
            UIAPoint = new Point(-1, -1);
            UIAFocusHandle = (IntPtr)0;
            UIACursorPosHandle = (IntPtr)0;
            UIAWindowText = "";
            UIAWindowRect = new Rect(0, 0, 0, 0);
            UIAClientRect = new Rect(0, 0, 0, 0);
            UIAKeyCode = "";
            UIAButtonClicked = "";
            UIAClickType = "";
            UIAWheelDegree = 0;
            UIAClipboard = "";
            UIAFileName = "";
            UIAControlInfo = "";
        }

        public static Record CreateRecord()
        {
            Record record = new Record();

            DateTime now = DateTime.Now;
            record.Timestamp = now.ToString("yyyy-MM-dd-HH-mm-ss-fff");
            record.HourSpan = TimeZoneInfo.Local.BaseUtcOffset.Hours;

            return record;
        }

        public static Record CreateInitRecord()
        {
            Record record = CreateRecord();
            record.Event = "Init";

            record.ComputerId = ComputerInfo.GetId();

            return record;
        }

        [DllImport("user32.dll", EntryPoint = "GetCursorPos")]
        private static extern bool GetCursorPos(ref Point point);

        [DllImport("user32.dll", EntryPoint = "WindowFromPoint")]
        private static extern IntPtr WindowFromPoint(Point point);

        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", EntryPoint = "GetWindowTextLength")]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetWindowText")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder buffer, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "GetWindowInfo")]
        private static extern int GetWindowInfo(IntPtr hwnd, ref WindowInfo wi);

        [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId")]
        private static extern int GetWindowThreadProcessId(IntPtr handle, ref int processId);

        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public Rect(int l, int t, int r, int b)
            {
                Left = l;
                Top = t;
                Right = r;
                Bottom = b;
            }
        }

        public struct WindowInfo
        {
            public uint cbSize;
            public Rect rcWindow;
            public Rect rcClient;
            public uint dwStyle;
            public uint dwExStyle;
            public uint dwWindowStatus;
            public uint cxWindowBorders;
            public uint cyWindowBorders;
            public short atomWindowType;
            public short wCreatorVersion;
        }

        public Record AddFocusInfo()
        {
            UIAFocusHandle = GetForegroundWindow();
            return this;
        }

        public Record AddCursorInfo()
        {
            Point p = new Point(-1, -1);
            GetCursorPos(ref p);

            UIAPoint = p;
            UIACursorPosHandle = WindowFromPoint(p);
            return this;
        }

        public Record AddWindowInfo()
        {
            WindowInfo info = new WindowInfo();
            if (UIAFocusHandle != (IntPtr)0)
            {
                StringBuilder s = new StringBuilder(GetWindowTextLength(UIAFocusHandle) + 1);
                GetWindowText(UIAFocusHandle, s, s.Capacity);
                UIAWindowText = s.ToString();
                GetWindowInfo(UIAFocusHandle, ref info);

                int pid = 0;
                GetWindowThreadProcessId(UIAFocusHandle, ref pid);
                Process p = Process.GetProcessById(pid);
                UIAFileName = p.MainModule.FileName;
            }
            else if (UIACursorPosHandle != (IntPtr)0)
            {
                StringBuilder s = new StringBuilder(GetWindowTextLength(UIACursorPosHandle) + 1);
                GetWindowText(UIACursorPosHandle, s, s.Capacity);
                UIAWindowText = s.ToString();
                GetWindowInfo(UIACursorPosHandle, ref info);

                int pid = 0;
                GetWindowThreadProcessId(UIACursorPosHandle, ref pid);
                Process p = Process.GetProcessById(pid);
                UIAFileName = p.MainModule.FileName;
            }
            UIAWindowRect = new Rect((info.rcWindow.Left + (int)info.cxWindowBorders),
                (info.rcWindow.Top + (int)info.cyWindowBorders),
                (info.rcWindow.Right - (int)info.cxWindowBorders),
                (info.rcWindow.Bottom - (int)info.cyWindowBorders));
            UIAClientRect = new Rect(info.rcClient.Left,
                info.rcClient.Top,
                info.rcClient.Right,
                info.rcClient.Bottom);
            return this;
        }
    }
}
