using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AlwaysDot
{
	static public class WinApi
	{
		private const string USER32 = "user32.dll";

		[DllImport(USER32)]
		public static extern int RegisterWindowMessage(string message);

		public static int RegisterWindowMessage(string format, params object[] args)
		{
			string message = String.Format(format, args);
			return RegisterWindowMessage(message);
		}

		public const int HWND_BROADCAST = 0xffff;
		public const int SW_SHOWNORMAL = 1;

		[DllImport(USER32)]
		public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

		[DllImportAttribute(USER32)]
		public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImportAttribute(USER32)]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		public static void ShowToFront(IntPtr window)
		{
			ShowWindow(window, SW_SHOWNORMAL);
			SetForegroundWindow(window);
		}
	}
}
