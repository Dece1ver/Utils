using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Utils.WinAPI.pInvoke
{
    [Serializable]
    [return: MarshalAs(UnmanagedType.Bool)]
    public delegate bool EnumWindowProc(IntPtr hWnd, IntPtr lParam);
}
