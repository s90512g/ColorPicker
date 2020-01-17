using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ColorPicker
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        private static ReadColorValue ReadColor = new ReadColorValue();
        private EllipseGeometry Ellipse = new EllipseGeometry(new Point(0, 0), 3, 3);
        private static IntPtr hHook = IntPtr.Zero;
        private static bool IsMouseUp = true;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {            
            DataContext = ReadColor;
            ReadColor.DrawBlockEvent += new DrawBlockHandler(DrawBlock);
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ReadColor.StopReadPixel();
        }
        private void DrawBlock(DrawBlockEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Ellipse.Center = e.BlockLocation;
                DrawBox.Data = Ellipse;
            });
        }

        private void CustomColorBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CustomColorBox.SelectedItem is ColorData)
            {
                ColorData item = CustomColorBox.SelectedItem as ColorData;
                if (item.Localtion.X == -1 || item.Localtion.Y == -1)
                {
                    DrawBox.Data = null;
                }
                else
                {
                    Ellipse.Center = item.Localtion;
                    DrawBox.Data = Ellipse;
                }
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(HookType hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        private delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);
        private static HookProc hookProcDelegate = HookCallback;
        private const int WH_MOUSE_LL = 14;
        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (MouseMessages.WM_LBUTTONUP == (MouseMessages)wParam)
            {
                IsMouseUp = true;
                ReadColor.SuspendReadPixel();
                UnhookWindowsHookEx(hHook);
                hHook = IntPtr.Zero;
            }
            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }
        private void ColorImg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CustomColorBox.SelectedIndex == -1)
                return;
            IsMouseUp = false;
            ReadColor.UpdateInfo(e.GetPosition(ColorImg), CustomColorBox.SelectedIndex);
            ReadColor.StartReadPixel();
            InstallMouseEvnetHook();
        }
        private void InstallMouseEvnetHook()
        {
            using (Process process = Process.GetCurrentProcess())
            using (ProcessModule module = process.MainModule)
            {
                IntPtr hModule = GetModuleHandle(module.ModuleName);
                hHook = SetWindowsHookEx(HookType.WH_MOUSE_LL, hookProcDelegate, hModule, 0);
            }
        }

        private void ColorImg_MouseMove(object sender, MouseEventArgs e)
        {
            if (!IsMouseUp)
            {
                ReadColor.UpdateInfo(e.GetPosition(ColorImg));
            }
        }
    }
}
