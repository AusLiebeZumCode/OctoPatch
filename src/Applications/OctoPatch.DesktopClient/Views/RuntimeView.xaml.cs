using System.Windows.Controls;
using OctoPatch.DesktopClient.ViewModels;

namespace OctoPatch.DesktopClient.Views
{
    /// <summary>
    /// Interaction logic for RuntimeView.xaml
    /// </summary>
    public partial class RuntimeView : UserControl
    {
        public RuntimeView()
        {
            InitializeComponent();
            DataContext = new RuntimeViewModel();
        }
    }
}
