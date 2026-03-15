using SkinHolderDesktop.ViewModels.Shared;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace SkinHolderDesktop.Views.Shared
{
    public partial class RegistroDetails : Window
    {
        public RegistroDetails()
        {
            InitializeComponent();

            Loaded += async (_, _) => await LoadIconAsync();
        }

        private static readonly HttpClient _httpClient = new();
        private const string IconUrl = "https://cdn.jagoba.dev/imgs/logo.ico";

        private async Task LoadIconAsync()
        {
            try
            {
                var bytes = await _httpClient.GetByteArrayAsync(IconUrl);
                using var stream = new MemoryStream(bytes);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze();
                TitleBarIcon.Source = bitmap;
            }
            catch { }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        private void BtnMinimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
    }
}