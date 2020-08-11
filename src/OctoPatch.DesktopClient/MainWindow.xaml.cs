using System;
using System.Threading;
using System.Windows;

namespace OctoPatch.DesktopClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            OctoPatchClient client = new OctoPatchClient();
            await client.Setup(new Uri("http://localhost:5000/engineServiceHub"), CancellationToken.None);

            var nodes = await client.GetNodeDescriptions(CancellationToken.None);
        }
    }
}
