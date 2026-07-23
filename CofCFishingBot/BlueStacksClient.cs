using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace CofCFishingBot
{
    public class BlueStacksClient
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT { public int Left, Top, Right, Bottom; }

        #region Win32 DLLs
        [DllImport("user32.dll")]
        public static extern bool SetProcessDpiAwarenessContext(IntPtr dpiContext);

        [DllImport("user32.dll")]
        public static extern uint GetDpiForWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
        #endregion

        public static IntPtr GetProcess()
        {
            var procs = Process.GetProcessesByName("HD-Player");
            if (procs.Length == 0) 
                return IntPtr.Zero;

            IntPtr hwnd = procs[0].MainWindowHandle;
            return hwnd;
        }
    }
}
