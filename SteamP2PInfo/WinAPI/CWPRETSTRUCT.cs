using System;
using System.Runtime.InteropServices;

namespace SteamP2PInfo.WinAPI
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CWPRETSTRUCT
    {
        public uint lResult;
        public IntPtr lParam;
        public IntPtr wParam;
        public uint message;
        public IntPtr hwnd;
    }
}
