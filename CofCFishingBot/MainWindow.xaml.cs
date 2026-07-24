using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static CofCFishingBot.BlueStacksClient;

namespace CofCFishingBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int HOTKEY_ID = 9000;
        private const uint MOD_NONE = 0x0000;
        private const uint VK_F6 = 0x75;

        #region Win32 DLLs
        [DllImport("user32.dll")]
        static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);


        #endregion

        #region readonly consts
        static readonly Color Angelbtn1 = Color.FromRgb(97, 184, 235);
        static readonly Color Angelbtn2 = Color.FromRgb(243, 162, 82);
        static readonly Color Angelbtn3 = Color.FromRgb(67, 106, 48);
        static readonly Color Angelbtn4 = Color.FromRgb(49, 41, 17);
        static readonly Color Schwimmer = Color.FromRgb(214, 61, 37);
        static readonly Color Fangbereich = Color.FromRgb(254, 244, 60);
        //static readonly Int32Rect scanArea = new Int32Rect(145, 656, 250, 20);
        static readonly IntPtr DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = new IntPtr(-4);
        #endregion

        private CancellationTokenSource? _cts;

        public MainWindow()
        {
            InitializeComponent();
            BlueStacksClient.SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);

        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var helper = new WindowInteropHelper(this);
            HwndSource source = HwndSource.FromHwnd(helper.Handle);

            source.AddHook(HwndHook);
            RegisterHotKey(helper.Handle, HOTKEY_ID, MOD_NONE, VK_F6);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;

            if (msg == WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID)
            {
                Dispatcher.Invoke(() => ToggleCapture());
                handled = true;
            }

            return IntPtr.Zero;
        }

        protected override void OnClosed(EventArgs e)
        {
            var helper = new WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, HOTKEY_ID);

            base.OnClosed(e);
        }

        public async Task BotAsync(CancellationToken cancellationToken)
        {
            var hwnd = BlueStacksClient.GetProcess();
            if (hwnd == IntPtr.Zero)
                return;

            uint dpi = GetDpiForWindow(hwnd);
            double scaling = dpi / 96.0;

            Console.WriteLine($"DPI={dpi}, scaling={scaling}");

            if (!GetClientRect(hwnd, out RECT clientRect))
            {
                Console.WriteLine("GetClientRect failed");
                return;
            }

            int clientW = clientRect.Right - clientRect.Left;
            int clientH = clientRect.Bottom - clientRect.Top;
            Console.WriteLine($"Client={clientW}x{clientH}");

            BitmapSource bmpSetup = ImageHandler.CaptureClientWindow(hwnd);
            Point pointClient = new Point(clientW / 2, (int)(clientH / 1.175));//262 826
            POINT pt = new POINT((int)pointClient.X, (int)pointClient.Y);
            ClientToScreen(hwnd, ref pt);
            Point pointScreen = new Point(pt.X, pt.Y);
            Int32Rect scanArea = new Int32Rect((int)(clientW / 3.5), (int)(clientH / 1.47), (int)(clientW / 2.1), 1);
            bool alreadyclicked = false;

            while (!cancellationToken.IsCancellationRequested)
            {

                BitmapSource bmp = ImageHandler.CaptureClientWindow(hwnd);

                if (pointClient.X >= 0 && pointClient.Y >= 0 && pointClient.X < bmp.Width && pointClient.Y < bmp.Height)
                {
                    Color pixel = ImageHandler.GetPixel(bmp, (int)pointClient.X, (int)pointClient.Y);
                    if (TriggerDetection.CheckIsClose(pixel, Angelbtn1, 50))
                    {
                        alreadyclicked = false;
                        MouseControl.ClickPhysical(pointScreen);
                    }
                    if (TriggerDetection.CheckIsClose(pixel, Angelbtn2, 30))
                    {

                        if (!alreadyclicked) { MouseControl.ClickPhysical(pointScreen); }
                        alreadyclicked = true;
                        bool hit = TriggerDetection.CheckYellowWithRedPoint(bmp, scanArea, Fangbereich, Schwimmer);
                        if (hit)
                        {
                            MouseControl.ClickPhysical(pointScreen);
                        }
                    }
                    if (TriggerDetection.CheckIsClose(pixel, Angelbtn3, 50))
                    {
                        MouseControl.ClickPhysical(pointScreen);
                        alreadyclicked = false;
                    }
                    if (TriggerDetection.CheckIsClose(pixel, Angelbtn4, 50))
                    {
                        MouseControl.ClickPhysical(pointScreen);
                        alreadyclicked = false;
                    }
                }
                await Task.Delay(50, cancellationToken);
            }
        }

        private async void Start_Button_Click(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;

            // Abgebrochene Tasks/Schleifen zurücksetzen
            _cts = new CancellationTokenSource();

            try
            {
                // Startet die Endlosschleife im Hintergrund, ohne die UI zu blockieren
                await BotAsync(_cts.Token);
            }
            catch
            {
                StartButton.IsEnabled = true;
                StopButton.IsEnabled = false;
                _cts.Dispose();
                _cts = null;
            }
        }

        private void Stop_Button_Click(object sender, RoutedEventArgs e)
        {
            _cts?.Cancel();
        }

        private void ToggleCapture()
        {
            if (_cts == null)
            {
                Start_Button_Click(this, new RoutedEventArgs());
            }
            else
            {
                Stop_Button_Click(this, new RoutedEventArgs());
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }
        }
    }
}