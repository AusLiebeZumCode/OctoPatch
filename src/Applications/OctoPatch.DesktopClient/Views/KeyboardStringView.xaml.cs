using System.Windows.Controls;
using OctoPatch.DesktopClient.Models;

namespace OctoPatch.DesktopClient.Views
{
    /// <summary>
    /// Interaction logic for KeyboardStringView.xaml
    /// </summary>
    [ConfigurationMap("a6fe76d7-5f0e-4763-a3a5-fcaf43c71464:KeyboardStringNode", typeof(KeyboardStringModel))]
    public partial class KeyboardStringView : UserControl
    {
        public KeyboardStringView()
        {
            InitializeComponent();
        }
    }
}
