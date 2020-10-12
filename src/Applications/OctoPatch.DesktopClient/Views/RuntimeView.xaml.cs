using System.ComponentModel;
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

            _viewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RuntimeViewModel.NodeConfiguration))
            {
                // Create proper node configuration control
                NodeConfigurationContainer.Content = ConfigurationMap.GetConfigurationView(_viewModel.NodeConfiguration);
            }
            else if (e.PropertyName == nameof(RuntimeViewModel.AdapterConfiguration))
            {
                // Create proper adapter configuration control
                AdapterConfigurationContainer.Content = ConfigurationMap.GetConfigurationView(_viewModel.NodeConfiguration);
            }
        }

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _viewModel.SelectedNode = e.NewValue as NodeModel;
        }
    }
}
