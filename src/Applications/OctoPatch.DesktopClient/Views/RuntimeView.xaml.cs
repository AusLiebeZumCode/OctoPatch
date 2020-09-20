using System.Windows;
using System.Windows.Controls;
using OctoPatch.DesktopClient.Models;
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

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _viewModel.SelectedNode = e.NewValue as NodeModel;
        }
    }
}
