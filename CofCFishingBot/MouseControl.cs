using System.Windows;
using System.Runtime.InteropServices;

namespace CofCFishingBot
{
    public static class MouseControl
    {
        #region consts
        const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        #endregion

        #region Win32 DLLs
        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out Point lpPoint);

        [DllImport("user32.dll")]
        static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);
        #endregion

        #region ClickEvent
        /// <summary>
        /// Click Event an der Physikalischen Position des Screens
        /// </summary>
        /// <param name="point"></param>
        public static void ClickPhysical(Point point)
        {
            SetCursorPos((int)point.X, (int)point.Y);
            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, UIntPtr.Zero);
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, UIntPtr.Zero);
        }
        #endregion
    }
}
