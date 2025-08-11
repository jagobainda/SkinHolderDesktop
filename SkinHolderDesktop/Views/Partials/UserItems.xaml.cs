using SkinHolderDesktop.ViewModels;
using System.Windows.Controls;

namespace SkinHolderDesktop.Views.Partials;

public partial class UserItems : UserControl
{
    public UserItems(UserItemsViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }
}
