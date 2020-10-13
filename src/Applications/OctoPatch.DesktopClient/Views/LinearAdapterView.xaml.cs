using System.Windows.Controls;
using OctoPatch.DesktopClient.Models;

namespace OctoPatch.DesktopClient.Views
{
    /// <summary>
    /// Interaction logic for LinearAdapterView.xaml
    /// </summary>
    [ConfigurationMap("598D58EB-756D-4BF7-B04B-AC9603315B6D:LinearTransformationAdapter", typeof(LinearAdapterModel))]
    public partial class LinearAdapterView : UserControl
    {
        public LinearAdapterView()
        {
            InitializeComponent();
        }
    }
}
