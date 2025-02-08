using System;
using System.Runtime.InteropServices;

namespace SteamP2PInfo.WinAPI
{
    [StructLayout(LayoutKind.Sequential)]
    public struct KBDLLHOOKSTRUCT
    {
        public int vkCode;
        public int scanCode;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }
}
