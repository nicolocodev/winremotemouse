using System;
using System.Runtime.InteropServices;

namespace WinRemoteMouse.Server
{
    internal class MouseInterop
    {
        internal const int MouseeventLeftDown = 0x02;
        internal const int MouseeventLeftUp = 0x04;
        internal const int MouseEventRightDown = 0x00000008;
        internal const int MouseEventRightUp = 0x00000010;

        [DllImport("user32.dll")]
        internal static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll",EntryPoint = "mouse_event")]
        internal static extern void MouseEvent(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(out MousePointer lpPoint);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MousePointer
    {
        public int X;
        public int Y;

        public MousePointer(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}