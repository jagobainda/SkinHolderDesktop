using SkinHolderDesktop.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SkinHolderDesktop.Views.Partials
{
    public partial class UserSettings : UserControl
    {
        private readonly UserSettingsViewModel _viewModel;

        public UserSettings(UserSettingsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;

            Loaded += async (_, _) => await _viewModel.InitializeCommand.ExecuteAsync(null);
        }

        private void CurrentPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_viewModel.CurrentPassword != ((PasswordBox)sender).Password)
                _viewModel.CurrentPassword = ((PasswordBox)sender).Password;
        }

        private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_viewModel.NewPassword != ((PasswordBox)sender).Password)
                _viewModel.NewPassword = ((PasswordBox)sender).Password;
        }

        private void ConfirmNewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_viewModel.ConfirmNewPassword != ((PasswordBox)sender).Password)
                _viewModel.ConfirmNewPassword = ((PasswordBox)sender).Password;
        }

        private void DeletePasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_viewModel.DeletePassword != ((PasswordBox)sender).Password)
                _viewModel.DeletePassword = ((PasswordBox)sender).Password;
        }
    }
}
