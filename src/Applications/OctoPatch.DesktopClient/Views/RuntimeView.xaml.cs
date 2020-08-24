using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using OctoPatch.DesktopClient.ViewModels;

namespace OctoPatch.DesktopClient.Views
{
    /// <summary>
    /// Interaction logic for RuntimeView.xaml
    /// </summary>
    public partial class RuntimeView : UserControl
    {
        private readonly RuntimeViewModel _viewModel;

        public RuntimeView()
        {
            InitializeComponent();
            DataContext = _viewModel = new RuntimeViewModel();
        }
    }
}
