using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using OctoPatch.DesktopClient.Models;

namespace OctoPatch.DesktopClient.ViewModels
{
    public interface IRuntimeViewModel : INotifyPropertyChanged
    {
        ObservableCollection<NodeDescription> NodeDescriptions { get; }

        NodeDescription SelectedNodeDescription { get; set; }

        ICommand AddSelectedNodeDescription { get; }

        ObservableCollection<NodeInstance> Nodes { get; }

        ICommand RemoveSelectedNode { get; }

        ICommand StartSelectedNode { get; }

        ICommand StopSelectedNode { get; }

        public NodeInstance SelectedNode { get; set; }

        public NodeDescriptionModel NodeDescription { get; }

        ICommand SaveNodeDescription { get; }

        public ObservableCollection<WireInstance> Wires { get; }
    }
}
