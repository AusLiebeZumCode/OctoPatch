using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using OctoPatch.Descriptions;
using OctoPatch.DesktopClient.Models;
using OctoPatch.Setup;

namespace OctoPatch.DesktopClient.ViewModels
{
    public interface IRuntimeViewModel : INotifyPropertyChanged
    {
        ObservableCollection<NodeDescription> NodeDescriptions { get; }

        NodeDescription SelectedNodeDescription { get; set; }

        ICommand AddSelectedNodeDescription { get; }

        ObservableCollection<NodeDescription> ContextNodeDescriptions { get; }

        NodeDescription SelectedContextNodeDescription { get; set; }

        ICommand AddSelectedContextNodeDescription { get; }

        ObservableCollection<NodeModel> NodeTree { get; }

        ICommand RemoveSelectedNode { get; }

        ICommand StartSelectedNode { get; }

        ICommand StopSelectedNode { get; }

        NodeModel SelectedNode { get; set; }

        NodeDescriptionModel NodeDescription { get; }

        ICommand SaveNodeDescription { get; }

        NodeConfigurationModel NodeConfiguration { get; }

        ICommand SaveNodeConfiguration { get; }

        OutputNodeModel SelectedWireConnector { get; }

        ICommand TakeConnector { get; }

        ObservableCollection<WireSetup> Wires { get; }
    }
}
