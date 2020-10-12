using System.Windows.Controls;
using OctoPatch.DesktopClient.Models;

namespace OctoPatch.DesktopClient.Views
{
    /// <summary>
    /// Interaction logic for RestGetView.xaml
    /// </summary>
    [ConfigurationMap("40945D30-186D-4AEE-8895-058FB4759EFF:RestGetNode", typeof(RestGetModel))]
    public partial class RestGetView : UserControl
    {
        public RestGetView()
        {
            InitializeComponent();
        }
    }
}
