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
        public RuntimeView()
        {
            InitializeComponent();
            DataContext = new RuntimeViewModel();
        }

        private void UIElement_OnDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        private void UIElement_OnDrop(object sender, DragEventArgs e)
        {
            
        }

        private void UIElement_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragDrop.DoDragDrop((ListBox) sender, "test", DragDropEffects.Copy);
        }

        private void UIElement_OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            
        }
    }
}
