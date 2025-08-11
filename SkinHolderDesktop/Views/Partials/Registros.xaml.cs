using SkinHolderDesktop.ViewModels;
using System.Windows.Controls;

namespace SkinHolderDesktop.Views.Partials
{
    /// <summary>
    /// Lógica de interacción para Registros.xaml
    /// </summary>
    public partial class Registros : UserControl
    {
        public Registros(RegistrosViewModel registrosViewModel)
        {
            InitializeComponent();

            DataContext = registrosViewModel;
        }
    }
}
