using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AlwaysDot
{
    public class KeyboardListener
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;

        private const string USER32 = "user32.dll";
        private const string KERNEL32 = "kernel32.dll";

        [DllImport(USER32, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport(USER32, CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport(USER32, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport(KERNEL32, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport(USER32)]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        [DllImport(USER32, SetLastError = true)]
        public static extern ushort GetKeyboardLayout([In] int idThread);

        [DllImport(USER32, SetLastError = true)]
        public static extern int GetWindowThreadProcessId([In] IntPtr hWnd, [Out, Optional] IntPtr lpdwProcessId);

        [DllImport(USER32, SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();
        public static string mss;

        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;

        public KeyboardListener()
        {
            _proc = HookCallback;
        }

        public void HookKeyboard()
        {
            _hookID = SetHook(_proc);
        }

        public void UnHookKeyboard()
        {
            UnhookWindowsHookEx(_hookID);
        }

        static ushort GetKeyboardLayout()
        {
            return GetKeyboardLayout(GetWindowThreadProcessId(GetForegroundWindow(), IntPtr.Zero));
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if (GetKeyboardLayout() == 1033)
                {
                    return (IntPtr)0;
                }
                else
                {
                    if (vkCode == 110)
                    {
                        UnHookKeyboard();
                        keybd_event((byte)191, 0, 1, 0);
                        HookKeyboard();
                        return (IntPtr)1;
                    }
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
    }
}
