using SkinHolderDesktop.ViewModels.Shared;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Interop;

namespace SkinHolderDesktop.Views.Shared
{
    public partial class RegistroDetails : Window
    {
        public RegistroDetails(RegistroDetailsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) Loaded += (_, _) => EnableDarkTitleBar(this);
        }

        [SupportedOSPlatform("windows")]
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        [SupportedOSPlatform("windows")]
        public static void EnableDarkTitleBar(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            if (Environment.OSVersion.Version.Build >= 17763)
            {
                int useDarkMode = 1;
                _ = DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, sizeof(int));
            }
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}